using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class FractalNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        [SerializeField] private AbstractNoiseGeneratorComponent baseNoiseGenerator;

        // [SerializeField] private MeshRenderer textureDisplay;

        private FractalNoiseGenerator noiseGeneratorObject;
        protected override AbstractNoiseGenerator NoiseGeneratorObject => noiseGeneratorObject;

        [SerializeField] private uint octaves;
        [SerializeField] private float lacunarity = 2;
        [SerializeField] private float frequency = 2;

        [SerializeField] private float gain = .5f;
        [SerializeField] private float amplitude = .5f;
        [SerializeField] private bool normalizeAmplitude = true;

        protected override void CreateGeneratorObject()
        {
            baseNoiseGenerator.TexHeight = TexHeight;
            baseNoiseGenerator.TexWidth = TexWidth;
            baseNoiseGenerator.Seed = seed;
            baseNoiseGenerator.GenerateTexture();
            noiseGeneratorObject = new FractalNoiseGenerator(octaves, baseNoiseGenerator.NoiseTexture, lacunarity, frequency, gain, amplitude);
            noiseGeneratorObject.NormalizeAmplitude = normalizeAmplitude;
        }
    }
}