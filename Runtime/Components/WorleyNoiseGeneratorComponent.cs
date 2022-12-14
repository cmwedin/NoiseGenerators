using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// A MonoBehaviour component wrapping a WorleyNoiseGenerator object
    /// </summary>
    public class WorleyNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        // private WorleyNoiseGenerator noiseGeneratorObject;
        // protected override AbstractNoiseGenerator NoiseGeneratorObject { get {
        //     if(noiseGeneratorObject == null) {
        //         CreateGeneratorObject();
        //     }
        //     return noiseGeneratorObject;
        // }}

        /// <summary>
        /// The texture channel the generated noise will be stored in (if all each channel will contain a different texture)
        /// </summary>
        public TextureChannel ActiveChannel { get => activeChannel; set => activeChannel = value; }
        [SerializeField,Tooltip("The texture channel the generated noise will be stored in (if all each channel will contain a different texture)")] 
        private TextureChannel activeChannel;
        /// <summary>
        /// If the texture should be required to tile seamlessly
        /// </summary>
        public bool TileTexture { get => tileTexture; set => tileTexture = value; }
        [SerializeField,Tooltip("If the texture should be required to tile seamlessly")] private bool tileTexture;
        /// <summary>
        /// If the values of the texture should be inverted (bright close to control points, dark far)
        /// </summary>
        public bool InvertTexture { get => invertTexture; set => invertTexture = value; }
        [SerializeField,Tooltip("If the values of the texture should be inverted (bright close to control points, dark far)")] private bool invertTexture;

        /// <summary>
        /// The number of cells along the x and y axis respectively
        /// </summary>
        public Vector2Int CellCounts { get => cellCounts; set => cellCounts = value; }
        [SerializeField, Tooltip("The number of cells along the x and y axis respectively")]  private Vector2Int cellCounts;

        /// <summary>
        /// Constructs a WorleyNoiseGenerator object and sets it's RequireSeamlessTiling and InvertTexture properties
        /// </summary>
        protected override AbstractNoiseGenerator CreateGeneratorObject() {
            var noiseGeneratorObject = new WorleyNoiseGenerator(TexWidth, TexHeight, seed, cellCounts, activeChannel);
            noiseGeneratorObject.RequireSeamlessTiling = tileTexture;
            noiseGeneratorObject.InvertTexture = invertTexture;
            return noiseGeneratorObject;
        }
        protected override void UpdateGeneratorSettings()
        {
            base.UpdateGeneratorSettings();
            var GeneratorAsWorley = NoiseGeneratorObject as WorleyNoiseGenerator;
            if(GeneratorAsWorley.InvertTexture != InvertTexture) {
                GeneratorAsWorley.InvertTexture = InvertTexture;
            }
            if(GeneratorAsWorley.RequireSeamlessTiling != TileTexture) {
                GeneratorAsWorley.RequireSeamlessTiling = TileTexture;
            }
            if(GeneratorAsWorley.CellCounts != CellCounts) {
                GeneratorAsWorley.CellCounts = CellCounts;
                CellCounts = GeneratorAsWorley.CellCounts;
            }
            if(GeneratorAsWorley.ActiveChannel != ActiveChannel) {
                GeneratorAsWorley.ActiveChannel = ActiveChannel;
            }
        }
    }
}