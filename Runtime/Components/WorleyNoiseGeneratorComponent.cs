using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class WorleyNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        protected override string ComputeShaderPath => "Compute/WorleyNoise";
        protected override int generateTextureKernel => NoiseGenShader.FindKernel("CSMain");
        private int GeneratePointsKernel => NoiseGenShader.FindKernel("GeneratePoints");
        protected int NormalizeTextureKernel => NoiseGenShader.FindKernel("NormalizeTexture");

        enum TextureChannel {R,G,B,A,All}
        [SerializeField] private TextureChannel activeChannel;
        private Vector4 channelMask {
            get => new Vector4(
                    activeChannel == TextureChannel.R ? 1 : 0, 
                    activeChannel == TextureChannel.G ? 1 : 0, 
                    activeChannel == TextureChannel.B ? 1 : 0, 
                    activeChannel == TextureChannel.A ? 1 : 0
                );
        }
        private int binChannelMask { get => (int)(channelMask.x * (2 ^ 0) + channelMask.y * (2 ^ 1) + channelMask.z * (2 ^ 2) + channelMask.w * (2 ^ 3)); }
        [SerializeField] private bool requireTiling;
        [SerializeField] private bool invertTexture;

        [SerializeField, Tooltip("The number of cells along the x and y axis respectively")] 
        private Vector2Int cellCounts;
        private Vector3Int PointThreadGroupCount { get => new Vector3Int(
            Mathf.CeilToInt((float)cellCounts.x/threadGroupSize.x),
            Mathf.CeilToInt((float)cellCounts.y/threadGroupSize.y),
            1
        );}
        private ComputeBuffer pointsBuffer;
        private ComputeBuffer minMaxBuffer;

        protected override void CleanUpOldTextures()
        {
            base.CleanUpOldTextures();
            pointsBuffer?.Release();
            minMaxBuffer?.Release();
        }

        protected override void SetShaderParameters()
        {
            if (cellCounts.x <= 0 || cellCounts.y <= 0) { throw new System.ArgumentException("the number of cells along each axis must be greater than 0"); }
            base.SetShaderParameters();
            NoiseGenShader.SetInt("_Seed",(int)(seed << binChannelMask));
            NoiseGenShader.SetInt("_CellXCount", cellCounts.x);
            NoiseGenShader.SetInt("_CellYCount", cellCounts.y);
            NoiseGenShader.SetInt("_PointCellWidth", Mathf.FloorToInt(texWidth/cellCounts.x));
            NoiseGenShader.SetInt("_PointCellHeight", Mathf.FloorToInt(texHeight/cellCounts.y));
            NoiseGenShader.SetVector("_ChannelMask", channelMask);
            NoiseGenShader.SetBool("_Tiling", requireTiling);
            NoiseGenShader.SetBool("_Invert", invertTexture);
            NoiseGenShader.SetBuffer(generateTextureKernel, "_PointsBuffer", pointsBuffer);
            NoiseGenShader.SetBuffer(GeneratePointsKernel, "_PointsBuffer", pointsBuffer);
            NoiseGenShader.SetBuffer(generateTextureKernel, "_MinMaxBuffer", minMaxBuffer);
            NoiseGenShader.SetBuffer(NormalizeTextureKernel, "_MinMaxBuffer", minMaxBuffer);
            NoiseGenShader.SetTexture(NormalizeTextureKernel, "_NoiseTexture", noiseTexture);
        }

        public override void GenerateTexture()
        {
            if (cellCounts.x <= 0 || cellCounts.y <= 0) {
                Debug.LogWarning("the number of cells along each axis must be greater than 0");
                return;
            }
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.wrapMode = TextureWrapMode.Repeat;
            noiseTexture.Create();
            pointsBuffer = new ComputeBuffer(cellCounts.x*cellCounts.y, 2 * sizeof(float));
            minMaxBuffer = new ComputeBuffer(2, sizeof(uint));
            minMaxBuffer.SetData(new int[] { int.MaxValue, 0 });
            try {
                SetShaderParameters();
            } catch (System.ArgumentException ex) {
                Debug.LogWarning(ex);
                CleanUpOldTextures();
                return;
            }catch (System.Exception){
                throw;
            }
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
            DisplayTexture();
            pointsBuffer.Release();
            minMaxBuffer.Release();
        }

        protected override void DisplayTexture()
        {
            base.DisplayTexture();
        }
    }
}