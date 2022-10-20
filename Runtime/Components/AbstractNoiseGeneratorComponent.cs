using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public abstract class AbstractNoiseGeneratorComponent : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// The object that generates the noise texture
        /// </summary>
        protected abstract AbstractNoiseGenerator NoiseGeneratorObject { get; }
        /// <summary>
        /// Constructs the generator object and sets its parameters
        /// </summary>
        protected abstract void CreateGeneratorObject();
        /// <summary>
        /// The noise texture created by the generator
        /// </summary>
        public RenderTexture NoiseTexture => noiseTexture;
        [SerializeField, Tooltip("The noise texture created by the generator")] RenderTexture noiseTexture;
        /// <summary>
        /// The MeshRenderer the texture will optionally be displayed on
        /// </summary>
        [SerializeField,Tooltip("The MeshRenderer the texture will optionally be displayed on (can be left empty)")] protected MeshRenderer displayMeshRenderer;
        /// <summary>
        /// The seed that will be used in the pseudo-random number generation
        /// </summary>
        [SerializeField,Tooltip("The seed that will be used in the pseudo-random number generation")] protected uint seed;
        /// <summary>
        /// The Width of the generated texture
        /// </summary>
        public uint TexWidth { get => texWidth; }
        [SerializeField,Tooltip("The Width of the generated texture")] protected uint texWidth;
        /// <summary>
        /// The height of the generated texture
        /// </summary>
        public uint TexHeight { get => texHeight; }
        [SerializeField, Tooltip("The height of the generated texture")] protected uint texHeight;

        // [SerializeField,Tooltip("increments the seed and regenerates the texture every frame to test generation speed and memory uses")] protected bool cycleTextureSeed;
        protected bool disposedValue;

        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            // if(cycleTextureSeed) {
            //     seed++;
            //     GenerateTexture();
            // }
        }
        // protected virtual void OnValidate() {
        //     NoiseGeneratorObject.TexHeight = TexHeight;
        //     NoiseGeneratorObject.TexWidth = TexWidth;
        //     NoiseGeneratorObject.Seed = seed;
        // }

        /// <summary>
        /// Displays the generated texture on the displayMeshRenderer if it is set
        /// </summary>
        protected virtual void DisplayTexture() {
            if (displayMeshRenderer != null) {
                displayMeshRenderer.sharedMaterial.mainTexture = NoiseTexture;
            }
        }

        /// <summary>
        /// Generates the noise texture
        /// </summary>
        public void GenerateTexture() {
            noiseTexture?.Release();
            CreateGeneratorObject();
            // Debug.Log("Generating texure");
            NoiseGeneratorObject.GenerateTexture();
            noiseTexture = NoiseGeneratorObject.NoiseTexture;
            // Debug.Log("Texture generation complete");
            DisplayTexture();
            NoiseGeneratorObject.Dispose();
        }

        // private void OnDestroy() {
        //     this.Dispose();
        // }
        // protected virtual void OnDisable() {
        //     this.Dispose();
        // }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // Debug.Log("Disposing noise generator");
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                NoiseGeneratorObject.Dispose();
                noiseTexture?.Release();
                noiseTexture = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~AbstractNoiseGeneratorComponent()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Disposes the NoiseGeneratorObject used to create the noise texture and the texture itself. Do not invoke this until done using the texture
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}