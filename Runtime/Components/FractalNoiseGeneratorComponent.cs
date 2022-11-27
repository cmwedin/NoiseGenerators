using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// A MonoBehaviour component wrapping a FractalNoiseGenerator object
    /// </summary>
    public class FractalNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        /// <summary>
        /// The base noise generator for creating the input texture to the fractal noise generator
        /// </summary>
        [SerializeField] private AbstractNoiseGeneratorComponent baseNoiseGenerator;

        /// <summary>
        /// The number of times detail should be added onto the noise texture
        /// </summary>
        public uint Octaves { get => octaves; set => octaves = value; }

        [SerializeField, Tooltip("The number of times detail should be added onto the noise texture")] private uint octaves;
        /// <summary>
        /// The factor by which the frequency should increase with each octave
        /// </summary>
        public float Lacunarity { get => lacunarity; set => lacunarity = value; }

        [SerializeField, Tooltip("The factor by which the frequency should increase with each octave")] private float lacunarity = 2;
        /// <summary>
        /// The initial frequency in the first octave
        /// </summary>
        public float Frequency { get => frequency; set => frequency = value; }

        [SerializeField, Tooltip("The initial frequency in the first octave")] private float frequency = 2;

        /// <summary>
        /// The factor by which the amplitude should decrease each octave
        /// </summary>
        public float Gain { get => gain; set => gain = value; }

        [SerializeField, Tooltip("The factor by which the amplitude should decrease each octave")] private float gain = .5f;
        /// <summary>
        /// The initial amplitude in the first octaves, changing this has no affect is normalizeAmplitude is true
        /// </summary>
        public float Amplitude { get => amplitude; set => amplitude = value; }

        [SerializeField] private float amplitude = .5f;
        /// <summary>
        /// If the affect of the initial amplitude should be normalized out of the final value
        /// </summary>
        public bool NormalizeAmplitude { get => normalizeAmplitude; set => normalizeAmplitude = value; }

        [SerializeField, Tooltip("If the affect of the initial amplitude should be normalized out of the final value")] private bool normalizeAmplitude = true;
        [SerializeField] private RenderTexture inputArray;

        private bool useTextureAssets = false;
        public bool UseTextureAssets
        {
            get => useTextureAssets;
            set
            {
                if (value == false)
                {
                    if (useTextureAssets == false) { return; }
                    EnableInputTextureGeneration();
                }
                else if (value == true)
                {
                    if (useTextureAssets == true) { return; }
                    DisableInputTextureGeneration();
                }
            }
        }

        public ReadOnlyCollection<Texture> InputTextureAssets {get {
                if (inputTextureAssets == null || inputTextureAssets.Count == 0) { return null; }
                else return inputTextureAssets.AsReadOnly(); 
        } }

        private List<Texture> inputTextureAssets;
        /// <summary>
        /// Switches the component from generating textures using a different noise generator component to using textures set from the asset folder
        /// </summary>
        private void DisableInputTextureGeneration()
        {

            useTextureAssets = true;
        }
        private void EnableInputTextureGeneration()
        {
            if (inputTextureAssets.Count != 0) { ClearInputTextures(); }
            useTextureAssets = false;
        }
        public void AddInputTexture(Texture2D texture)
        {
            if (!UseTextureAssets)
            {
                Debug.Log("Disable input texture generation before adding input texture assets");
            }
            if (VerifyInputSize(texture))
            {
                inputTextureAssets.Add(texture);
            }
            else
            {
                Debug.Log("texture does not match size of other inputs");
            }
        }
        public bool VerifyInputSize(Texture2D texture)
        {
            if (inputTextureAssets.Count == 0)
            {
                TexHeight = (uint)texture.height;
                TexWidth = (uint)texture.width;
                return true;
            }
            else
            {
                if (texHeight != (uint)texture.height || texWidth != (uint)texture.width)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void ClearInputTextures()
        {
            inputTextureAssets = new List<Texture>();
        }

        protected override AbstractNoiseGenerator CreateGeneratorObject()
        {
            if (UseTextureAssets)
            {
                if (inputTextureAssets.Count != 0)
                {
                    var noiseGeneratorObject = new FractalNoiseGenerator(Octaves, inputTextureAssets, Lacunarity, Frequency, Gain, Amplitude);
                    noiseGeneratorObject.NormalizeAmplitude = NormalizeAmplitude;
                    noiseGeneratorObject.OnTextureGeneration += () => { inputArray = noiseGeneratorObject.InputTextureArray; };
                    return noiseGeneratorObject;
                } else {
                    var noiseGeneratorObject = new FractalNoiseGenerator(TexWidth, TexHeight, Octaves, Lacunarity, Frequency, Gain, Amplitude);
                    noiseGeneratorObject.NormalizeAmplitude = NormalizeAmplitude;
                    noiseGeneratorObject.OnTextureGeneration += () => { inputArray = noiseGeneratorObject.InputTextureArray; };
                    return noiseGeneratorObject;
                }
            }
            else
            {
                baseNoiseGenerator.TexHeight = TexHeight;
                baseNoiseGenerator.TexWidth = TexWidth;
                baseNoiseGenerator.Seed = seed;
                baseNoiseGenerator.GenerateTexture();
                _disposedValue = false;
                var noiseGeneratorObject = new FractalNoiseGenerator(Octaves, baseNoiseGenerator.NoiseTexture, Lacunarity, Frequency, Gain, Amplitude);
                noiseGeneratorObject.NormalizeAmplitude = NormalizeAmplitude;
                noiseGeneratorObject.OnTextureGeneration += () => { inputArray = noiseGeneratorObject.InputTextureArray; };
                return noiseGeneratorObject;
            }
        }

        public int DesiredInputTextureCount {
            get
            {
                return desiredInputTextureCount;
            }
            set { 
                desiredInputTextureCount = Mathf.Clamp(1, value, (int)Octaves);
                if(InputTextureAssets.Count > desiredInputTextureCount) {
                    for (int i = 1; i <= InputTextureAssets.Count - desiredInputTextureCount; i++)
                    {
                        inputTextureAssets.Remove(inputTextureAssets[i]);
                    }
                } 
            }
        }
        private int desiredInputTextureCount = 1;

        private void CreateInputTextures()
        {
            if (DesiredInputTextureCount == 1)
            {
                baseNoiseGenerator.GenerateTexture();
                ((FractalNoiseGenerator)NoiseGeneratorObject).SetInputTextures(baseNoiseGenerator.NoiseTexture.Copy());
            }
            else
            {
                var input = new List<Texture>();
                for (int i = 0; i < DesiredInputTextureCount; i++)
                {
                    baseNoiseGenerator.GenerateTexture();
                    input.Add(baseNoiseGenerator.NoiseTexture.Copy());
                    baseNoiseGenerator.Seed++;
                }
                ((FractalNoiseGenerator)NoiseGeneratorObject).SetInputTextures(input);
            }
        }
        protected override void UpdateGeneratorSettings()
        {
            base.UpdateGeneratorSettings();
            if(useTextureAssets){
                if(inputTextureAssets.Count != DesiredInputTextureCount) {
                    throw new System.ArgumentException($"Number of input textures selected does not match desired number of inputs, {inputTextureAssets.Count} vs {DesiredInputTextureCount}");
                }
                ((FractalNoiseGenerator)NoiseGeneratorObject).SetInputTextures(inputTextureAssets);
            } else {
                if (baseNoiseGenerator.TexHeight != TexHeight)
                {
                    baseNoiseGenerator.TexHeight = TexHeight;
                    TexHeight = baseNoiseGenerator.TexHeight;
                }
                if (baseNoiseGenerator.TexWidth != TexWidth)
                {
                    baseNoiseGenerator.TexWidth = TexWidth;
                    TexWidth = baseNoiseGenerator.TexWidth;
                }
                if (baseNoiseGenerator.Seed != Seed)
                {
                    baseNoiseGenerator.Seed = Seed;
                }
                baseNoiseGenerator.GenerateTexture();
                CreateInputTextures();
            }

            var GeneratorAsFractal = NoiseGeneratorObject as FractalNoiseGenerator;
            if (GeneratorAsFractal.Octaves != Octaves)
            {
                GeneratorAsFractal.Octaves = Octaves;
            }
            if (GeneratorAsFractal.Lacunarity != Lacunarity)
            {
                GeneratorAsFractal.Lacunarity = Lacunarity;
            }
            if (GeneratorAsFractal.Frequency != Frequency)
            {
                GeneratorAsFractal.Frequency = Frequency;
            }
            if (GeneratorAsFractal.Gain != Gain)
            {
                GeneratorAsFractal.Gain = Gain;
            }
            if (GeneratorAsFractal.NormalizeAmplitude != NormalizeAmplitude)
            {
                GeneratorAsFractal.NormalizeAmplitude = NormalizeAmplitude;
            }
            if (GeneratorAsFractal.Amplitude != Amplitude)
            {
                GeneratorAsFractal.Amplitude = Amplitude;
            }
        }

        private bool _disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                baseNoiseGenerator?.Dispose();
                _disposedValue = true;
                if (inputArray != null)
                {
                    inputArray.Release();
                    inputArray = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}