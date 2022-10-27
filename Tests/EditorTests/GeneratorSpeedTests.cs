using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SadSapphicGames.NoiseGenerators.Testing
{
    public class GeneratorSpeedTests
    {
        // A Test behaves as an ordinary method
        const int testSize = 100000;
        // A Test behaves as an ordinary method
        [Test]
        public void RandomGeneratorSpedTest()
        {
            RandomNoiseGenerator generator = new RandomNoiseGenerator(1920, 1920, 1);
            for (int i = 0; i < testSize; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
        }
    }
}