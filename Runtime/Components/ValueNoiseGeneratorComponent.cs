using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class ValueNoiseGeneratorComponent : AbstractNoiseGeneratorComponent {
        private ValueNoiseGenerator noiseGeneratorObject;
        protected override AbstractNoiseGenerator NoiseGeneratorObject { get {
            if(noiseGeneratorObject == null) {
                CreateGeneratorObject();
            }
            return noiseGeneratorObject;
        }}
        /// <summary>
        /// If the texture should be required to tile seamlessly
        /// </summary>
        [SerializeField,Tooltip("If the texture should be required to tile seamlessly")] private bool tileTexture;
        /// <summary>
        /// The pixel size of a single lattice cell
        /// </summary>
        [SerializeField,Tooltip("The pixel size of a single lattice cell")] private Vector2Int latticeCellSize;

        protected override void CreateGeneratorObject() {
            if(noiseGeneratorObject != null) {
                noiseGeneratorObject.Dispose();
            }
            noiseGeneratorObject = new ValueNoiseGenerator(TexWidth, TexHeight, seed, latticeCellSize);
            NoiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            disposedValue = false;
        }
        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        // }
        // protected override void OnValidate()
        // {
        //     base.OnValidate();
        //     NoiseGeneratorObject.RequireSeamlessTiling = tileTexture;
        //     noiseGeneratorObject.LatticeCellSize = latticeCellSize;
        // }
    }
}