using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class PerlinNoiseGenerator : AbstractNoiseGenerator
    {
        protected override int generateTextureKernel { get => noiseGenShader.FindKernel("CSMain"); }
        [SerializeField] bool tileTexture;
        int generateLatticeKernel { get => noiseGenShader.FindKernel("GenerateLattice"); }
        int wrapLatticeKernel { get => noiseGenShader.FindKernel("WrapLattice"); }

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
            noiseGenShader.SetInt("_LatticeSizeX", (int)LatticeSize.x);
            noiseGenShader.SetInt("_LatticeSizeY", (int)LatticeSize.y);
            noiseGenShader.SetBool("_TileTexture", tileTexture);
            noiseGenShader.SetInt("_LatticeTexWidth", latticeTexWidth);
            noiseGenShader.SetInt("_LatticeTexHeight", latticeTexHeight);
            noiseGenShader.SetBuffer(generateLatticeKernel, "_GradientBuffer", gradientBuffer);
            noiseGenShader.SetBuffer(generateTextureKernel, "_GradientBuffer", gradientBuffer);
            noiseGenShader.SetBuffer(wrapLatticeKernel, "_GradientBuffer", gradientBuffer);
        }

        public override void GenerateTexture()
        {
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            gradientBuffer = new ComputeBuffer(latticeTexWidth * latticeTexHeight, 8 * sizeof(float));
            SetShaderParameters();
            noiseGenShader.Dispatch(generateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            if(tileTexture) {
                noiseGenShader.Dispatch(wrapLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            }
            noiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            DisplayTexture();
            gradientBuffer?.Release();

        }
        protected override void DisplayTexture()
        {
            base.DisplayTexture();
            if (tileTexture)
            {
                displayMeshRenderer.sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
                displayMeshRenderer.sharedMaterial.mainTextureScale = new Vector2(2, 2);
            }
        }
    }
}