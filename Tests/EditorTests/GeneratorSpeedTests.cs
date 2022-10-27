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
        Vector2Int testLatticeCellSize = new Vector2Int(64, 64);
        Vector2Int testCellCount = new Vector2Int(10, 10);
        // A Test behaves as an ordinary method
        [Test]
        public void RandomGeneratorSpedTest()
        {
            var generator = new RandomNoiseGenerator(1920, 1920, 1);
            for (int i = 0; i < testSize; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
            generator.Dispose();
        }
        [Test]
        public void ValueGeneratorSpedTest()
        {
            var generator = new ValueNoiseGenerator(1920, 1920, 1,testLatticeCellSize);
            for (int i = 0; i < testSize; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
            generator.Dispose();
        }
        [Test]
        public void PerlinGeneratorSpedTest()
        {
            var generator = new PerlinNoiseGenerator(1920, 1920, 1,testLatticeCellSize);
            for (int i = 0; i < testSize; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
            generator.Dispose();
        }
        [Test]
        public void WorleyGeneratorSpedTest()
        {
            var generator = new WorleyNoiseGenerator(1920, 1920, 1,testCellCount,TextureChannel.R);
            for (int i = 0; i < testSize; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
            generator.Dispose();
        }
    }
}