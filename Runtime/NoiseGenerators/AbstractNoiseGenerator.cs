using UnityEngine;
using System;

namespace SadSapphicGames.NoiseGenerators
{
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
        private ComputeShader noiseGenShader;
        protected ComputeShader NoiseGenShader { get {
            if(noiseGenShader == null) {
                noiseGenShader = Resources.Load<ComputeShader>(ComputeShaderPath);
            }
            return noiseGenShader;
        }}
        protected abstract string ComputeShaderPath { get; }

    //? Kernel and thread group info
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
        private bool requireSeamlessTiling = true;
        public bool RequireSeamlessTiling {
            get => requireSeamlessTiling;
            set {
                requireSeamlessTiling = value;
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            }

        }
        private uint texWidth = 256;
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
        private uint texHeight = 256;
        public uint TexHeight { 
            get => texHeight; 
            set { 
                if(value == 0) {value++;}
                texHeight = value;
                noiseTexture.Resize((int)TexWidth,(int)TexHeight);
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
        public RenderTexture NoiseTexture { get => noiseTexture; }

        protected virtual void SetShaderParameters() {
            // Debug.Log("Setting Shader Parameters");
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

                // noiseTexture?.Release();
                // noiseTexture = null;
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