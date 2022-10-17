using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class WorleyNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        private WorleyNoiseGenerator noiseGeneratorObject;
        protected override AbstractNoiseGenerator NoiseGeneratorObject { get {
            if(noiseGeneratorObject == null) {
                CreateGeneratorObject();
            }
            return noiseGeneratorObject;
        }}

        [SerializeField] private TextureChannel activeChannel;
        [SerializeField] private bool tileTexture;
        [SerializeField] private bool invertTexture;

        [SerializeField, Tooltip("The number of cells along the x and y axis respectively")] 
        private Vector2Int cellCounts;

        protected override void CreateGeneratorObject() {
            if(noiseGeneratorObject != null) {
                noiseGeneratorObject.Dispose();
            }
            noiseGeneratorObject = new WorleyNoiseGenerator(TexWidth, TexHeight, seed, cellCounts, activeChannel);
            noiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            noiseGeneratorObject.InvertTexture = invertTexture;
            disposedValue = false;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        // protected override void OnValidate()
        // {
        //     base.OnValidate();
        //     noiseGeneratorObject.RequireSeamlessTiling = tileTexture;
        //     noiseGeneratorObject.InvertTexture = invertTexture;
        //     noiseGeneratorObject.CellCounts = cellCounts;
        // }

    }
}