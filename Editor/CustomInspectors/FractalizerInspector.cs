using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SadSapphicGames.NoiseGenerators;

namespace SadSapphicGames.NoiseGeneratorsEditor
{
    [CustomEditor(typeof(FractalNoiseGeneratorComponent))]
    public class FractalizerInspector : NoiseGenInspector
    {
        FractalNoiseGeneratorComponent targetGenerator { get => (FractalNoiseGeneratorComponent)target; }
        public override void OnInspectorGUI()
        {
            DrawGenerateTextureButton();
            DrawTexturePreview(5);
            DrawDefaultInspector();
        }
    }
}