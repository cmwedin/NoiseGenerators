using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class RandomNoiseGenerator : AbstractNoiseGenerator
    {
        public RandomNoiseGenerator(uint texWidth, uint texHeight, uint seed) : base(texWidth, texHeight, seed) {
        }

        protected override string ComputeShaderPath => "Compute/RandomNoise";

        public override void GenerateTexture() {
            SetShaderParameters();
            NoiseGenShader.Dispatch(0,texThreadGroupCount.x,texThreadGroupCount.y,texThreadGroupCount.z);
        }
    }
}