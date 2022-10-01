using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class PerlinNoiseGenerator : MonoBehaviour
    {
        public ComputeShader perlinNoiseShader;
        private Vector3Int threadGroupSize = new Vector3Int(8, 8, 1);

        int GenerateTextureKernel { get => perlinNoiseShader.FindKernel("CSMain"); }
        private Vector3Int texThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)noiseTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)noiseTexture.height / (float)threadGroupSize.x),
                1
            );
        }
        int GenerateLatticeKernel { get => perlinNoiseShader.FindKernel("GenerateLattice"); }
        private Vector3Int latticeThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)latticeTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)latticeTexture.height / (float)threadGroupSize.x),
                1
            );
        }

        public RenderTexture noiseTexture;
        public RenderTexture latticeTexture;
        public MeshRenderer displayMeshRenderer;
        public uint seed;
        public uint texWidth;
        public uint texHeight;
        public uint latticeSize;
        private int latticeTexWidth { get => Mathf.CeilToInt((float)texWidth / (float)latticeSize)+1; }
        private int latticeTexHeight { get => Mathf.CeilToInt((float)texHeight / (float)latticeSize)+1; }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetShaderParameters() {
            perlinNoiseShader.SetInt("_Seed", (int)seed);
            perlinNoiseShader.SetInt("_TexWidth", (int)texWidth);
            perlinNoiseShader.SetInt("_TexHeight", (int)texHeight);
            perlinNoiseShader.SetInt("_LatticeSize", (int)latticeSize);
            perlinNoiseShader.SetInt("_LatticeTexWidth", latticeTexWidth);
            perlinNoiseShader.SetInt("_LatticeTexHeight", latticeTexHeight);
            perlinNoiseShader.SetTexture(GenerateTextureKernel, "_NoiseTexture", noiseTexture);
            perlinNoiseShader.SetTexture(GenerateLatticeKernel, "_LatticeTexture", latticeTexture);
            perlinNoiseShader.SetTexture(GenerateTextureKernel, "_LatticeTexture", latticeTexture);
        }
        public void GenerateTexture() {
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            latticeTexture = new RenderTexture(latticeTexWidth, latticeTexHeight, 24);
            noiseTexture.enableRandomWrite = true;
            latticeTexture.enableRandomWrite = true;
            noiseTexture.Create();
            latticeTexture.Create();
            SetShaderParameters();
            perlinNoiseShader.Dispatch(GenerateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            perlinNoiseShader.Dispatch(GenerateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            displayMeshRenderer.sharedMaterial.mainTexture = noiseTexture;
        }
    }
}