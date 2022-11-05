using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// The abstract bass class of all MonoBehaviour components wrapping an AbstractNoiseGenerator
    /// </summary>
    [ExecuteInEditMode]
    public abstract class AbstractNoiseGeneratorComponent : MonoBehaviour, IDisposable
    {
        private AbstractNoiseGenerator noiseGeneratorObject;
        /// <summary>
        /// The object that generates the noise texture
        /// </summary>
        protected AbstractNoiseGenerator NoiseGeneratorObject { get {
            if(noiseGeneratorObject == null) {
                noiseGeneratorObject = CreateGeneratorObject();
                    disposedValue = false;
                }    
            return noiseGeneratorObject;
        } }

        /// <summary>
        /// This event will be invoked when a new texture is generated
        /// </summary>
        public event Action OnTextureGeneration;
        /// <summary>
        /// A boolean to indicate if the texture has been generated yet
        /// </summary>
        public bool TextureGenerated { get; private set; } = false;
        /// <summary>
        /// Constructs the generator object and sets its parameters
        /// </summary>
        protected abstract AbstractNoiseGenerator CreateGeneratorObject();
        
        /// <summary>
        /// The noise texture created by the generator
        /// </summary>
        public RenderTexture NoiseTexture  { get {
                if (!TextureGenerated) { return null; }
                return NoiseGeneratorObject.NoiseTexture; 
            } 
        }

        //? removed as of v1.0.1
        // /// <summary>
        // /// Redundant field so the noise texture can be examined in the inspector, Unity does not serialize properties
        // /// </summary>
        // [SerializeField, Tooltip("The noise texture created by the generator")] RenderTexture noiseTexture;

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

        protected virtual void UpdateGeneratorSettings() {
            //? If the values of the component are different than the values of the object, update the objects values
            if(NoiseGeneratorObject.TexHeight != TexHeight) {
                NoiseGeneratorObject.TexHeight = TexHeight;
                //? Capture any changes that where made in validation
                TexHeight = NoiseGeneratorObject.TexHeight;
            }
            if(NoiseGeneratorObject.TexWidth != TexWidth) {
                NoiseGeneratorObject.TexWidth = TexWidth;
                TexWidth = NoiseGeneratorObject.TexWidth;
            }
            if (NoiseGeneratorObject.Seed != Seed) {
                NoiseGeneratorObject.Seed = Seed;
                
                //? Unneeded because the seed value does not require validation
                // //Seed = NoiseGeneratorObject.Seed;
            }
        }

        /// <summary>
        /// Generates the noise texture
        /// </summary>
        public void GenerateTexture() {
            UpdateGeneratorSettings();
            NoiseGeneratorObject.GenerateTexture();
            TextureGenerated = true;
            OnTextureGeneration?.Invoke();
        }

        private void OnDisable() {
            this.Dispose();
        }

        private bool disposedValue = true;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // Debug.Log($"Disposing noise generator {this.name}");
                disposedValue = true;
                if (disposing)
                {
                    noiseGeneratorObject?.Dispose();
                    noiseGeneratorObject = null;
                }
                TextureGenerated = false;
            }
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