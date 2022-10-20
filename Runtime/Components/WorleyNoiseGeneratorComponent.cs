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

        /// <summary>
        /// The texture channel the generated noise will be stored in (if all each channel will contain a different texture)
        /// </summary>
        [SerializeField,Tooltip("The texture channel the generated noise will be stored in (if all each channel will contain a different texture)")] 
        private TextureChannel activeChannel;
        /// <summary>
        /// If the texture should be required to tile seamlessly
        /// </summary>
        [SerializeField,Tooltip("If the texture should be required to tile seamlessly")] private bool tileTexture;
        /// <summary>
        /// If the values of the texture should be inverted (bright close to control points, dark far)
        /// </summary>
        [SerializeField,Tooltip("If the values of the texture should be inverted (bright close to control points, dark far)")] private bool invertTexture;

        /// <summary>
        /// The number of cells along the x and y axis respectively
        /// </summary>
        [SerializeField, Tooltip("The number of cells along the x and y axis respectively")]  private Vector2Int cellCounts;

        /// <summary>
        /// Constructs a WorleyNoiseGenerator object and sets it's RequireSeamlessTiling and InvertTexture properties
        /// </summary>
        protected override void CreateGeneratorObject() {
            if(noiseGeneratorObject != null) {
                noiseGeneratorObject.Dispose();
            }
            noiseGeneratorObject = new WorleyNoiseGenerator(TexWidth, TexHeight, seed, cellCounts, activeChannel);
            noiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            noiseGeneratorObject.InvertTexture = invertTexture;
            disposedValue = false;
        }
        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        // }
        // protected override void OnValidate()
        // {
        //     base.OnValidate();
        //     noiseGeneratorObject.RequireSeamlessTiling = tileTexture;
        //     noiseGeneratorObject.InvertTexture = invertTexture;
        //     noiseGeneratorObject.CellCounts = cellCounts;
        // }

    }
}