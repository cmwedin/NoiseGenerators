using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public abstract class AbstractNoiseGeneratorComponent : MonoBehaviour, IDisposable
    {
        // /// <summary>
        // /// The object that generates the noise texture
        // /// </summary>
        // protected abstract AbstractNoiseGenerator NoiseGeneratorObject { get; }
        /// <summary>
        /// This event will be invoked when a new texture is generated
        /// </summary>
        public event Action GeneratedTexture;
        /// <summary>
        /// Constructs the generator object and sets its parameters
        /// </summary>
        protected abstract AbstractNoiseGenerator CreateGeneratorObject();
        
        /// <summary>
        /// The noise texture created by the generator
        /// </summary>
        public RenderTexture NoiseTexture  { get {
                if( noiseTexture == null) {
                    GenerateTexture();
                }
                return noiseTexture;
            }
        }
        [SerializeField, Tooltip("The noise texture created by the generator")] RenderTexture noiseTexture;

        /// <summary>
        /// The seed that will be used in the pseudo-random number generation
        /// </summary>
        public uint Seed { get => seed; set => seed = value; }
        [SerializeField,Tooltip("The seed that will be used in the pseudo-random number generation")] protected uint seed;

        /// <summary>
        /// The Width of the generated texture
        /// </summary>
        public uint TexWidth { get => texWidth; set {
                if (value == 0) { value++; }
                texWidth = value;
        }}
        [SerializeField,Tooltip("The Width of the generated texture")] protected uint texWidth;

        /// <summary>
        /// The height of the generated texture
        /// </summary>
        public uint TexHeight { get => texHeight; set {
                if (value == 0) { value++; }
                texHeight = value;
        }}
        [SerializeField, Tooltip("The height of the generated texture")] protected uint texHeight;

        protected bool disposedValue;

        /// <summary>
        /// Generates the noise texture
        /// </summary>
        public void GenerateTexture() {
            if(noiseTexture != null) {
                noiseTexture.Release();
                noiseTexture = null;
            }
            using var generator = CreateGeneratorObject();

            generator.GenerateTexture();
            noiseTexture = generator.NoiseTexture;
            GeneratedTexture?.Invoke();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // Debug.Log("Disposing noise generator");
                if (disposing)
                {
                    // NoiseGeneratorObject.Dispose();
                }
                noiseTexture?.Release();
                noiseTexture = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~AbstractNoiseGeneratorComponent()
        {
            Dispose(disposing: false);
        }
// 
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