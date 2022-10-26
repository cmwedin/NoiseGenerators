using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class NoiseTextureDisplay : MonoBehaviour
    {
        [SerializeField] AbstractNoiseGeneratorComponent textureGenerator;

        private void OnValidate() {
            if (textureGenerator != null)
            {
                Debug.Log("Setting texture display event");
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