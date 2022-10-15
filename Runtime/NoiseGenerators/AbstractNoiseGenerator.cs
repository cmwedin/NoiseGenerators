using UnityEngine;
using System;

namespace SadSapphicGames.NoiseGenerators
{
    public abstract class AbstractNoiseGenerator : IDisposable
    {

        protected AbstractNoiseGenerator(
            uint texWidth,
            uint texHeight,
            uint seed
        ) {
            this.texWidth = texWidth;
            this.texHeight = texHeight;
            Seed = seed;
            noiseTexture = new RenderTexture((int)TexWidth, (int)TexHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
        }

//? compute shader fields
        private ComputeShader noiseGenShader;
        protected ComputeShader NoiseGenShader { get {
            if(noiseGenShader == null) {
                noiseGenShader = Resources.Load<ComputeShader>(ComputeShaderPath);
            }
            return noiseGenShader;
        }}
        protected abstract string ComputeShaderPath { get; }

        protected int generateTextureKernel { get => NoiseGenShader.FindKernel("CSMain"); }
        protected Vector3Int threadGroupSize = new Vector3Int(8, 8, 1);
        protected Vector3Int texThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)texWidth / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)texHeight / (float)threadGroupSize.x),
                1
            );
        }

//? texture parameters
        public bool RegenerateTextureOnParamChange { get; set; }
        private uint texWidth = 256;
        public uint TexWidth { 
            get => texWidth; 
            set { 
                texWidth = value;
                noiseTexture.width = (int)texWidth;
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            }
        }
        private uint texHeight = 256;
        public uint TexHeight { 
            get => texHeight; 
            set { 
                texHeight = value;
                noiseTexture.height = (int)texHeight;
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            } }
        private uint seed;
        public uint Seed { 
            get => seed;
            set { 
                seed = value; 
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            }
        }
        protected RenderTexture noiseTexture;

        protected virtual void SetShaderParameters() {
            NoiseGenShader.SetInt("_Seed", (int)Seed);
            NoiseGenShader.SetInt("_TexWidth", (int)TexWidth);
            NoiseGenShader.SetInt("_TexHeight", (int)TexHeight);
            NoiseGenShader.SetTexture(generateTextureKernel, "_NoiseTexture", noiseTexture);
        }
        public abstract void GenerateTexture();


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

                noiseTexture?.Release();
                noiseTexture = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~AbstractNoiseGenerator()
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