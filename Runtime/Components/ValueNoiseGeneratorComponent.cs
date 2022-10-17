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

        [SerializeField] private bool tileTexture;

        [SerializeField] private Vector2Int latticeCellSize;
        // public Vector2Int LatticeSize {
        //     get {
        //         if(TexWidth % latticeSize.x != 0) {
        //             Debug.LogWarning("Horizontal lattice size not set to factor of texture width, adjusting value to closest factor");
        //             latticeSize = new Vector2Int(
        //                 HelperMethods.FindClosestFactor((int)TexWidth,latticeSize.x),
        //                 latticeSize.y
        //             );
        //         } if (TexHeight % latticeSize.y != 0) {
        //             Debug.LogWarning("Vertical lattice size not set to factor of texture height, adjusting value to closest factor");
        //             latticeSize = new Vector2Int(
        //                 latticeSize.x,
        //                 HelperMethods.FindClosestFactor((int)TexHeight,latticeSize.y)
        //             );
        //         }
        //         return latticeSize; 
        //     }
        // }


        // private int latticeTexWidth { get => Mathf.CeilToInt((float)texWidth / (float)LatticeSize.x)+1; }
        // private int latticeTexHeight { get => Mathf.CeilToInt((float)texHeight / (float)LatticeSize.y)+1; }

        protected override void CreateGeneratorObject() {
            if(noiseGeneratorObject != null) {
                noiseGeneratorObject.Dispose();
            }
            noiseGeneratorObject = new ValueNoiseGenerator(TexWidth, TexHeight, seed, latticeCellSize);
            NoiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            disposedValue = false;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        // protected override void OnValidate()
        // {
        //     base.OnValidate();
        //     NoiseGeneratorObject.RequireSeamlessTiling = tileTexture;
        //     noiseGeneratorObject.LatticeCellSize = latticeCellSize;
        // }
    }
}