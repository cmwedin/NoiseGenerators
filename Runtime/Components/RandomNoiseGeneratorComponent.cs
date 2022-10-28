using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// A MonoBehaviour component wrapping a RandomNoiseGenerator object
    /// </summary>
    public class RandomNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        /// <summary>
        /// Constructs a RandomNoiseGenerator object
        /// </summary>
        protected override AbstractNoiseGenerator CreateGeneratorObject()
        {
            var noiseGeneratorObject = new RandomNoiseGenerator(TexWidth, TexHeight, seed);
            return noiseGeneratorObject;
        }
    }
}
