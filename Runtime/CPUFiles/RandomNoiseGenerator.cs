using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class RandomNoiseGenerator : MonoBehaviour
    {
        public ComputeShader randomNoiseShader;
        public RenderTexture noiseTexture;
        public uint seed;
        public uint texWidth;
        public uint texHeight;
        // Start is called before the first frame update
        void Start()
        {
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();

            randomNoiseShader.SetTexture(0, "_NoiseTexture", noiseTexture);
            randomNoiseShader.SetInt("_Seed", (int)seed);
            randomNoiseShader.SetInt("_TexWidth", (int)texWidth);
            randomNoiseShader.SetInt("_TexHeight", (int)texHeight);

            GenerateTexture();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GenerateTexture()
        {
            randomNoiseShader.Dispatch(0, noiseTexture.width / 8, noiseTexture.height / 8, 1);
        }
    }
}
