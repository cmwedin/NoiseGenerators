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
        int previewSize = 100;

        public override void OnInspectorGUI()
        {
            DrawGenerateTextureButton();
            DrawTexturePreview(5);
            // DrawBaseGeneratorInfo();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
        protected void DrawGenerateTextureButton() {
            if(GUILayout.Button("Generate Texture")){
                targetGenerator.GenerateTexture();
            }
        }

        protected void DrawTexturePreview(int bufferSize) {
            if (!targetGenerator.TextureGenerated) { return; }
            EditorGUILayout.BeginVertical();
                
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Texture Preview Size:");
            previewSize = EditorGUILayout.IntSlider(previewSize,0,Mathf.Min(targetGenerator.NoiseTexture.width,Screen.width-20));
            EditorGUILayout.EndHorizontal();

            EditorGUI.DrawPreviewTexture(
                new Rect(
                    (Screen.width - previewSize) / 2,
                    bufferSize +GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height,
                    previewSize,
                    previewSize
                ), targetGenerator.NoiseTexture
            );
            GUILayout.Space(previewSize + bufferSize);
            
            EditorGUILayout.EndVertical();
        }
        protected void DrawBasicGeneratorInfo(){
            EditorGUILayout.PropertyField(serializedObject.FindProperty("texWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("texHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("seed"));
        }

    }

}