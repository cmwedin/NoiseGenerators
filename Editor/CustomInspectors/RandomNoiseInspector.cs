using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SadSapphicGames.NoiseGenerators;

namespace SadSapphicGames.NoiseGeneratorsEditor
{
    [CustomEditor(typeof(RandomNoiseGenerator))]
    public class RandomNoiseInspector : Editor
    {
        RandomNoiseGenerator targetGenerator { get => (RandomNoiseGenerator)target; }
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Generate Texture")){
                targetGenerator.GenerateTexture();
            }
            base.OnInspectorGUI();
        }
    }
}