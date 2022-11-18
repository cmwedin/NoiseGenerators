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
            DrawInputCountSlider();
            DrawTextureAssetSwitch();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
        protected void DrawInputCountSlider(){
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Input Texture Count");
            targetGenerator.InputTextureCount = EditorGUILayout.IntSlider(targetGenerator.InputTextureCount, 1, (int)targetGenerator.Octaves);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        protected void DrawTextureAssetSwitch() {
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
                    DrawGeneratorSelection();
                    GUILayout.Label("Generating Input Texture");
                    // targetGenerator.UseTextureAssets = false;
                    break;
                case 1:
                    if (!targetGenerator.UseTextureAssets)
                    {
                        targetGenerator.UseTextureAssets = true;
                    }
                    DrawInputTextureSelection();
                    GUILayout.Label("Using assets as Input Texture");
                    break;
                default:
                    throw new System.Exception($"Invalid setting for input texture mode in {target.ToString()}");
            }
            EditorGUILayout.EndVertical();
        }
        protected void DrawInputTextureSelection() {
            int selectedInputsCount = targetGenerator.InputTextureAssets.Count;
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Clear Inputs")){
                targetGenerator.ClearInputTextures();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            for (int i = 0; i < selectedInputsCount; i++)
            {
                EditorGUILayout.ObjectField(targetGenerator.InputTextureAssets[i], typeof(Texture2D),false);
            }
            EditorGUI.EndDisabledGroup();
            for (int j = selectedInputsCount; j < targetGenerator.InputTextureCount; j++) {
                Texture2D selection = null;
                selection = (Texture2D)EditorGUILayout.ObjectField(selection, typeof(Texture2D), false);
                if (selection != null) { targetGenerator.AddInputTexture(selection); }
            }
            EditorGUILayout.EndHorizontal();
        }
        protected void DrawGeneratorSelection(){
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseNoiseGenerator"));
        }
    }
}