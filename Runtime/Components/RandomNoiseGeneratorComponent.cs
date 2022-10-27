using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
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
