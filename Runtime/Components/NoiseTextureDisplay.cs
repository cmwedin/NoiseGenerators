using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter)), ExecuteInEditMode]
    public class NoiseTextureDisplay : MonoBehaviour
    {
        [SerializeField] AbstractNoiseGeneratorComponent textureGenerator;

        private void OnEnable() {
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Texture"));
            var tmpGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
            GetComponent<MeshFilter>().sharedMesh = tmpGO.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(tmpGO);
        }
        private void OnValidate() {
            if (textureGenerator != null)
            {
                textureGenerator.GeneratedTexture += () =>
                {
                    if (Application.isPlaying)
                    {
                        GetComponent<MeshRenderer>().material.mainTexture = textureGenerator.NoiseTexture;
                    } else if (Application.isEditor) {
                        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = textureGenerator.NoiseTexture;
                    }
                };
            }
        }
    }
}