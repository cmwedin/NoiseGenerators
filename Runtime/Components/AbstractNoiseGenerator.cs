using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public abstract class AbstractNoiseGenerator : MonoBehaviour
    {
        private ComputeShader noiseGenShader;
        protected ComputeShader NoiseGenShader { get {
            if(noiseGenShader == null) {
                noiseGenShader = Resources.Load<ComputeShader>(ComputeShaderPath);
            }
            return noiseGenShader;
        }}
        protected abstract string ComputeShaderPath { get; }

        protected abstract int generateTextureKernel { get; }
        protected Vector3Int threadGroupSize = new Vector3Int(8, 8, 1);
        protected Vector3Int texThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)noiseTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)noiseTexture.height / (float)threadGroupSize.x),
                1
            );
        }
        [SerializeField] protected RenderTexture noiseTexture;
        public RenderTexture GetNoiseTexture() => noiseTexture;
        [SerializeField] protected MeshRenderer displayMeshRenderer;
        // [SerializeField] protected bool animate;
        [SerializeField] protected uint seed;
        [SerializeField] protected uint texWidth;
        public uint TexWidth { get => texWidth; }
        [SerializeField] protected uint texHeight;
        public uint TexHeight { get => texHeight; }

        [SerializeField,Tooltip("increments the seed and regenerates the texture every frame to test generation speed and memory uses")] protected bool cycleTextureSeed;
        // Start is called before the first frame update
        void Start()
        {
            GenerateTexture();
        }

        // Update is called once per frame
        void Update()
        {
            if(cycleTextureSeed) {
                seed++;
                GenerateTexture();
            }
        }
        protected virtual void SetShaderParameters() {
            NoiseGenShader.SetInt("_Seed", (int)seed);
            NoiseGenShader.SetInt("_TexWidth", (int)texWidth);
            NoiseGenShader.SetInt("_TexHeight", (int)texHeight);
            NoiseGenShader.SetTexture(generateTextureKernel, "_NoiseTexture", noiseTexture);
        }
        protected virtual void DisplayTexture() {
            if (displayMeshRenderer != null) {
                displayMeshRenderer.sharedMaterial.mainTexture = noiseTexture;
            }
        }
        protected virtual void CleanUpOldTextures() {
            // noiseTexture?.Release();
            if (noiseTexture != null) {
                noiseTexture.Release();
                DestroyImmediate(noiseTexture); 
            }
        }
        public abstract void GenerateTexture();
        private void OnDestroy() {
            CleanUpOldTextures();
        }
    }
}