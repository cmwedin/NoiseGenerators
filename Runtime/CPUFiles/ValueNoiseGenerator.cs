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
                Mathf.CeilToInt((float)latticeTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)latticeTexture.height / (float)threadGroupSize.x),
                1
            );
        }

        public RenderTexture latticeTexture;
        public uint latticeSize;
        private int latticeTexWidth { get => Mathf.CeilToInt((float)texWidth / (float)latticeSize)+1; }
        private int latticeTexHeight { get => Mathf.CeilToInt((float)texHeight / (float)latticeSize)+1; }

        // Start is called before the first frame update
        void Start()
        {
            GenerateTexture();
        }

        // Update is called once per frame
        void Update()
        {

        }
        protected override void CleanUpOldTextures()
        {
            base.CleanUpOldTextures();
            latticeTexture?.Release();
            DestroyImmediate(latticeTexture);
        }
        protected override void SetShaderParameters()
        {
            base.SetShaderParameters();
            noiseGenShader.SetInt("_LatticeSize", (int)latticeSize);
            noiseGenShader.SetInt("_LatticeTexWidth", latticeTexWidth);
            noiseGenShader.SetInt("_LatticeTexHeight", latticeTexHeight);
            noiseGenShader.SetTexture(generateLatticeKernel, "_LatticeTexture", latticeTexture);
            noiseGenShader.SetTexture(generateTextureKernel, "_LatticeTexture", latticeTexture);
        }

        public override void GenerateTexture()
        {
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            latticeTexture = new RenderTexture(latticeTexWidth, latticeTexHeight, 24);
            noiseTexture.enableRandomWrite = true;
            latticeTexture.enableRandomWrite = true;
            noiseTexture.Create();
            latticeTexture.Create();
            SetShaderParameters();
            noiseGenShader.Dispatch(generateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            noiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            displayMeshRenderer.sharedMaterial.mainTexture = noiseTexture;
        }
    }
}