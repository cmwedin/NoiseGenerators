using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class ValueNoiseGenerator : MonoBehaviour
    {
        public ComputeShader valueNoiseShader;
        private Vector3Int threadGroupSize = new Vector3Int(8, 8, 1);

        int GenerateTextureKernel { get => valueNoiseShader.FindKernel("CSMain"); }
        private Vector3Int texThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)noiseTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)noiseTexture.height / (float)threadGroupSize.x),
                1
            );
        }
        int GenerateLatticeKernel { get => valueNoiseShader.FindKernel("GenerateLattice"); }
        private Vector3Int latticeThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)latticeTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)latticeTexture.height / (float)threadGroupSize.x),
                1
            );
        }

        public RenderTexture noiseTexture;
        private RenderTexture latticeTexture;
        public MeshRenderer displayMeshRenderer;
        public uint seed;
        public uint texWidth;
        public uint texHeight;
        public uint latticeSize;
        private int latticeTexWidth { get => Mathf.CeilToInt((float)texWidth / (float)latticeSize); }
        private int latticeTexHeight { get => Mathf.CeilToInt((float)texHeight / (float)latticeSize); }

        // Start is called before the first frame update
        void Start()
        {
            GenerateTexture();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetShaderParameters()
        {
            valueNoiseShader.SetInt("_Seed", (int)seed);
            valueNoiseShader.SetInt("_TexWidth", (int)texHeight);
            valueNoiseShader.SetInt("_TexHeight", (int)texHeight);
            valueNoiseShader.SetInt("_LatticeSize", (int)latticeSize);
            valueNoiseShader.SetInt("_LatticeTexWidth", latticeTexWidth);
            valueNoiseShader.SetInt("_LatticeTexHeight", latticeTexHeight);
            valueNoiseShader.SetTexture(GenerateTextureKernel, "_NoiseTexture", noiseTexture);
            valueNoiseShader.SetTexture(GenerateLatticeKernel, "_LatticeTexture", latticeTexture);
            valueNoiseShader.SetTexture(GenerateTextureKernel, "_LatticeTexture", latticeTexture);
        }

        public void GenerateTexture()
        {
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            latticeTexture = new RenderTexture(latticeTexWidth, latticeTexHeight, 24);
            noiseTexture.enableRandomWrite = true;
            latticeTexture.enableRandomWrite = true;
            noiseTexture.Create();
            latticeTexture.Create();
            SetShaderParameters();
            valueNoiseShader.Dispatch(GenerateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            valueNoiseShader.Dispatch(GenerateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            displayMeshRenderer.sharedMaterial.mainTexture = noiseTexture;
        }
    }
}