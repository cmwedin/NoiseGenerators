using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators {
    //TODO
    public class FractalNoiseGenerator : AbstractNoiseGenerator
    {
//? Shader Parameters
        protected override string ComputeShaderPath => "Compute/NoiseFractalizer";
        private AbstractNoiseGenerator baseNoiseGenerator;
        protected int normalizeTextureKernel => NoiseGenShader.FindKernel("NormalizeTexture");
        private ComputeBuffer minMaxBuffer;

//? Texture Parameters
    //? override
        public override bool RequireSeamlessTiling {
            get => true;
            set { 
                if(!value) {
                    Debug.LogWarning("Fractal noise textures must tile seamlessly");
                    value = true;
                }
                baseNoiseGenerator.RequireSeamlessTiling = value; 
            }
        }

        //? fractal noise parameters
        private uint octaves;
        public uint Octaves { get => octaves; set => octaves = value; }

        private float lacunarity;
        public float Lacunarity { get => lacunarity; set => lacunarity = value; }

        private float frequency;
        public float Frequency { get => frequency; set => frequency = value; }

        private float gain;
        public float Gain { get => gain; set => gain = value; }

        private float amplitude;
        public float Amplitude { 
            get => amplitude; 
            set { 
                if(NormalizeAmplitude) {
                    Debug.LogWarning("The affect of the amplitude value will not be apparent unless NormalizeAmplitude is set to false, it is currently set to true");
                }
                amplitude = value; } }
        public bool NormalizeAmplitude { get; set; }


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

        protected override void SetShaderParameters()
        {
            base.SetShaderParameters();
            NoiseGenShader.SetInt("_Octaves", (int)octaves);
            NoiseGenShader.SetFloat("_Lacunarity", lacunarity);
            NoiseGenShader.SetFloat("_Frequency", frequency);
            NoiseGenShader.SetFloat("_Gain",gain);
            NoiseGenShader.SetFloat("_Amplitude", amplitude);
            NoiseGenShader.SetBool("_NormalizeAmplitude", NormalizeAmplitude);
            NoiseGenShader.SetBuffer(generateTextureKernel,"_MinMaxBuffer", minMaxBuffer);
            NoiseGenShader.SetTexture(generateTextureKernel, "_InNoiseTexture", baseNoiseGenerator.NoiseTexture);
            NoiseGenShader.SetTexture(generateTextureKernel, "_OutNoiseTexture", noiseTexture);
            NoiseGenShader.SetBuffer(normalizeTextureKernel,"_MinMaxBuffer", minMaxBuffer);
            NoiseGenShader.SetTexture(normalizeTextureKernel, "_OutNoiseTexture", noiseTexture);
        }

        public override void GenerateTexture()
        {
            baseNoiseGenerator.GenerateTexture();
            SetShaderParameters();
            NoiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            NoiseGenShader.Dispatch(normalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            minMaxBuffer.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            baseNoiseGenerator.Dispose();
            minMaxBuffer?.Dispose();
            minMaxBuffer = null;
            base.Dispose(disposing);
        }
    }
}