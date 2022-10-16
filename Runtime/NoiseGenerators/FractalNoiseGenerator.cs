using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators {
    //TODO
    public class FractalNoiseGenerator : AbstractNoiseGenerator
    {
        private AbstractNoiseGenerator BaseNoiseGenerator;
        private NoiseType baseNoiseType;

        public FractalNoiseGenerator(uint _texWidth, uint _texHeight, uint _seed) : base(_texWidth, _texHeight, _seed) {
        }

        protected override string ComputeShaderPath => throw new System.NotImplementedException();

        public override void GenerateTexture()
        {
            throw new System.NotImplementedException();
        }
    }
}