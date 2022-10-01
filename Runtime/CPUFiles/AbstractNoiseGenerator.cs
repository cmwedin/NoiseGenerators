using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public abstract class AbstractNoiseGenerator : MonoBehaviour
    {
        [SerializeField] protected ComputeShader noiseGenShader;
        protected abstract int generateTextureKernel { get; }
        protected Vector3Int threadGroupSize = new Vector3Int(8, 8, 1);
        protected Vector3Int texThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt((float)noiseTexture.width / (float)threadGroupSize.x),
                Mathf.CeilToInt((float)noiseTexture.height / (float)threadGroupSize.x),
                1
            );
        }
        [SerializeField] protected RenderTexture noiseTexture;
        [SerializeField] protected MeshRenderer displayMeshRenderer;
        // [SerializeField] protected bool animate;
        [SerializeField] protected uint seed;
        [SerializeField] protected uint texWidth;
        [SerializeField] protected uint texHeight;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        protected virtual void SetShaderParameters() {
            noiseGenShader.SetInt("_Seed", (int)seed);
            noiseGenShader.SetInt("_TexWidth", (int)texWidth);
            noiseGenShader.SetInt("_TexHeight", (int)texHeight);
            noiseGenShader.SetTexture(generateTextureKernel, "_NoiseTexture", noiseTexture);
        }
        protected void DisplayTexture() {
            displayMeshRenderer.sharedMaterial.mainTexture = noiseTexture;
        }
        public abstract void GenerateTexture();
    }
}