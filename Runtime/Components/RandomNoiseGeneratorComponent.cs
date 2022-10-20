using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class RandomNoiseGeneratorComponent : AbstractNoiseGeneratorComponent
    {
        private RandomNoiseGenerator noiseGeneratorObject;
        protected override AbstractNoiseGenerator NoiseGeneratorObject { get { 
            if(noiseGeneratorObject == null) {
                    CreateGeneratorObject();
                }
            return noiseGeneratorObject; 
        } }

        /// <summary>
        /// Constructs a RandomNoiseGenerator object
        /// </summary>
        protected override void CreateGeneratorObject()
        {
            if(noiseGeneratorObject != null) {
                noiseGeneratorObject.Dispose();
            }
            noiseGeneratorObject = new RandomNoiseGenerator(TexWidth, TexHeight, seed);
            disposedValue = false;
        }
    }
}
