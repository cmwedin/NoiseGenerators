using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators {
    /// <summary>
    /// A generator object for adding detail to an existing noise texture (or the texture generated by a AbstractNoiseGenerator instance) using fractal brownian motion
    /// </summary>

    public class FractalNoiseGenerator : AbstractNoiseGenerator
    {
//? Shader Parameters
        protected override string ComputeShaderPath => "Compute/NoiseFractalizer";
        /// <summary>
        /// The generator that creates the input texture if not using a pre-generated one
        /// </summary>
        private AbstractNoiseGenerator baseNoiseGenerator;
        protected int normalizeTextureKernel => NoiseGenShader.FindKernel("NormalizeTexture");
        /// <summary>
        /// The buffer for storing the minimum and maximum values generated by the compute shader
        /// </summary>
        private ComputeBuffer minMaxBuffer;
        /// <summary>
        /// If a pre-generated noise texture has been supplied in the constructor
        /// </summary>
        private bool usePreGeneratedTexture = false;

        //? Texture Parameters
        //? override
        public override bool RequireSeamlessTiling {
            get => true;
            set { 
                if(!value) {
                    Debug.LogWarning("Fractal noise textures must tile seamlessly");
                    value = true;
                }
                base.RequireSeamlessTiling = value; 
            }
        }

        //? fractal noise parameters
        /// <summary>
        /// The input texture to use in the fractal noise generation process
        /// </summary>
        private List<RenderTexture> inputTextures;
        private RenderTexture inputTextureArray;
        public RenderTexture InputTextureArray { get
            {
                if (inputTextureArray == null) {
                    CreateInputArray();
                }
                return inputTextureArray;
            }
            private set => inputTextureArray = value; }

        public ReadOnlyCollection<RenderTexture> InputTextures {
            get {
                if(inputTextures == null || inputTextures.Count == 0) {
                    if (!usePreGeneratedTexture)
                    {
                        baseNoiseGenerator.GenerateTexture();
                        SetInputTextures(baseNoiseGenerator.NoiseTexture);
                    } else {
                        throw new MissingReferenceException("This generator is supposed to be using a pre-generated texture however its input texture is null");
                    }
                }
                return inputTextures.AsReadOnly();
            } 
        }
        /// <summary>
        /// The number of times to layer detail from the input texture onto the final result
        /// </summary>
        public uint Octaves { get => octaves; set => octaves = value; }
        private uint octaves;

        /// <summary>
        /// The factor by which the frequency should increase with each octave
        /// </summary>
        private float lacunarity;
        public float Lacunarity { get => lacunarity; set => lacunarity = value; }
        /// <summary>
        /// The initial frequency in the first octave
        /// </summary>
        private float frequency;
        public float Frequency { get => frequency; set => frequency = value; }
        /// <summary>
        /// The factor by which the amplitude should decrease each octave
        /// </summary>
        private float gain;
        public float Gain { get => gain; set => gain = value; }
        /// <summary>
        /// The initial amplitude in the first octaves, changing this has no affect is normalizeAmplitude is true
        /// </summary>
        private float amplitude;
        public float Amplitude { 
            get => amplitude; 
            set { 
                if(NormalizeAmplitude) {
                    Debug.LogWarning("The affect of the amplitude value will not be apparent unless NormalizeAmplitude is set to false, it is currently set to true");
                }
                amplitude = value; } }
        /// <summary>
        /// If the affect of the initial amplitude should be normalized out of the final value
        /// </summary>
        public bool NormalizeAmplitude { get; set; }

        /// <summary>
        /// Constructs a fractal noise generator using a separate noise generator object  
        /// </summary>
        /// <param name="_octaves">The number of times detail will be added onto the final texture </param>
        /// <param name="_baseNoiseGenerator">The noise generator that creates the input texture</param>
        /// <param name="_lacunarity">The factor by which the frequency should increase with each octave</param>
        /// <param name="_frequency">The initial frequency in the first octave</param>
        /// <param name="_gain">The factor by which the amplitude should decrease each octave</param>
        /// <param name="_amplitude">The initial amplitude in the first octaves</param>
        public FractalNoiseGenerator(
            uint _octaves,
            AbstractNoiseGenerator _baseNoiseGenerator,
            float _lacunarity = 2,
            float _frequency = 1,
            float _gain = .5f,
            float _amplitude = .5f
        ) : base(_baseNoiseGenerator.TexWidth,_baseNoiseGenerator.TexHeight,_baseNoiseGenerator.Seed) {
            baseNoiseGenerator = _baseNoiseGenerator;
            RequireSeamlessTiling = true;
            minMaxBuffer = new ComputeBuffer(8, sizeof(uint));
            minMaxBuffer.SetData(new int[] { int.MaxValue, int.MaxValue,int.MaxValue,int.MaxValue,0,0,0,0 });
            Octaves = _octaves;
            Lacunarity = _lacunarity;
            Frequency = _frequency;
            Gain = _gain;
            if(_amplitude != .5f) { //? if they want to use a non-standard amplitude set NormalizeAmplitude to false so its affect is apparent;
                amplitude = _amplitude;
            } else {
                NormalizeAmplitude = false;
                Amplitude = _amplitude;
            }
        }
        /// <summary>
        /// Constructs a fractal noise texture that uses a pre-generated input texture
        /// </summary>
        /// <param name="_octaves">The number of times detail will be added onto the final texture </param>
        /// <param name="_inputTexture">The input texture used to layer detail onto the final result</param>
        /// <param name="_lacunarity">The factor by which the frequency should increase with each octave</param>
        /// <param name="_frequency">The initial frequency in the first octave</param>
        /// <param name="_gain">The factor by which the amplitude should decrease each octave</param>
        /// <param name="_amplitude">The initial amplitude in the first octaves</param>
        public FractalNoiseGenerator(
            uint _octaves,
            RenderTexture _inputTexture,
            float _lacunarity = 2,
            float _frequency = 1,
            float _gain = .5f,
            float _amplitude = .5f
        ) : base((uint)_inputTexture.width, (uint)_inputTexture.height,0) {
            usePreGeneratedTexture = true;
            SetInputTextures(_inputTexture);
            RequireSeamlessTiling = true;
            minMaxBuffer = new ComputeBuffer(8, sizeof(uint));
            minMaxBuffer.SetData(new int[] { int.MaxValue, int.MaxValue,int.MaxValue,int.MaxValue,0,0,0,0 });
            Octaves = _octaves;
            Lacunarity = _lacunarity;
            Frequency = _frequency;
            Gain = _gain;
            if(_amplitude != .5f) { //? if they want to use a non-standard amplitude set NormalizeAmplitude to false so its affect is apparent;
                amplitude = _amplitude;
            } else {
                NormalizeAmplitude = false;
                Amplitude = _amplitude;
            }
        }
        private void CreateInputArray() {
            inputTextureArray?.Release();
            inputTextureArray = null;
            inputTextureArray = new RenderTexture((int)TexWidth, (int)TexHeight,24);
            inputTextureArray.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
            inputTextureArray.volumeDepth = (int)Octaves;
            inputTextureArray.enableRandomWrite = true;
            inputTextureArray.Create();
            for (int i = 0; i < Octaves; i++)
            {
                if (i < inputTextures.Count) {
                    Graphics.CopyTexture(inputTextures[i], 0, 0, inputTextureArray, i, 0);
                } else {
                    Graphics.CopyTexture(inputTextures[^1], 0, 0, inputTextureArray, i, 0);
                }
            }
        }
        public void SetInputTextures(RenderTexture texture,bool disposeOldTextures = true){
            if(disposeOldTextures && inputTextures != null) {
                foreach (var renderTex in inputTextures) {
                    if (renderTex != texture)
                    {
                        renderTex.Release();
                    }
                }
            }
            inputTextures = new List<RenderTexture> { texture };
        }
        public void SetInputTextures(List<RenderTexture> textures,bool disposeOldTextures = true){
            if(disposeOldTextures && inputTextures != null) {
                foreach (var renderTex in inputTextures) {
                    if (!textures.Contains(renderTex))
                    {
                        renderTex.Release();
                    }
                }
            }
            inputTextures = textures;
        }

        protected override void SetShaderParameters()
        {
            base.SetShaderParameters();
            NoiseGenShader.SetInt("_Octaves", (int)octaves);
            NoiseGenShader.SetFloat("_Lacunarity", lacunarity);
            NoiseGenShader.SetFloat("_Frequency", frequency);
            NoiseGenShader.SetFloat("_Gain",gain);
            NoiseGenShader.SetFloat("_Amplitude", amplitude);
            NoiseGenShader.SetBool("_NormalizeAmplitude", NormalizeAmplitude);
            NoiseGenShader.SetBuffer(GenerateTextureKernel,"_MinMaxBuffer", minMaxBuffer);
            CreateInputArray();
            NoiseGenShader.SetTexture(GenerateTextureKernel, "_InNoiseTextureArray", InputTextureArray);
            NoiseGenShader.SetTexture(GenerateTextureKernel, "_OutNoiseTexture", noiseTexture);
            NoiseGenShader.SetBuffer(normalizeTextureKernel,"_MinMaxBuffer", minMaxBuffer);
            NoiseGenShader.SetTexture(normalizeTextureKernel, "_OutNoiseTexture", noiseTexture);
        }

        protected override void InnerGenerateTexture()
        {
            minMaxBuffer.SetData(new int[] { int.MaxValue, int.MaxValue,int.MaxValue,int.MaxValue,0,0,0,0 });
            NoiseGenShader.Dispatch(GenerateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            NoiseGenShader.Dispatch(normalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
        }
        /// <summary>
        /// Generates a fractal noise texture using the given input texture and parameters
        /// </summary>
        /// <param name="_octaves">The number of times detail will be added onto the final texture</param>
        /// <param name="_inputTexture">The input texture used to layer detail onto the final result</param>
        /// <param name="_lacunarity">The factor by which the frequency should increase with each octave</param>
        /// <param name="_frequency">The initial frequency in the first octave</param>
        /// <param name="_gain">The factor by which the amplitude should decrease each octave</param>
        /// <param name="_amplitude">The initial amplitude in the first octaves</param>
        /// <returns>The generated noise texture</returns>
        public static RenderTexture GenerateTexture(
            uint _octaves,
            RenderTexture _inputTexture,
            float _lacunarity = 2,
            float _frequency = 1,
            float _gain = 0.5f,
            float _amplitude = 0.5f
        ) {
            using FractalNoiseGenerator generator = new FractalNoiseGenerator(_octaves, _inputTexture, _lacunarity, _frequency, _gain, _amplitude);
            generator.GenerateTexture();
            RenderTexture output = generator.NoiseTexture.Copy();
            generator.Dispose(); //?should be redundant
            return output;
        }

        bool disposedValue = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    minMaxBuffer.Dispose();
                    minMaxBuffer = null;
                    baseNoiseGenerator?.Dispose();
                }
                inputTextureArray?.Release();
                base.Dispose(disposing);
                disposedValue = true;
            }
        }
    }
}