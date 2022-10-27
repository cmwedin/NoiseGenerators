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
        [SerializeField,Tooltip("The number of times detail should be added onto the noise texture")] private uint octaves;
        /// <summary>
        /// The factor by which the frequency should increase with each octave
        /// </summary>
        [SerializeField,Tooltip("The factor by which the frequency should increase with each octave")] private float lacunarity = 2;
        /// <summary>
        /// The initial frequency in the first octave
        /// </summary>
        [SerializeField,Tooltip("The initial frequency in the first octave")] private float frequency = 2;

        /// <summary>
        /// The factor by which the amplitude should decrease each octave
        /// </summary>
        [SerializeField,Tooltip("The factor by which the amplitude should decrease each octave")] private float gain = .5f;
        /// <summary>
        /// The initial amplitude in the first octaves, changing this has no affect is normalizeAmplitude is true
        /// </summary>
        [SerializeField] private float amplitude = .5f;
        /// <summary>
        /// If the affect of the initial amplitude should be normalized out of the final value
        /// </summary>
        [SerializeField,Tooltip("If the affect of the initial amplitude should be normalized out of the final value")] private bool normalizeAmplitude = true;

        protected override AbstractNoiseGenerator CreateGeneratorObject()
        {
            baseNoiseGenerator.TexHeight = TexHeight;
            baseNoiseGenerator.TexWidth = TexWidth;
            baseNoiseGenerator.Seed = seed;
            baseNoiseGenerator.GenerateTexture();
            var noiseGeneratorObject = new FractalNoiseGenerator(octaves, baseNoiseGenerator.NoiseTexture, lacunarity, frequency, gain, amplitude);
            noiseGeneratorObject.NormalizeAmplitude = normalizeAmplitude;
            return noiseGeneratorObject;
        }
    }
}