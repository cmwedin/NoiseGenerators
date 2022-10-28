using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SadSapphicGames.NoiseGenerators.Testing
{
    public class GeneratorComponentTests
    {
        const int testSize = 100000;
        Vector2Int testLatticeCellSize = new Vector2Int(64, 64);
        Vector2Int testWorleyPoinyCount = new Vector2Int(10, 10);
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
            GameObject.Destroy(go);
        }
        [Test]
        public void ValueGeneratorComponentMemoryTest()
        {
            var go = new GameObject();
            var generator = go.AddComponent<ValueNoiseGeneratorComponent>();
            generator.TexWidth = 1920; generator.TexHeight = 1920;
            generator.Seed = 1;
            generator.LatticeCellSize = testLatticeCellSize;
            generator.TileTexture = true;
            for (int i = 0; i < testSize; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
            generator.Dispose();
            GameObject.Destroy(go);
        }
        [Test]
        public void PerlinGeneratorComponentMemoryTest()
        {
            var go = new GameObject();
            var generator = go.AddComponent<PerlinNoiseGeneratorComponent>();
            generator.TexWidth = 1920; generator.TexHeight = 1920;
            generator.Seed = 1;
            generator.LatticeCellSize = testLatticeCellSize;
            generator.TileTexture = true;
            for (int i = 0; i < testSize; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
            generator.Dispose();
            GameObject.Destroy(go);
        }
        [Test]
        public void WorleyGeneratorComponentMemoryTest()
        {
            var go = new GameObject();
            var generator = go.AddComponent<WorleyNoiseGeneratorComponent>();
            generator.TexWidth = 1920; generator.TexHeight = 1920;
            generator.Seed = 1;
            generator.CellCounts = testWorleyPoinyCount;
            generator.TileTexture = true;
            generator.ActiveChannel = TextureChannel.R;
            for (int i = 0; i < 1000; i++)
            {
                generator.GenerateTexture();
                generator.Seed++;
            }
            generator.Dispose();
            GameObject.Destroy(go);
        }
    }
}