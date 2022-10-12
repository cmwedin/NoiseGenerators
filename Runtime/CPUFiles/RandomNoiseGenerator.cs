using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class RandomNoiseGenerator : AbstractNoiseGenerator
    {
        protected override int generateTextureKernel => noiseGenShader.FindKernel("CSMain");

        public override void GenerateTexture() {
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            SetShaderParameters();
            noiseGenShader.Dispatch(0,texThreadGroupCount.x,texThreadGroupCount.y,texThreadGroupCount.z);
            DisplayTexture();
        }
    }
}
