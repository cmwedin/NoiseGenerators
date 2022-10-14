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
        [SerializeField] private bool tileTexture;
        private ComputeBuffer latticeBuffer;
        [SerializeField] private Vector2Int latticeSize;
        public Vector2Int LatticeSize {
            get {
                if(TexWidth % latticeSize.x != 0) {
                    Debug.LogWarning("Horizontal lattice size not set to factor of texture width, adjusting value to closest factor");
                    latticeSize = new Vector2Int(
                        HelperMethods.FindClosestFactor((int)TexWidth,latticeSize.x),
                        latticeSize.y
                    );
                } if (TexHeight % latticeSize.y != 0) {
                    Debug.LogWarning("Vertical lattice size not set to factor of texture height, adjusting value to closest factor");
                    latticeSize = new Vector2Int(
                        latticeSize.x,
                        HelperMethods.FindClosestFactor((int)TexHeight,latticeSize.y)
                    );
                }
                return latticeSize; 
            }
        }
        private int latticeTexWidth { get => Mathf.CeilToInt((float)texWidth / (float)LatticeSize.x)+1; }
        private int latticeTexHeight { get => Mathf.CeilToInt((float)texHeight / (float)LatticeSize.y)+1; }

        protected override void CleanUpOldTextures()
        {
            base.CleanUpOldTextures();
            latticeBuffer?.Release();
        }
        protected override void SetShaderParameters()
        {
            base.SetShaderParameters();
            noiseGenShader.SetInt("_LatticeSizeX", (int)LatticeSize.x);
            noiseGenShader.SetInt("_LatticeSizeY", (int)LatticeSize.y);
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