using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class PerlinNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        /// <summary>
        /// If the texture should be required to tile seamlessly
        /// </summary>
        public bool TileTexture { get => tileTexture; set => tileTexture = value; }
        [SerializeField, Tooltip("If the texture should be required to tile seamlessly")] private bool tileTexture;
        
        /// <summary>
        /// The pixel size of a single lattice cell
        /// </summary>
        public Vector2Int LatticeCellSize { get => latticeCellSize; set => latticeCellSize = value; }
        [SerializeField,Tooltip("The pixel size of a single lattice cell")] private Vector2Int latticeCellSize;

        /// <summary>
        /// Constructs a PerlinNoiseGenerator and sets it's RequireSeamlessTiling property
        /// </summary>
        protected override AbstractNoiseGenerator CreateGeneratorObject()
        {
            var noiseGeneratorObject = new PerlinNoiseGenerator(TexWidth, TexHeight, seed, latticeCellSize);
            noiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            return noiseGeneratorObject;
        }
        protected override void UpdateGeneratorSettings()
        {
            base.UpdateGeneratorSettings();
            var GeneratorAsPerlin = NoiseGeneratorObject as PerlinNoiseGenerator;
            if(GeneratorAsPerlin.LatticeCellSize != LatticeCellSize) {
                GeneratorAsPerlin.LatticeCellSize = LatticeCellSize;
                LatticeCellSize = GeneratorAsPerlin.LatticeCellSize;
            }
            if(GeneratorAsPerlin.RequireSeamlessTiling != TileTexture) {
                GeneratorAsPerlin.RequireSeamlessTiling = TileTexture;
            }
        }
    }
}