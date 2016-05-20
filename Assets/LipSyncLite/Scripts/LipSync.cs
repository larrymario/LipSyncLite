using UnityEngine;
using System.Collections.Generic;

namespace LipSyncLite
{
    public class LipSync : MonoBehaviour 
    {
        #region Fields
        public static string[] vowelsJP = { "a", "i", "u", "e", "o" };
        public static string[] vowelsCN = { "a", "e", "i", "o", "u", "v" };

        public const int MAX_BLEND_VALUE_COUNT = 6;

        public ELipSyncMethod lipSyncMethod;
        public AudioSource audioSource;

        #region Fields for Runtime LipSync
        public ERecognizerLanguage recognizerLanguage;
        public ETargetType targetType;
        #region Fields for BlendShape
        public SkinnedMeshRenderer targetBlendShapeObject;
        public string[] propertyNames = new string[MAX_BLEND_VALUE_COUNT];
        public float propertyMinValue = 0.0f;
        public float propertyMaxValue = 100.0f;
        #endregion
        #region Fields for Live2D
        public LipSyncLive2DHelper targetLive2DHelper;
        public string paramXName;
        public string paramYName;
        public Vector2[] paramMaxValues = new Vector2[MAX_BLEND_VALUE_COUNT];
        #endregion


        public int windowSize = 1024;
        public float amplitudeThreshold = 0.01f;
        public float moveTowardsSpeed = 8;

        private LipSyncRuntimeRecognizer runtimeRecognizer;
        private string[] currentVowels;
        private Dictionary<string, int> vowelToIndexDict = new Dictionary<string, int>();
        private int[] propertyIndexs = new int[MAX_BLEND_VALUE_COUNT];
        private float blendValuesSum;

        private string recognizeResult;
        private float[] targetBlendValues = new float[MAX_BLEND_VALUE_COUNT];
        private float[] currentBlendValues = new float[MAX_BLEND_VALUE_COUNT];
        #region Fields for Live2D
        private float currentPropertyXValue;
        private float currentPropertyYValue;
        #endregion
        #endregion

        #region Fields for Baked LipSync
        public Animator targetAnimator;

        private int lastTimeSamples;
        #endregion
        #endregion

        public string[] CurrentVowels
        {
            get
            {
                return currentVowels;
            }
        }

        public void InitializeRecognizer()
        {
            switch (recognizerLanguage)
            {
                case ERecognizerLanguage.Japanese:
                    currentVowels = vowelsJP;
                    break;
                case ERecognizerLanguage.Chinese:
                    currentVowels = vowelsCN;
                    break;
            }
            for (int i = 0; i < currentVowels.Length; ++i)
            {
                vowelToIndexDict[currentVowels[i]] = i;
                propertyIndexs[i] = targetBlendShapeObject.sharedMesh.GetBlendShapeIndex(propertyNames[i]);
            }
            runtimeRecognizer = new LipSyncRuntimeRecognizer(recognizerLanguage, windowSize, amplitudeThreshold);
        }

        void Start()
        {
            InitializeRecognizer();
            
        }

        void Update()
        {
            if (lipSyncMethod == ELipSyncMethod.Runtime)
            {
                recognizeResult = runtimeRecognizer.RecognizeByAudioSource(audioSource);
                for (int i = 0; i < targetBlendValues.Length; ++i)
                {
                    targetBlendValues[i] = 0.0f;
                }
                currentPropertyXValue = 0;
                currentPropertyYValue = 0;

                if (recognizeResult != null)
                {
                    targetBlendValues[vowelToIndexDict[recognizeResult]] = 1.0f;
                }

                blendValuesSum = 0.0f;
                for (int j = 0; j < currentBlendValues.Length; ++j)
                {
                    blendValuesSum += currentBlendValues[j];
                }

                for (int k = 0; k < currentBlendValues.Length; ++k)
                {
                    if (propertyIndexs[k] != -1)
                    {
                        currentBlendValues[k] = Mathf.MoveTowards(
                            currentBlendValues[k],
                            targetBlendValues[k],
                            moveTowardsSpeed * Time.deltaTime);
                        if (targetType == ETargetType.BlendShape)
                        {
                            targetBlendShapeObject.SetBlendShapeWeight(
                                propertyIndexs[k], 
                                Mathf.Lerp(propertyMinValue, propertyMaxValue, currentBlendValues[k]));
                        }
                        else if (targetType == ETargetType.Live2D)
                        {
                            currentPropertyXValue += Mathf.Lerp(0, paramMaxValues[k].x, currentBlendValues[k]);
                            currentPropertyYValue += Mathf.Lerp(0, paramMaxValues[k].y, currentBlendValues[k]);
                        }
                    }
                }
                if (targetType == ETargetType.Live2D)
                {
                    targetLive2DHelper.SetParamFloat(
                        paramXName,
                        currentPropertyXValue);
                    targetLive2DHelper.SetParamFloat(
                        paramYName,
                        currentPropertyYValue);
                }
            }
            else if (lipSyncMethod == ELipSyncMethod.Baked)
            {
                if (audioSource.timeSamples < lastTimeSamples)
                {
                    if (audioSource.isPlaying == true)
                    {
                        targetAnimator.CrossFade(audioSource.clip.name + "_anim", 0f);
                    }
                }
                lastTimeSamples = audioSource.timeSamples;
            }
            
        }

        void OnValidate()
        {
            windowSize = Mathf.ClosestPowerOfTwo(Mathf.Clamp(windowSize, 32, 8192));
            amplitudeThreshold = Mathf.Max(0, amplitudeThreshold);
            moveTowardsSpeed = Mathf.Clamp(moveTowardsSpeed, 5, 25);
        }
    }

    public enum ELipSyncMethod
    {
        Runtime,
        Baked
    }

    public enum ETargetType
    {
        BlendShape,
        Live2D
    }
}
