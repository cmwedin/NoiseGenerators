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
            if(GUILayout.Button("Generate Texture")){
                targetGenerator.GenerateTexture();
            }
            if (targetGenerator.TextureGenerated)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Texture Preview Size:");
                previewSize = EditorGUILayout.IntSlider(previewSize,0,Mathf.Min(targetGenerator.NoiseTexture.width,Screen.width-20));
                EditorGUILayout.EndHorizontal();
                // using (var l = new EditorGUILayout.HorizontalScope())
                // {
                //     GUILayout.FlexibleSpace();
                //     GUILayout.Label(targetGenerator.NoiseTexture, new GUILayoutOption[2] { GUILayout.Width(previewSize), GUILayout.Height(previewSize) });
                //     GUILayout.FlexibleSpace();
                // }
                int bufferSize = 5;
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
            base.OnInspectorGUI();
        }
    }
}