using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SadSapphicGames.NoiseGenerators.Testing
{
    public class GeneratorComponentTests
    {
        const int testSize = 5000;
        // A Test behaves as an ordinary method
        [Test]
        public void RandomGeneratorComponentMemoryTest()
        {
            var go = new GameObject();
            var generator = go.AddComponent<RandomNoiseGeneratorComponent>();
            generator.TexWidth = 1920; generator.TexHeight = 1920;
            generator.Seed = 1;
            for (int i = 0; i < testSize; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
            generator.Dispose();
        }
    }
}