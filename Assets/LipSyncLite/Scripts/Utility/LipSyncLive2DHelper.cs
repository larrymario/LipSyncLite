using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

namespace LipSyncLite
{
    public class LipSyncLive2DHelper : MonoBehaviour
    {
        public const int MAX_BLEND_VALUE_COUNT = 6;

        public string componentTypeName;
        public string live2DModelFieldName;

        private Type targetType;
        private Component targetComponent;

        private Type typeOfLive2DModelUnity;
        private MethodInfo methodSetParamFloat;

        private FieldInfo targetField;
        private object targetLive2DModel;

        public void SetParamFloat(string paramId, float value)
        {
            methodSetParamFloat.Invoke(targetLive2DModel, new object[] { paramId, value });
        }

        void Start()
        {
            targetType = Type.GetType(componentTypeName);
            if (targetType == null)
            {
                Debug.LogError("[LipSyncLive2DHelper] Type not found. Make sure you have specified the correct type name, including its namespace.");
                return;
            }
            targetComponent = GetComponent(targetType);

            targetField = targetType.GetField(live2DModelFieldName, BindingFlags.Public | BindingFlags.Instance);
            if (targetField == null)
            {
                targetField = targetType.GetField(live2DModelFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (targetField == null)
                {
                    Debug.LogError("[LipSyncLive2DHelper] Field not found. Make sure you have specified the correct field name.");
                    return;
                }
            }

            Assembly live2dAssembly = Assembly.Load(new AssemblyName("Live2DUnity"));
            if (live2dAssembly == null)
            {
                Debug.LogError("[LipSyncLive2DHelper] Live2D libraries not found. Make sure you have imported them and it's named \'Live2DUnity\', since I can't do that for you...");
                return;
            }
            else
            {
                typeOfLive2DModelUnity = live2dAssembly.GetType("live2d.Live2DModelUnity");
                if (typeOfLive2DModelUnity == null)
                {
                    Debug.LogError("[LipSyncLive2DHelper] Live2DModelUnity Type not found. Make sure you have imported Live2D-related libraries, since I can't do that for you...");
                    return;
                }
                else
                {
                    methodSetParamFloat = typeOfLive2DModelUnity.GetMethod("setParamFloat", new Type[] { typeof(string), typeof(float) });
                    if (methodSetParamFloat == null)
                    {
                        Debug.LogError("[LipSyncLive2DHelper] Something goes wrong with the Live2D libraries. Make sure it's a valid one.");
                        return;
                    }
                }

            }



            StartCoroutine(DelayGetLive2DModel());
            
        }

        IEnumerator DelayGetLive2DModel()
        {
            yield return null;
            targetLive2DModel = targetField.GetValue(targetComponent);
        }
    }

}

