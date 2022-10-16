using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public abstract class AbstractNoiseGeneratorComponent : MonoBehaviour, IDisposable
    {
        protected abstract AbstractNoiseGenerator NoiseGeneratorObject { get; }
        protected abstract void CreateGeneratorObject();
        public RenderTexture NoiseTexture => NoiseGeneratorObject.NoiseTexture;
        [SerializeField] protected MeshRenderer displayMeshRenderer;
        [SerializeField] protected uint seed;
        [SerializeField] protected uint texWidth;
        public uint TexWidth { get => texWidth; }
        [SerializeField] protected uint texHeight;
        public uint TexHeight { get => texHeight; }

        [SerializeField,Tooltip("increments the seed and regenerates the texture every frame to test generation speed and memory uses")] protected bool cycleTextureSeed;
        protected bool disposedValue;

        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            if(cycleTextureSeed) {
                seed++;
                GenerateTexture();
            }
        }
        protected virtual void OnValidate() {
            NoiseGeneratorObject.TexHeight = TexHeight;
            NoiseGeneratorObject.TexWidth = TexWidth;
            NoiseGeneratorObject.Seed = seed;
        }

        protected virtual void DisplayTexture() {
            if (displayMeshRenderer != null) {
                displayMeshRenderer.sharedMaterial.mainTexture = NoiseTexture;
            }
        }

        public void GenerateTexture() {
            // Debug.Log("Generating texure");
            NoiseGeneratorObject.GenerateTexture();
            // Debug.Log("Texture generation complete");
            DisplayTexture();
        }

        private void OnDestroy() {
            this.Dispose();
        }
        private void OnDisable() {
            this.Dispose();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                NoiseGeneratorObject.Dispose();
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~AbstractNoiseGeneratorComponent()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}