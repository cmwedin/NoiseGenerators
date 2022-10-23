using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class PerlinNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        private PerlinNoiseGenerator noiseGeneratorObject;
        protected override AbstractNoiseGenerator NoiseGeneratorObject
        {
            get
            {
                if (noiseGeneratorObject == null)
                {
                    CreateGeneratorObject();
                }
                return noiseGeneratorObject;
            }
        }

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
        protected override void CreateGeneratorObject()
        {
            if(noiseGeneratorObject != null) {
                noiseGeneratorObject.Dispose();
            }
            noiseGeneratorObject = new PerlinNoiseGenerator(TexWidth, TexHeight, seed, latticeCellSize);
            NoiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            disposedValue = false;
        }
    }
}