using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class RandomNoiseGenerator : AbstractNoiseGenerator
    {
        protected override string ComputeShaderPath => "Compute/RandomNoise";
        
        protected override int generateTextureKernel => NoiseGenShader.FindKernel("CSMain");

        public override void GenerateTexture() {
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            SetShaderParameters();
            NoiseGenShader.Dispatch(0,texThreadGroupCount.x,texThreadGroupCount.y,texThreadGroupCount.z);
            DisplayTexture();
        }
    }
}
