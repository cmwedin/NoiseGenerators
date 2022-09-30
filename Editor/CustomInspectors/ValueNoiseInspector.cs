using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SadSapphicGames.NoiseGenerators;

namespace SadSapphicGames.NoiseGeneratorsEditor
{
    [CustomEditor(typeof(ValueNoiseGenerator))]
    public class ValueNoiseInspector : Editor
    {
        ValueNoiseGenerator targetGenerator { get => (ValueNoiseGenerator)target; }
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Generate Texture")){
                targetGenerator.GenerateTexture();
            }
            base.OnInspectorGUI();
        }
    }
}
