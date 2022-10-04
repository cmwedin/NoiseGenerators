using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class PerlinNoiseGenerator : AbstractNoiseGenerator
    {
        protected override int generateTextureKernel { get => noiseGenShader.FindKernel("CSMain"); }
        int generateLatticeKernel { get => noiseGenShader.FindKernel("GenerateLattice"); }
        private Vector3Int latticeThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)latticeTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)latticeTexture.height / (float)threadGroupSize.x),
                1
            );
        }

        public RenderTexture latticeTexture;
        [SerializeField] private RenderTexture gradientTextures;
        private ComputeBuffer gradientBuffer;
        public uint latticeSize;
        private int latticeTexWidth { get => Mathf.CeilToInt((float)texWidth / (float)latticeSize)+1; }
        private int latticeTexHeight { get => Mathf.CeilToInt((float)texHeight / (float)latticeSize)+1; }
        public bool useOptimizedCode;

        // Start is called before the first frame update
        void Start()
        {

        }
        protected override void CleanUpOldTextures()
        {
            base.CleanUpOldTextures();
            latticeTexture?.Release();
            DestroyImmediate(latticeTexture);
            gradientTextures?.Release();
            DestroyImmediate(gradientTextures);
            gradientBuffer?.Release();
        }

        protected override void SetShaderParameters() {
            base.SetShaderParameters();
            noiseGenShader.SetInt("_LatticeSize", (int)latticeSize);
            noiseGenShader.SetInt("_LatticeTexWidth", latticeTexWidth);
            noiseGenShader.SetInt("_LatticeTexHeight", latticeTexHeight);
            noiseGenShader.SetBool("_UseOptimizedCode",useOptimizedCode);
            noiseGenShader.SetTexture(generateLatticeKernel, "_LatticeTexture", latticeTexture);
            noiseGenShader.SetTexture(generateTextureKernel, "_LatticeTexture", latticeTexture);
            noiseGenShader.SetBuffer(generateLatticeKernel, "_GradientBuffer", gradientBuffer);
            noiseGenShader.SetBuffer(generateTextureKernel, "_GradientBuffer", gradientBuffer);
            noiseGenShader.SetTexture(generateLatticeKernel, "gradientTextures", gradientTextures);
            noiseGenShader.SetTexture(generateTextureKernel, "gradientTextures", gradientTextures);
        }
        public override void GenerateTexture()
        {
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            latticeTexture = new RenderTexture(latticeTexWidth, latticeTexHeight, 24);
            gradientTextures = new RenderTexture(latticeTexWidth,latticeTexHeight,24);
            gradientTextures.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
            gradientTextures.volumeDepth = 4;
            noiseTexture.enableRandomWrite = true;
            latticeTexture.enableRandomWrite = true;
            gradientTextures.enableRandomWrite = true;
            gradientBuffer = new ComputeBuffer(latticeTexWidth * latticeTexHeight, 8 * sizeof(float));
            noiseTexture.Create();
            latticeTexture.Create();
            gradientTextures.Create();
            SetShaderParameters();
            noiseGenShader.Dispatch(generateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            noiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            DisplayTexture();
        }
    }
}