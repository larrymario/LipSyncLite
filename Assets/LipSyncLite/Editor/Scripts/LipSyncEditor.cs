using UnityEngine;
using UnityEditor;


namespace LipSyncLite
{
    [CustomEditor(typeof(LipSync))]
    [CanEditMultipleObjects]
    public class LipSyncEditor : Editor
    {
        private bool isAdvancedOptionsFoldOut;

        public override void OnInspectorGUI()
        {
            LipSync targetLipSync = (LipSync)target;
            serializedObject.UpdateIfDirtyOrScript();
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lipSyncMethod"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSource"));
                EditorGUILayout.Space();
                if (targetLipSync.lipSyncMethod == ELipSyncMethod.Runtime)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("recognizerLanguage"));
                    string[] selectedVowels = null;
                    switch (targetLipSync.recognizerLanguage)
                    {
                        case ERecognizerLanguage.Japanese:
                            selectedVowels = LipSync.vowelsJP;
                            break;
                        case ERecognizerLanguage.Chinese:
                            selectedVowels = LipSync.vowelsCN;
                            break;
                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("targetType"));
                    if (targetLipSync.targetType == ETargetType.BlendShape)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetBlendShapeObject"));
                        EditorGUILayout.LabelField("Vowel Property Names");
                        EditorGUILayout.BeginVertical(EditorStyles.textField);
                        {
                            SerializedProperty propertyNames = serializedObject.FindProperty("propertyNames");


                            for (int i = 0; i < selectedVowels.Length; ++i)
                            {
                                EditorGUILayout.PropertyField(propertyNames.GetArrayElementAtIndex(i), new GUIContent(selectedVowels[i]));
                            }

                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("propertyMinValue"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("propertyMaxValue"));
                    }
                    else if (targetLipSync.targetType == ETargetType.Live2D)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetLive2DHelper"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("paramXName"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("paramYName"));
                        EditorGUILayout.LabelField("Vowel Property Max Values");
                        EditorGUILayout.BeginVertical(EditorStyles.textField);
                        {
                            SerializedProperty paramMaxValues = serializedObject.FindProperty("paramMaxValues");

                            for (int i = 0; i < selectedVowels.Length; ++i)
                            {
                                EditorGUILayout.PropertyField(paramMaxValues.GetArrayElementAtIndex(i), new GUIContent(selectedVowels[i]));
                            }

                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.Space();

                    isAdvancedOptionsFoldOut = EditorGUILayout.Foldout(isAdvancedOptionsFoldOut, "Advanced Options");
                    if (isAdvancedOptionsFoldOut == true)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("windowSize"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("amplitudeThreshold"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("moveTowardsSpeed"));
                    }
                    EditorGUILayout.Space();

                    if (Application.isPlaying)
                    {
                        EditorGUILayout.HelpBox(
                            "Changes of settings at runtime must be applied manually using the button below.",
                            MessageType.Warning);
                        if (GUILayout.Button("Apply runtime changes") == true)
                        {
                            targetLipSync.InitializeRecognizer();
                        }
                    }
                }
                else if (targetLipSync.lipSyncMethod == ELipSyncMethod.Baked)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("targetAnimator"));

                    if (GUILayout.Button("LipSync Baker") == true)
                    {
                        BakingEditorWindow window = EditorWindow.GetWindow<BakingEditorWindow>("LipSync Baker");
                        window.Show();
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();

            
        }

    }

}