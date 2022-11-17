using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SadSapphicGames.NoiseGenerators;

namespace SadSapphicGames.NoiseGeneratorsEditor
{
    [CustomEditor(typeof(FractalNoiseGeneratorComponent))]
    public class FractalGenInspector : NoiseGenInspector
    {
        FractalNoiseGeneratorComponent targetGenerator { get => (FractalNoiseGeneratorComponent)target; }
        public override void OnInspectorGUI()
        {
            DrawGenerateTextureButton();
            DrawTexturePreview(5);
            DrawTextureAssetSwitch();
            DrawDefaultInspector();
        }
        public void DrawTextureAssetSwitch() {
            EditorGUILayout.BeginVertical();
            int selection = targetGenerator.UseTextureAssets ? 1 : 0;
            selection = GUILayout.Toolbar(selection, new string[] { "Generate Textures for Input","Use Texture Assets for Input" });
            switch (selection)
            {
                case 0:
                    if (targetGenerator.UseTextureAssets)
                    {
                        targetGenerator.UseTextureAssets = false;
                    }
                    GUILayout.Label("Generating Input Texture");
                    // targetGenerator.UseTextureAssets = false;
                    break;
                case 1:
                    if (!targetGenerator.UseTextureAssets)
                    {
                        targetGenerator.UseTextureAssets = true;
                    }
                    GUILayout.Label("Using assets as Input Texture");
                    break;
                default:
                    throw new System.Exception($"Invalid setting for input texture mode in {target.ToString()}");
            }
            EditorGUILayout.EndVertical();
        }
    }
}