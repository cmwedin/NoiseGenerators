using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SadSapphicGames.NoiseGenerators;

namespace SadSapphicGames.NoiseGeneratorsEditor
{
    [CustomEditor(typeof(AbstractNoiseGenerator),true)]
    public class NoiseGenInspector : Editor
    {
        AbstractNoiseGenerator targetGenerator { get => (AbstractNoiseGenerator)target; }
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Generate Texture")){
                targetGenerator.GenerateTexture();
            }
            base.OnInspectorGUI();
        }
    }
}