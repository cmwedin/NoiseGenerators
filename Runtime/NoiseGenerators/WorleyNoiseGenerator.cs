using System;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{

    public class WorleyNoiseGenerator : AbstractNoiseGenerator
    {
        /// <summary>
        /// Constructs a WorleyNoiseGenerator
        /// </summary>
        /// <param name="_texWidth">The width of the generated texture</param>
        /// <param name="_texHeight">The height of the generated texture</param>
        /// <param name="_seed">The seed of the pseudo-random number generation</param>
        /// <param name="_cellCount">The number of cells along each axis to place a point int</param>
        /// <param name="_activeChannel">The channel to place the generated noise in, if TextureChannel.All generated a separate texture for each channel</param>
        public WorleyNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _cellCount,
            TextureChannel _activeChannel
        ) : base(_texWidth, _texHeight, _seed) {
            CellCounts = _cellCount;
            ActiveChannel = _activeChannel;
            pointsBuffer = new ComputeBuffer(CellCounts.x*CellCounts.y, 2 * sizeof(float));
            minMaxBuffer = new ComputeBuffer(2, sizeof(uint));
            minMaxBuffer.SetData(new int[] { int.MaxValue, 0 });
        }

        protected override string ComputeShaderPath => "Compute/WorleyNoise";
        /// <summary>
        /// The kernel for the method to place the control points in the compute shader
        /// </summary>
        private int GeneratePointsKernel => NoiseGenShader.FindKernel("GeneratePoints");
        /// <summary>
        /// The number of thread groups to use when placing the points
        /// </summary>
        private Vector3Int PointThreadGroupCount { get => new Vector3Int(
            Mathf.CeilToInt((float)CellCounts.x/threadGroupSize.x),
            Mathf.CeilToInt((float)CellCounts.y/threadGroupSize.y),
            1
        );}

        /// <summary>
        /// The kernel for normalizing the values of the final texture
        /// </summary>
        protected int NormalizeTextureKernel => NoiseGenShader.FindKernel("NormalizeTexture");
        /// <summary>
        /// The compute buffer for the positions of the control points
        /// </summary>
        private ComputeBuffer pointsBuffer;
        /// <summary>
        /// The compute buffer for the minimum and maximum values placed on the texture (used in normalization)
        /// </summary>
        private ComputeBuffer minMaxBuffer;

//? Texture Parameters
        /// <summary>
        /// The channel the noise is being stored in
        /// </summary>
        public TextureChannel ActiveChannel { get => activeChannel; set => activeChannel = value; }
        private TextureChannel activeChannel;

        /// <summary>
        /// converts the active channel into a vector4 with 1 in the active channel and 0 elsewhere
        /// </summary>
        private Vector4 channelMask {
            get => new Vector4(
                    ActiveChannel == TextureChannel.R ? 1 : 0, 
                    ActiveChannel == TextureChannel.G ? 1 : 0, 
                    ActiveChannel == TextureChannel.B ? 1 : 0, 
                    ActiveChannel == TextureChannel.A ? 1 : 0
                );
        }
        /// <summary>
        /// Converts the channel mask into a number to bit-shift the seed by  
        /// </summary>
        private int binChannelMask { get => (int)(channelMask.x * (2 ^ 0) + channelMask.y * (2 ^ 1) + channelMask.z * (2 ^ 2) + channelMask.w * (2 ^ 3)); }
        
        /// <summary>
        /// If the values of the texture should be inverted so the are brighter close to the control points and dark far
        /// </summary>
        public bool InvertTexture { get; set; }

        /// <summary>
        /// The number of cells along each axis to place a control point in
        /// </summary>
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


        private Vector2Int cellCounts;


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

        protected override void InnerGenerateTexture()
        {
            // SetShaderParameters();
            if(ActiveChannel == TextureChannel.All) {
                for (int i = 0; i < 4; i++)
                {
                    ActiveChannel = (TextureChannel)i;
                    SetShaderParameters();
                    NoiseGenShader.Dispatch(GeneratePointsKernel, PointThreadGroupCount.x, PointThreadGroupCount.y, PointThreadGroupCount.z);
                    NoiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
                    NoiseGenShader.Dispatch(NormalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
                }
                ActiveChannel = TextureChannel.All;
            } else {
                NoiseGenShader.Dispatch(GeneratePointsKernel, PointThreadGroupCount.x, PointThreadGroupCount.y, PointThreadGroupCount.z);
                NoiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
                NoiseGenShader.Dispatch(NormalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            }
            // pointsBuffer.Release();
            // minMaxBuffer.Release();
        }
        /// <summary>
        /// Generates a Worley noise texture using the given parameters
        /// </summary>
        /// <param name="_texWidth">the width of the texture</param>
        /// <param name="_texHeight">the height of the texture</param>
        /// <param name="_seed">the seed of the pseudo-random number generation</param>
        /// <param name="_cellCount">the number of cells to place a point in along each axis</param>
        /// <param name="_activeChannel">the channel to store the texture in, if TextureChannel.All each channel will store a different texture</param>
        /// <param name="_requireSeamlessTiling">if the texture should tile seamlessly</param>
        /// <param name="_invertTexture">if the values of the texture should be inverted</param>
        /// <returns></returns>
        public static RenderTexture GenerateTexture( 
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _cellCount,
            TextureChannel _activeChannel,
            bool _requireSeamlessTiling = false,
            bool _invertTexture = false
        ) {
            WorleyNoiseGenerator generator = new WorleyNoiseGenerator(_texWidth, _texHeight, _seed, _cellCount, _activeChannel);
            generator.RequireSeamlessTiling = _requireSeamlessTiling;
            generator.InvertTexture = _invertTexture;
            generator.GenerateTexture();
            RenderTexture output = generator.NoiseTexture;
            generator.Dispose();
            return output;
        }

        private bool disposedValue = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    pointsBuffer?.Release();
                    pointsBuffer = null;
                    minMaxBuffer?.Release();
                    minMaxBuffer = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}