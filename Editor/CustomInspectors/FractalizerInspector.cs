using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SadSapphicGames.NoiseGenerators;

namespace SadSapphicGames.NoiseGeneratorsEditor
{
    [CustomEditor(typeof(FractalNoiseGeneratorComponent))]
    public class FractalizerInspector : Editor
    {
        FractalNoiseGeneratorComponent targetGenerator { get => (FractalNoiseGeneratorComponent)target; }
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Generate Texture")){
                targetGenerator.GenerateTexture();
            }
            base.OnInspectorGUI();
        }
    }
}