using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class PerlinNoiseGenerator : AbstractNoiseGenerator
    {
        protected override string ComputeShaderPath => "Compute/PerlinNoise";

        protected override int generateTextureKernel { get => NoiseGenShader.FindKernel("CSMain"); }
        [SerializeField] bool tileTexture;
        int generateLatticeKernel { get => NoiseGenShader.FindKernel("GenerateLattice"); }
        int wrapLatticeKernel { get => NoiseGenShader.FindKernel("WrapLattice"); }

        private Vector3Int latticeThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((latticeTexWidth / (float)threadGroupSize.x)),
                Mathf.CeilToInt(latticeTexHeight / (float)threadGroupSize.y),
                1
            );
        }

        private ComputeBuffer gradientBuffer;
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

        // Start is called before the first frame update
        void Start()
        {

        }
        protected override void CleanUpOldTextures()
        {
            base.CleanUpOldTextures();
        }

        protected override void SetShaderParameters() {
            base.SetShaderParameters();
            NoiseGenShader.SetInt("_LatticeSizeX", (int)LatticeSize.x);
            NoiseGenShader.SetInt("_LatticeSizeY", (int)LatticeSize.y);
            NoiseGenShader.SetBool("_TileTexture", tileTexture);
            NoiseGenShader.SetInt("_LatticeTexWidth", latticeTexWidth);
            NoiseGenShader.SetInt("_LatticeTexHeight", latticeTexHeight);
            NoiseGenShader.SetBuffer(generateLatticeKernel, "_GradientBuffer", gradientBuffer);
            NoiseGenShader.SetBuffer(generateTextureKernel, "_GradientBuffer", gradientBuffer);
            NoiseGenShader.SetBuffer(wrapLatticeKernel, "_GradientBuffer", gradientBuffer);
        }

        public override void GenerateTexture()
        {
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            gradientBuffer = new ComputeBuffer(latticeTexWidth * latticeTexHeight, 8 * sizeof(float));
            SetShaderParameters();
            NoiseGenShader.Dispatch(generateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            if(tileTexture) {
                NoiseGenShader.Dispatch(wrapLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            }
            NoiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            DisplayTexture();
            gradientBuffer?.Release();
        }
    }
}