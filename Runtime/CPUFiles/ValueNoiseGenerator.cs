using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class ValueNoiseGenerator : AbstractNoiseGenerator {
        protected override int generateTextureKernel { get => noiseGenShader.FindKernel("CSMain"); }
        int generateLatticeKernel { get => noiseGenShader.FindKernel("GenerateLattice"); }
        private Vector3Int latticeThreadGroupCount {
            get => new Vector3Int(
                Mathf.CeilToInt((float)latticeTexWidth / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)latticeTexHeight / (float)threadGroupSize.y),
                1
            );
        }

        private ComputeBuffer latticeBuffer;
        public uint latticeSize;
        private int latticeTexWidth { get => Mathf.CeilToInt((float)texWidth / (float)latticeSize)+1; }
        private int latticeTexHeight { get => Mathf.CeilToInt((float)texHeight / (float)latticeSize)+1; }

        protected override void CleanUpOldTextures()
        {
            base.CleanUpOldTextures();
            latticeBuffer?.Release();
        }
        protected override void SetShaderParameters()
        {
            base.SetShaderParameters();
            noiseGenShader.SetInt("_LatticeSize", (int)latticeSize);
            noiseGenShader.SetInt("_LatticeTexWidth", latticeTexWidth);
            noiseGenShader.SetInt("_LatticeTexHeight", latticeTexHeight);
            noiseGenShader.SetBuffer(generateLatticeKernel, "_LatticeBuffer", latticeBuffer);
            noiseGenShader.SetBuffer(generateTextureKernel, "_LatticeBuffer", latticeBuffer);
        }

        public override void GenerateTexture()
        {
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            latticeBuffer = new ComputeBuffer(latticeTexWidth * latticeTexHeight, 4 * sizeof(float));
            SetShaderParameters();
            noiseGenShader.Dispatch(generateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            noiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            DisplayTexture();
        }
    }
}