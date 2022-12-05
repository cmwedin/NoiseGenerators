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
            DrawInputCountSlider();
            if(targetGenerator.UseTextureAssets) {
                EditorGUILayout.BeginHorizontal();      
                if(GUILayout.Button("Clear Inputs")){
                    targetGenerator.ClearInputTextures();
                }
                EditorGUILayout.EndHorizontal();
                DrawInputTextureSelection();
            } else {
                DrawGeneratorSelection();
            }
            // DrawDefaultInspector();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("seed"));
            if(targetGenerator.UseTextureAssets) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("texWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("texWidth"));
                EditorGUI.EndDisabledGroup();
            } else {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("texWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("texWidth"));
            }
            DrawFractalGeneratorInfo();
            serializedObject.ApplyModifiedProperties();
        }
        protected void DrawInputCountSlider(){
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Desired Input Texture Count");
            targetGenerator.DesiredInputTextureCount = EditorGUILayout.IntSlider(targetGenerator.DesiredInputTextureCount, 1, (int)targetGenerator.Octaves);
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
                    break;
                case 1:
                    if (!targetGenerator.UseTextureAssets)
                    {
                        targetGenerator.UseTextureAssets = true;
                    }
                    break;
                default:
                    throw new System.Exception($"Invalid setting for input texture mode in {target.ToString()}");
            }
            EditorGUILayout.EndVertical();
        }
        protected void DrawInputTextureSelection() {
            int selectedInputsCount = targetGenerator.InputTextureAssets != null ? targetGenerator.InputTextureAssets.Count : 0;
            int maxTexPerRow = 5;
            EditorGUILayout.BeginHorizontal();
            
            //? Selected Textures
            EditorGUI.BeginDisabledGroup(true);
            if(selectedInputsCount >= maxTexPerRow) {
                for (int i = 0; i < selectedInputsCount && i  < targetGenerator.DesiredInputTextureCount; i++)
                {
                EditorGUILayout.ObjectField(targetGenerator.InputTextureAssets[i], typeof(Texture2D),false);
                    if((i + 1) % maxTexPerRow == 0){
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    } 
                }
            } else {
                for (int i = 0; i < selectedInputsCount && i  < targetGenerator.DesiredInputTextureCount; i++)
                {
                    EditorGUILayout.ObjectField(targetGenerator.InputTextureAssets[i], typeof(Texture2D),false);
                }
            }
            EditorGUI.EndDisabledGroup();
            
            //? Unselected Textures
            for (int j = selectedInputsCount; j < targetGenerator.DesiredInputTextureCount; j++) {
                Texture2D selection = null;
                selection = (Texture2D)EditorGUILayout.ObjectField(selection, typeof(Texture2D), false);
                if (selection != null) { targetGenerator.AddInputTexture(selection); }
                if((j + 1) % maxTexPerRow == 0){
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                } 
            }
            EditorGUILayout.EndHorizontal();
        }
        protected void DrawGeneratorSelection(){
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseNoiseGenerator"));
        }
        protected void DrawFractalGeneratorInfo() {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("octaves"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lacunarity"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frequency"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gain"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("normalizeAmplitude"));
            if (!targetGenerator.NormalizeAmplitude)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amplitude"));
            }
        }
    }
}