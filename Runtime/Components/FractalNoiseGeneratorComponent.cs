using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class FractalNoiseGeneratorComponent : MonoBehaviour
    {
        [SerializeField] private AbstractNoiseGeneratorComponent baseNoiseGenerator;
        private RenderTexture inputTexture
        {
            get
            {
                baseNoiseGenerator.GenerateTexture();
                return baseNoiseGenerator.NoiseTexture;
            }
        }
        [SerializeField] private RenderTexture noiseTexture;
        [SerializeField] private MeshRenderer textureDisplay;


        private ComputeShader fractalizeShader;
        public ComputeShader FractalizeShader { get {
            if(fractalizeShader == null) {
                fractalizeShader = Resources.Load<ComputeShader>("Compute/NoiseFractalizer");
            }
            return fractalizeShader;
        }}

        protected int generateTextureKernel => FractalizeShader.FindKernel("CSMain");
        protected int normalizeTextureKernel => FractalizeShader.FindKernel("NormalizeTexture");

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
        [SerializeField] private float lacunarity = 2;
        [SerializeField] private float frequency = 2;

        [SerializeField] private float gain = .5f;
        [SerializeField] private float amplitude = .5f;
        [SerializeField] private bool normalizeAmplitude = true;

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
            FractalizeShader.SetInt("_Octaves", (int)octaves);
            FractalizeShader.SetFloat("_Lacunarity", lacunarity);
            FractalizeShader.SetFloat("_Frequency", frequency);
            FractalizeShader.SetFloat("_Gain",gain);
            FractalizeShader.SetFloat("_Amplitude", amplitude);
            FractalizeShader.SetBool("_NormalizeAmplitude", normalizeAmplitude);
            FractalizeShader.SetInt("_TexWidth",(int)texWidth);
            FractalizeShader.SetInt("_TexHeight",(int)texHeight);
            FractalizeShader.SetBuffer(generateTextureKernel,"_MinMaxBuffer", minMaxBuffer);
            FractalizeShader.SetTexture(generateTextureKernel, "_InNoiseTexture", inputTexture);
            FractalizeShader.SetTexture(generateTextureKernel, "_OutNoiseTexture", noiseTexture);
            FractalizeShader.SetBuffer(normalizeTextureKernel,"_MinMaxBuffer", minMaxBuffer);
            FractalizeShader.SetTexture(normalizeTextureKernel, "_OutNoiseTexture", noiseTexture);
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
            FractalizeShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            FractalizeShader.Dispatch(normalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            textureDisplay.sharedMaterial.mainTexture = noiseTexture;
            minMaxBuffer.Dispose();
        }
    }
}