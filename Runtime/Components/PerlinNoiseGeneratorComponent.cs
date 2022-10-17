using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class PerlinNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        private PerlinNoiseGenerator noiseGeneratorObject;
        protected override AbstractNoiseGenerator NoiseGeneratorObject { get {
            if(noiseGeneratorObject == null) {
                CreateGeneratorObject();
            }
            return noiseGeneratorObject;
            
        }}

        [SerializeField] bool tileTexture;

        [SerializeField] private Vector2Int latticeSize;


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

        // Start is called before the first frame update
        protected override void CreateGeneratorObject()
        {
            if(noiseGeneratorObject != null) {
                noiseGeneratorObject.Dispose();
            }
            noiseGeneratorObject = new PerlinNoiseGenerator(TexWidth, TexHeight, seed, latticeSize);
            NoiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            disposedValue = false;
        }
        // protected override void OnValidate()
        // {
        //     base.OnValidate();
        //     noiseGeneratorObject.LatticeCellSize = latticeSize;
        //     NoiseGeneratorObject.RequireSeamlessTiling = tileTexture;
        // }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Debug.Log("Disposing noise generator");
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                noiseGeneratorObject.Dispose();
                disposedValue = true;
            }
        }
    }
}