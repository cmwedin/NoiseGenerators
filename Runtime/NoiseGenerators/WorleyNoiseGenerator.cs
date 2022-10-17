using System;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public enum TextureChannel {R,G,B,A,All}

    public class WorleyNoiseGenerator : AbstractNoiseGenerator
    {
        public WorleyNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _cellCount,
            TextureChannel _activeChannel
        ) : base(_texWidth, _texHeight, _seed) {
            CellCounts = _cellCount;
            activeChannel = _activeChannel;
            pointsBuffer = new ComputeBuffer(CellCounts.x*CellCounts.y, 2 * sizeof(float));
            minMaxBuffer = new ComputeBuffer(2, sizeof(uint));
            minMaxBuffer.SetData(new int[] { int.MaxValue, 0 });
        }

        protected override string ComputeShaderPath => "Compute/WorleyNoise";
        private int GeneratePointsKernel => NoiseGenShader.FindKernel("GeneratePoints");
        private Vector3Int PointThreadGroupCount { get => new Vector3Int(
            Mathf.CeilToInt((float)CellCounts.x/threadGroupSize.x),
            Mathf.CeilToInt((float)CellCounts.y/threadGroupSize.y),
            1
        );}

        protected int NormalizeTextureKernel => NoiseGenShader.FindKernel("NormalizeTexture");
        private ComputeBuffer pointsBuffer;
        private ComputeBuffer minMaxBuffer;

//? Texture Parameters
        private TextureChannel activeChannel;
        private Vector4 channelMask {
            get => new Vector4(
                    activeChannel == TextureChannel.R ? 1 : 0, 
                    activeChannel == TextureChannel.G ? 1 : 0, 
                    activeChannel == TextureChannel.B ? 1 : 0, 
                    activeChannel == TextureChannel.A ? 1 : 0
                );
        }
        private int binChannelMask { get => (int)(channelMask.x * (2 ^ 0) + channelMask.y * (2 ^ 1) + channelMask.z * (2 ^ 2) + channelMask.w * (2 ^ 3)); }
        public bool InvertTexture { get; set; }
        private Vector2Int cellCounts;
        public Vector2Int CellCounts {
            get => cellCounts;
            set {
                if (value.x < 0 || value.y < 0) { value = new Vector2Int(Math.Abs(value.x), Math.Abs(value.y)); }
                if (value.x == 0) { value.x++; }
                if (value.y == 0) { value.y++; }
                // if (value.x > TexWidth / 10 || value.y > TexHeight / 10) {value = new Vector2Int((int)Math.Min(value.x,TexWidth / 10),(int)Math.Min(value.y,TexHeight)); }
                cellCounts = value;
            }
        }

        protected override void SetShaderParameters()
        {
            base.SetShaderParameters();
            NoiseGenShader.SetInt("_Seed",(int)(Seed << binChannelMask));
            NoiseGenShader.SetInt("_CellXCount", cellCounts.x);
            NoiseGenShader.SetInt("_CellYCount", cellCounts.y);
            NoiseGenShader.SetInt("_PointCellWidth", Mathf.FloorToInt(TexWidth/cellCounts.x));
            NoiseGenShader.SetInt("_PointCellHeight", Mathf.FloorToInt(TexHeight/cellCounts.y));
            NoiseGenShader.SetVector("_ChannelMask", channelMask);
            NoiseGenShader.SetBool("_Tiling", RequireSeamlessTiling);
            NoiseGenShader.SetBool("_Invert", InvertTexture);
            NoiseGenShader.SetBuffer(generateTextureKernel, "_PointsBuffer", pointsBuffer);
            NoiseGenShader.SetBuffer(GeneratePointsKernel, "_PointsBuffer", pointsBuffer);
            NoiseGenShader.SetBuffer(generateTextureKernel, "_MinMaxBuffer", minMaxBuffer);
            NoiseGenShader.SetBuffer(NormalizeTextureKernel, "_MinMaxBuffer", minMaxBuffer);
            NoiseGenShader.SetTexture(NormalizeTextureKernel, "_NoiseTexture", noiseTexture);
        }

        public override void GenerateTexture()
        {
            SetShaderParameters();
            if(activeChannel == TextureChannel.All) {
                for (int i = 0; i < 4; i++)
                {
                    activeChannel = (TextureChannel)i;
                    SetShaderParameters();
                    NoiseGenShader.Dispatch(GeneratePointsKernel, PointThreadGroupCount.x, PointThreadGroupCount.y, PointThreadGroupCount.z);
                    NoiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
                    NoiseGenShader.Dispatch(NormalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
                }
                activeChannel = TextureChannel.All;
            } else {
                NoiseGenShader.Dispatch(GeneratePointsKernel, PointThreadGroupCount.x, PointThreadGroupCount.y, PointThreadGroupCount.z);
                NoiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
                NoiseGenShader.Dispatch(NormalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            }
            pointsBuffer.Release();
            minMaxBuffer.Release();
        }

        private bool disposedValue = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state 
                }

                pointsBuffer?.Release();
                pointsBuffer = null;
                minMaxBuffer?.Release();
                minMaxBuffer = null;
            }
            base.Dispose(disposing);
        }
        ~WorleyNoiseGenerator() {
            Dispose(disposing: false);
        }
    }
}