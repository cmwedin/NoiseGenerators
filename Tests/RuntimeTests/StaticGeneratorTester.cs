using UnityEngine;
using SadSapphicGames.NoiseGenerators;

namespace SadSapphicGames.NoiseGenerators
{
    internal enum NoiseType {Random,Value,Perlin,Worley}
    public class StaticGeneratorTester : MonoBehaviour {
        [SerializeField] uint texWidth;
        [SerializeField] uint texHeight;
        [SerializeField] uint seed;
        [SerializeField] Vector2Int latticeCellSize;
        [SerializeField] bool allowPartialLatticeCells;
        [SerializeField] Vector2Int cellCount;
        [SerializeField] bool requireSeamlessTiling;
        [SerializeField] bool invertWorleyTexture;
        [SerializeField] NoiseType fractalInputType;
        [SerializeField] uint octaves;
        [SerializeField] TextureChannel worleyTextureChannel;
        [SerializeField] RenderTexture randomTexture;
        [SerializeField] RenderTexture valueTexture;
        [SerializeField] RenderTexture perlinTexture;
        [SerializeField] RenderTexture worleyTexture;
        [SerializeField] RenderTexture fractalTexture;

        private void Start() {
            randomTexture = RandomNoiseGenerator.GenerateTexture(texWidth, texHeight, seed, requireSeamlessTiling);
            valueTexture = ValueNoiseGenerator.GenerateTexture(texWidth, texHeight, seed, latticeCellSize, allowPartialLatticeCells, requireSeamlessTiling);
            perlinTexture = PerlinNoiseGenerator.GenerateTexture(texWidth, texHeight, seed, latticeCellSize, allowPartialLatticeCells, requireSeamlessTiling);
            worleyTexture = WorleyNoiseGenerator.GenerateTexture(texWidth, texHeight, seed, cellCount, worleyTextureChannel, requireSeamlessTiling, invertWorleyTexture);
            switch (fractalInputType)
            {
                case NoiseType.Random:
                    fractalTexture = FractalNoiseGenerator.GenerateTexture(octaves, fractalTexture);
                    break;
               case NoiseType.Value:
                    fractalTexture = FractalNoiseGenerator.GenerateTexture(octaves, valueTexture);
                    break;
               case NoiseType.Perlin:
                    fractalTexture = FractalNoiseGenerator.GenerateTexture(octaves, perlinTexture);
                    break;
               case NoiseType.Worley:
                    fractalTexture = FractalNoiseGenerator.GenerateTexture(octaves, worleyTexture);
                    break;
                default:
                    break;
            }
        }
    }
}