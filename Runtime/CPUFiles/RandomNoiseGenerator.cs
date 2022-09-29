using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class RandomNoiseGenerator : MonoBehaviour
    {
        public ComputeShader randomNoiseShader;
        public RenderTexture noiseTexture;
        public MeshRenderer displayMeshRenderer;
        public uint seed;
        public uint texWidth;
        public uint texHeight;
        // Start is called before the first frame update
        void Start()
        {
            // GenerateTexture();
        }

        // Update is called once per frame
        void Update() {
        }

        private void SetShaderParameters() {
            randomNoiseShader.SetInt("_Seed", (int)seed);
            randomNoiseShader.SetInt("_TexWidth", (int)texWidth);
            randomNoiseShader.SetInt("_TexHeight", (int)texHeight);
            randomNoiseShader.SetTexture(0, "_NoiseTexture", noiseTexture);
        }

        public void GenerateTexture() {
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            SetShaderParameters();
            randomNoiseShader.Dispatch(0, noiseTexture.width / 8, noiseTexture.height / 8, 1);
            displayMeshRenderer.material.mainTexture = noiseTexture;
        }
        public void DisplayTexture() {
        }
        private void OnPreRender() {
            Camera.main.targetTexture = noiseTexture;
        }
        private void OnPostRender() {
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), noiseTexture);
            Camera.main.targetTexture = null;
        }        
    }
}
