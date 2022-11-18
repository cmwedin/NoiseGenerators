using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        /// This field is being sunset and will be removed in the 1.1.0 update. Please transition to always using pre-generated input textures for fractal noise generators
        /// The generator that creates the input texture if not using a pre-generated one
        /// </summary>
        private AbstractNoiseGenerator baseNoiseGenerator;
        private bool usingBaseGenerator = false;
        protected int normalizeTextureKernel => NoiseGenShader.FindKernel("NormalizeTexture");
        /// <summary>
        /// The buffer for storing the minimum and maximum values generated by the compute shader
        /// </summary>
        private ComputeBuffer minMaxBuffer;
        /// <summary>
        /// If a pre-generated noise texture has been supplied in the constructor
        /// </summary>
        private bool inputsSet = false;

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
        public override uint TexHeight { get => base.TexHeight;
            set {
                RegenerateTextureOnParamChange = false;
                if (inputsSet) {
                    ReleaseInputRenderTextures();
                    inputTextures = null;
                    inputsSet = false;
                }
                base.TexHeight = value; 
            }
        }
        public override uint TexWidth { get => base.TexWidth;
            set {
                RegenerateTextureOnParamChange = false;
                if (inputsSet) {
                    ReleaseInputRenderTextures();
                    inputTextures = null;
                    inputsSet = false;
                }
                base.TexWidth = value;
            }
        }
        //? fractal noise parameters

        /// <summary>
        /// The Tex2DArray used by the compute shader for input
        /// </summary>
        public RenderTexture InputTextureArray { get
            {
                return inputTextureArray;
            }
            private set => inputTextureArray = value; 
        }
        private RenderTexture inputTextureArray;
        private void CreateInputArray() {
            if(disposedValue) {
                throw new System.ObjectDisposedException(this.ToString());
            }
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
                    // Graphics.CopyTexture(inputTextures[i], 0, 0, inputTextureArray, i, 0);
                    Graphics.Blit(inputTextures[i], inputTextureArray, 0, i);
                } else {
                    Graphics.Blit(inputTextures[^1], inputTextureArray, 0, i);

                }
            }
        }


        /// <summary>
        /// The input textures to use in the fractal noise generation process, if multiple textures are used any in excess of the number of octaves will be ignored
        /// </summary>
        public ReadOnlyCollection<Texture> InputTextures {
            get {
                if(!inputsSet) {
                    return null;
                }
                return inputTextures.AsReadOnly();
            } 
        }
        private void ReleaseInputRenderTextures() {
                var disposables =
                    from input in inputTextures
                    where input is RenderTexture
                    select input as RenderTexture;
                foreach (var tex in disposables) {
                    tex.Release();
                }
        }
        private List<Texture> inputTextures;

        /// <summary>
        /// Set an input texture for the FractalNoiseGenerator
        /// </summary>
        /// <param name="texture">the input texture to use</param>
        /// <param name="releaseOldTextures">If the previously used input textures should be released or not, do not set this to false unless you are not using render textures or know what you are doing</param>
        /// <exception cref="System.ObjectDisposedException">Throw if you attempt to set a disposed generator's noise textures</exception>
        public void SetInputTextures(Texture texture, bool releaseOldTextures = true){
            if(disposedValue) {
                throw new System.ObjectDisposedException(this.ToString());
            }
            if(inputsSet) {
                if(releaseOldTextures) {
                    ReleaseInputRenderTextures();
                }
                inputsSet = false;
            }
            AddInputTexture(texture);
        }

        /// <summary>
        /// Set the input textures for the FractalNoiseGenerator
        /// </summary>
        /// <param name="textures">the input textures to use</param>
        /// <param name="releaseOldTextures">If the previously used input textures should be released or not</param>
        /// <exception cref="System.ObjectDisposedException">Throw if you attempt to set a disposed generator's noise textures</exception>
        public void SetInputTextures(List<Texture> textures,bool releaseOldTextures = true){
            if(disposedValue) {
                throw new System.ObjectDisposedException(this.ToString());
            }
            if(inputsSet) {
                if(releaseOldTextures) {
                    ReleaseInputRenderTextures();
                }
                inputsSet = false;
            }

            foreach (var texture in textures) {
                AddInputTexture(texture);
            }
        }
        /// <summary>
        /// Adds a texture to the FractalNoiseGenerator's collection of inputs
        /// </summary>
        /// <param name="texture">The texture to add</param>
        /// <exception cref="System.ObjectDisposedException">Thrown if you attempt to add a texture to the inputs of a disposed fractal noise generator</exception>
        public void AddInputTexture(Texture texture) {
            if(disposedValue) {
                throw new System.ObjectDisposedException(this.ToString());
            } 
            if(!inputsSet) {
                inputTextures = new List<Texture>();
                inputsSet = true;
            }
            if(inputTextures.Contains(texture)) {
                Debug.Log("Inputs for this fractal noise generator already contain that texture");
            } else if (texture.width != TexWidth || texture.height != TexHeight) {
                Debug.Log("that texture does not match the texture size of this fractal noise generator");
            } else {
                inputTextures.Add(texture);
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
            usingBaseGenerator = true;
            Debug.LogWarning("Support for creating a fractal noise generator that doesn't use pre-generated input textures is being sunset and will be removed in version 1.1.0, please transition to using pre-generated textures");
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
        public FractalNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _octaves,
            float _lacunarity = 2,
            float _frequency = 1,
            float _gain = .5f,
            float _amplitude = .5f
        ) : base(_texWidth,_texHeight,1) {
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
            Texture _inputTexture,
            float _lacunarity = 2,
            float _frequency = 1,
            float _gain = .5f,
            float _amplitude = .5f
        ) : base((uint)_inputTexture.width, (uint)_inputTexture.height,0) {
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
        /// <summary>
        /// Constructs a fractal noise texture that uses a pre-generated input texture
        /// </summary>
        /// <param name="_octaves">The number of times detail will be added onto the final texture </param>
        /// <param name="_inputTextures">The input texture used to layer detail onto the final result</param>
        /// <param name="_lacunarity">The factor by which the frequency should increase with each octave</param>
        /// <param name="_frequency">The initial frequency in the first octave</param>
        /// <param name="_gain">The factor by which the amplitude should decrease each octave</param>
        /// <param name="_amplitude">The initial amplitude in the first octaves</param>
        public FractalNoiseGenerator(
            uint _octaves,
            List<Texture> _inputTextures,
            float _lacunarity = 2,
            float _frequency = 1,
            float _gain = .5f,
            float _amplitude = .5f
        ) : base((uint)_inputTextures[0].width, (uint)_inputTextures[0].height,0) {
            SetInputTextures(_inputTextures);
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

        protected override void SetShaderParameters()
        {
            base.SetShaderParameters();
            if(!inputsSet) {
                if(usingBaseGenerator) {
                    baseNoiseGenerator.GenerateTexture();
                    AddInputTexture(baseNoiseGenerator.NoiseTexture);
                } else {
                    throw new NoInputSetException();
                }
            }
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
                if (inputsSet)
                {
                    ReleaseInputRenderTextures();
                }
                base.Dispose(disposing);
                disposedValue = true;
            }
        }
    }
    [System.Serializable]
    public class NoInputSetException : System.Exception
    {
        public NoInputSetException() : base("Cannot generate a fractal noise texture without setting input textures") { }
        public NoInputSetException(string message) : base(message) { }
        public NoInputSetException(string message, System.Exception inner) : base(message, inner) { }
        protected NoInputSetException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}