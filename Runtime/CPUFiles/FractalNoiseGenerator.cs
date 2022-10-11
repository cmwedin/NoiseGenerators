using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class FractalNoiseGenerator : MonoBehaviour
    {
        [SerializeField] private AbstractNoiseGenerator baseNoiseGenerator;
        private RenderTexture inputTexture
        {
            get
            {
                if (baseNoiseGenerator.GetNoiseTexture() == null)
                {
                    baseNoiseGenerator.GenerateTexture();
                }
                return baseNoiseGenerator.GetNoiseTexture();
            }
        }
        [SerializeField] private RenderTexture noiseTexture;
        [SerializeField] private MeshRenderer textureDisplay;


        [SerializeField] private ComputeShader fractalizeShader;
        protected int generateTextureKernel => fractalizeShader.FindKernel("CSMain");
        protected int normalizeTextureKernel => fractalizeShader.FindKernel("NormalizeTexture");

        private ComputeBuffer minMaxBuffer;

        protected Vector3Int threadGroupSize = new Vector3Int(8, 8, 1);
        protected Vector3Int texThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)noiseTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)noiseTexture.height / (float)threadGroupSize.x),
                1
            );
        }
        [SerializeField] private uint octaves;
        [SerializeField] private float lacunarity;
        [SerializeField] private float gain;

        private uint texWidth { get => baseNoiseGenerator.TexWidth; }
        private uint texHeight { get => baseNoiseGenerator.TexHeight; }


        protected virtual void CleanUpOldTextures() {
            if (noiseTexture != null) {
                noiseTexture.Release();
                DestroyImmediate(noiseTexture); 
            }
            minMaxBuffer?.Dispose();
        }

        private void SetShaderParameters() {
            fractalizeShader.SetInt("_Octaves", (int)octaves);
            fractalizeShader.SetFloat("_Lacunarity", lacunarity);
            fractalizeShader.SetFloat("_Gain",gain);
            fractalizeShader.SetInt("_TexWidth",(int)texWidth);
            fractalizeShader.SetInt("_TexHeight",(int)texHeight);
            fractalizeShader.SetBuffer(generateTextureKernel,"_MinMaxBuffer", minMaxBuffer);
            fractalizeShader.SetTexture(generateTextureKernel, "_InNoiseTexture", inputTexture);
            fractalizeShader.SetTexture(generateTextureKernel, "_OutNoiseTexture", noiseTexture);
            fractalizeShader.SetBuffer(normalizeTextureKernel,"_MinMaxBuffer", minMaxBuffer);
            fractalizeShader.SetTexture(normalizeTextureKernel, "_OutNoiseTexture", noiseTexture);
        }

        public void GenerateTexture()
        {
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            minMaxBuffer = new ComputeBuffer(8, sizeof(uint));
            minMaxBuffer.SetData(new int[] { int.MaxValue, int.MaxValue,int.MaxValue,int.MaxValue,0,0,0,0 });
            SetShaderParameters();
            fractalizeShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            fractalizeShader.Dispatch(normalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            textureDisplay.sharedMaterial.mainTexture = noiseTexture;
            minMaxBuffer.Dispose();
        }
    }
}