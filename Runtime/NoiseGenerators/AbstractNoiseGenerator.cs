using UnityEngine;
using System;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// Base abstract class used for noise generators
    /// </summary>
    public abstract class AbstractNoiseGenerator : IDisposable
    {
        protected AbstractNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _seed
        ) {
            this.texWidth = _texWidth;
            this.texHeight = _texHeight;
            Seed = _seed;
            noiseTexture = new RenderTexture((int)TexWidth, (int)TexHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            noiseTexture.wrapMode = TextureWrapMode.Repeat;
            RegenerateTextureOnParamChange = false;
        }

//? compute shader fields
    //? Shader reference
        /// <summary>
        /// The compute shader used to generate the noise
        /// </summary>
        private ComputeShader noiseGenShader;
        /// <summary>
        /// If the compute shader reference has never been set for this object loads it from the resource folder
        /// </summary>
        protected ComputeShader NoiseGenShader { get {
            if(noiseGenShader == null) {
                noiseGenShader = Resources.Load<ComputeShader>(ComputeShaderPath);
            }
            return noiseGenShader;
        }}
        /// <summary>
        /// The path in the resource folder to the compute shader
        /// </summary>
        protected abstract string ComputeShaderPath { get; }

    //? Kernel and thread group info
        /// <summary>
        /// The kernel of the method to generate the texture in the compute shader
        /// </summary>
        protected int generateTextureKernel { get => NoiseGenShader.FindKernel("CSMain"); }
        /// <summary>
        /// The number of threads per group in the compute shader
        /// </summary>
        protected Vector3Int threadGroupSize = new Vector3Int(8, 8, 1);
        /// <summary>
        /// The number of thread groups used to generate the noise texture
        /// </summary>
        protected Vector3Int texThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)texWidth / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)texHeight / (float)threadGroupSize.x),
                1
            );
        }

//? texture parameters
        /// <summary>
        /// If GenerateTexture() should be invoked every time a parameter is changed, defaults to false
        /// </summary>
        public bool RegenerateTextureOnParamChange { get; set; }

        /// <summary>
        /// If the texture should be required to tile seamlessly, defaults to true
        /// </summary>
        public virtual bool RequireSeamlessTiling {
            get => requireSeamlessTiling;
            set {
                requireSeamlessTiling = value;
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            }
        }
        private bool requireSeamlessTiling = true;

        /// <summary>
        /// The pixel width of the texture
        /// </summary>
        public uint TexWidth { 
            get => texWidth; 
            set {
                if(value == 0) {value++;}
                texWidth = value;
                noiseTexture.Resize((int)TexWidth,(int)TexHeight);
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            }
        }
        private uint texWidth = 256;

        /// <summary>
        /// The pixel height of the texture
        /// </summary>
        public uint TexHeight { 
            get => texHeight; 
            set { 
                if(value == 0) {value++;}
                texHeight = value;
                noiseTexture.Resize((int)TexWidth,(int)TexHeight);
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            } 
        }
        private uint texHeight = 256;

        /// <summary>
        /// The seed for the pseudo-random number generator
        /// </summary>
        public uint Seed { 
            get => seed;
            set { 
                seed = value; 
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            }
        }
        private uint seed;

        /// <summary>
        /// The generated noise texture
        /// </summary>
        public RenderTexture NoiseTexture { get => noiseTexture; }
        protected RenderTexture noiseTexture;

//? Events
        public event Action OnTextureGeneration;

//? Methods
        /// <summary>
        /// Sets the parameters of the compute shader
        /// </summary>
        protected virtual void SetShaderParameters() {
            // Debug.Log("Setting Shader Parameters");
            NoiseGenShader.SetInt("_Seed", (int)Seed);
            NoiseGenShader.SetInt("_TexWidth", (int)TexWidth);
            NoiseGenShader.SetInt("_TexHeight", (int)TexHeight);
            NoiseGenShader.SetTexture(generateTextureKernel, "_NoiseTexture", noiseTexture);
        }

        /// <summary>
        /// The inner implementation of generated the noise texture
        /// </summary>
        protected abstract void InnerGenerateTexture();
        /// <summary>
        /// Generates the noise texture
        /// </summary>
        public void GenerateTexture() {
            SetShaderParameters();
            ResetNoiseTexture();
            InnerGenerateTexture();
            OnTextureGeneration?.Invoke();
        }

        private void ResetNoiseTexture()
        {
            NoiseTexture.DiscardContents();
            NoiseTexture.Release();
            NoiseTexture.Create();
        }


        //? IDisposable implementation
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // noiseTexture?.Release();
                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes the resources used to generate the texture, but not the texture itself. Remember to dispose of that through its Release() method when finished using it
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}