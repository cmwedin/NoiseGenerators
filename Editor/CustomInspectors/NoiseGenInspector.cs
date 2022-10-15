using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SadSapphicGames.NoiseGenerators;

namespace SadSapphicGames.NoiseGeneratorsEditor
{
    [CustomEditor(typeof(AbstractNoiseGeneratorComponent),true)]
    public class NoiseGenInspector : Editor
    {
        AbstractNoiseGeneratorComponent targetGenerator { get => (AbstractNoiseGeneratorComponent)target; }
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Generate Texture")){
                targetGenerator.GenerateTexture();
            }
            base.OnInspectorGUI();
        }
    }
}