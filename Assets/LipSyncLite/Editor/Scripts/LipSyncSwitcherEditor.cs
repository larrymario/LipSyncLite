using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LipSyncLite
{
    [CustomEditor(typeof(LipSyncSwitcher))]
    [CanEditMultipleObjects]
    public class LipSyncSwitcherEditor : Editor
    {
        private bool isKeyTimePointsFoldout;

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfDirtyOrScript();
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lipSync"));
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("targets"), true);
                SerializedProperty keyTimePoints = serializedObject.FindProperty("keyTimePoints");
                SerializedProperty targetIndexs = serializedObject.FindProperty("targetIndexs");
                isKeyTimePointsFoldout = EditorGUILayout.Foldout(isKeyTimePointsFoldout, "Key time points");
                EditorGUI.indentLevel += 1;
                {
                    if (isKeyTimePointsFoldout == true)
                    {
                        EditorGUILayout.PropertyField(keyTimePoints.FindPropertyRelative("Array.size"));
                        //keyTimePoints.FindPropertyRelative("Array.size").intValue = targetIndexs.FindPropertyRelative("Array.size").intValue;
                        targetIndexs.arraySize = keyTimePoints.arraySize;
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("Key Time point");
                                EditorGUILayout.LabelField("Target index");
                            }
                            EditorGUILayout.EndHorizontal();
                            for (int i = 0; i < targetIndexs.arraySize; ++i)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.PropertyField(keyTimePoints.GetArrayElementAtIndex(i), GUIContent.none);
                                    EditorGUILayout.PropertyField(targetIndexs.GetArrayElementAtIndex(i), GUIContent.none);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUI.indentLevel -= 1;
            }
            serializedObject.ApplyModifiedProperties();
        }


    }
}
