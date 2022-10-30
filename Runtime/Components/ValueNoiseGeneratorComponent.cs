using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// A MonoBehaviour component wrapping a ValueNoiseGenerator object
    /// </summary>
    public class ValueNoiseGeneratorComponent : AbstractNoiseGeneratorComponent {
        /// <summary>
        /// If the texture should be required to tile seamlessly
        /// </summary>
        public bool TileTexture { get => tileTexture; set => tileTexture = value; }
        [SerializeField,Tooltip("If the texture should be required to tile seamlessly")] private bool tileTexture;
        /// <summary>
        /// The pixel size of a single lattice cell
        /// </summary>
        public Vector2Int LatticeCellSize { get => latticeCellSize; set => latticeCellSize = value; }
        [SerializeField,Tooltip("The pixel size of a single lattice cell")] private Vector2Int latticeCellSize;

        /// <summary>
        /// Creates a ValueNoiseGeneratorObject and sets its RequireSeamlessTiling property
        /// </summary>
        protected override AbstractNoiseGenerator CreateGeneratorObject() {
            var noiseGeneratorObject = new ValueNoiseGenerator(TexWidth, TexHeight, seed, latticeCellSize);
            noiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            return noiseGeneratorObject;
        }
        protected override void UpdateGeneratorSettings()
        {
            base.UpdateGeneratorSettings();
            var GeneratorAsValue = NoiseGeneratorObject as ValueNoiseGenerator;
            if(GeneratorAsValue.LatticeCellSize != LatticeCellSize) {
                GeneratorAsValue.LatticeCellSize = LatticeCellSize;
                LatticeCellSize = GeneratorAsValue.LatticeCellSize;
            }
            if(GeneratorAsValue.RequireSeamlessTiling != TileTexture) {
                GeneratorAsValue.RequireSeamlessTiling = TileTexture;
            }
        }
    }
}