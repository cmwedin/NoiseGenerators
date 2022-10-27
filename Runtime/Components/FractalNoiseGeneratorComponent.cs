using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class FractalNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        /// <summary>
        /// The base noise generator for creating the input texture to the fractal noise generator
        /// </summary>
        [SerializeField] private AbstractNoiseGeneratorComponent baseNoiseGenerator;

        // /// <summary>
        // /// The object that generates the fractal noise texture
        // /// </summary>
        // protected override AbstractNoiseGenerator NoiseGeneratorObject => noiseGeneratorObject;
        // private FractalNoiseGenerator noiseGeneratorObject;

        /// <summary>
        /// The number of times detail should be added onto the noise texture
        /// </summary>
        public uint Octaves { get => octaves; set => octaves = value; }

        [SerializeField,Tooltip("The number of times detail should be added onto the noise texture")] private uint octaves;
        /// <summary>
        /// The factor by which the frequency should increase with each octave
        /// </summary>
        public float Lacunarity { get => lacunarity; set => lacunarity = value; }

        [SerializeField,Tooltip("The factor by which the frequency should increase with each octave")] private float lacunarity = 2;
        /// <summary>
        /// The initial frequency in the first octave
        /// </summary>
        public float Frequency { get => frequency; set => frequency = value; }

        [SerializeField,Tooltip("The initial frequency in the first octave")] private float frequency = 2;

        /// <summary>
        /// The factor by which the amplitude should decrease each octave
        /// </summary>
        public float Gain { get => gain; set => gain = value; }

        [SerializeField,Tooltip("The factor by which the amplitude should decrease each octave")] private float gain = .5f;
        /// <summary>
        /// The initial amplitude in the first octaves, changing this has no affect is normalizeAmplitude is true
        /// </summary>
        public float Amplitude { get => amplitude; set => amplitude = value; }

        [SerializeField] private float amplitude = .5f;
        /// <summary>
        /// If the affect of the initial amplitude should be normalized out of the final value
        /// </summary>
        public bool NormalizeAmplitude { get => normalizeAmplitude; set => normalizeAmplitude = value; }
        [SerializeField,Tooltip("If the affect of the initial amplitude should be normalized out of the final value")] private bool normalizeAmplitude = true;


        protected override AbstractNoiseGenerator CreateGeneratorObject()
        {
            baseNoiseGenerator.TexHeight = TexHeight;
            baseNoiseGenerator.TexWidth = TexWidth;
            baseNoiseGenerator.Seed = seed;
            baseNoiseGenerator.GenerateTexture();
            var noiseGeneratorObject = new FractalNoiseGenerator(Octaves, baseNoiseGenerator.NoiseTexture, Lacunarity, Frequency, Gain, Amplitude);
            noiseGeneratorObject.NormalizeAmplitude = NormalizeAmplitude;
            return noiseGeneratorObject;
        }
        protected override void UpdateGeneratorSettings()
        {
            base.UpdateGeneratorSettings();
            var GeneratorAsFractal = NoiseGeneratorObject as FractalNoiseGenerator;
            if(GeneratorAsFractal.Octaves != Octaves) {
                GeneratorAsFractal.Octaves = Octaves;
            }
            if(GeneratorAsFractal.Lacunarity != Lacunarity) {
                GeneratorAsFractal.Lacunarity = Lacunarity;
            }
            if(GeneratorAsFractal.Frequency != Frequency) {
                GeneratorAsFractal.Frequency = Frequency;
            }
            if(GeneratorAsFractal.Gain != Gain) {
                GeneratorAsFractal.Gain = Gain;
            }
            if(GeneratorAsFractal.NormalizeAmplitude != NormalizeAmplitude) {
                GeneratorAsFractal.NormalizeAmplitude = NormalizeAmplitude;
            }
            if(GeneratorAsFractal.Amplitude != Amplitude) {
                GeneratorAsFractal.Amplitude = Amplitude;
            }
        }
    }
}