using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// A generator object used to create a purely random noise texture
    /// </summary>
    public class RandomNoiseGenerator : AbstractNoiseGenerator
    {
        /// <summary>
        /// Constructs a RandomNoiseGenerator
        /// </summary>
        /// <param name="texWidth">The width of the generated texture</param>
        /// <param name="texHeight">The height of the generated texture</param>
        /// <param name="seed">The seed of the pseudo-random number generation</param>
        public RandomNoiseGenerator(uint texWidth, uint texHeight, uint seed) : base(texWidth, texHeight, seed) {
        }

        protected override string ComputeShaderPath => "Compute/RandomNoise";

        protected override void InnerGenerateTexture() {
            NoiseGenShader.Dispatch(GenerateTextureKernel,texThreadGroupCount.x,texThreadGroupCount.y,texThreadGroupCount.z);
        }
        /// <summary>
        /// Generates a random noise texture using the provided parameters
        /// </summary>
        /// <param name="_texWidth">the width of the generated texture</param>
        /// <param name="_texHeight">the height of the generated texture</param>
        /// <param name="_seed">the seed of the pseudo-random number generation</param>
        /// <param name="_requireSeamlessTiling">If the texture should tile seamlessly</param>
        /// <returns>the generated texture</returns>
        public static RenderTexture GenerateTexture(uint _texWidth, uint _texHeight, uint _seed, bool _requireSeamlessTiling = true) {
            using RandomNoiseGenerator generator = new RandomNoiseGenerator(_texWidth, _texHeight, _seed);
            generator.RequireSeamlessTiling = _requireSeamlessTiling;
            generator.GenerateTexture();
            RenderTexture output = generator.NoiseTexture.Copy();
            generator.Dispose();
            return output;
        }
    }
}