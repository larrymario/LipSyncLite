using UnityEngine;
using System.Collections.Generic;

namespace LipSyncLite
{
    public class LipSyncSwitcher : MonoBehaviour 
    {
        public AudioSource audioSource;
        public LipSync lipSync;
        public SkinnedMeshRenderer[] targets;
        public float[] keyTimePoints;
        public int[] targetIndexs;

        private int currentProgress;
        private int lastProgress;

        void Start()
        {
            currentProgress = 0;
        }

        void Update()
        {
            if ((audioSource != null) && (lipSync != null))
            {
                if (currentProgress < keyTimePoints.Length - 1)
                {
                    while (audioSource.time > keyTimePoints[currentProgress])
                    {
                        ++currentProgress;
                    }
                }
                if (currentProgress > 1)
                {
                    while (audioSource.time < keyTimePoints[currentProgress - 1])
                    {
                        --currentProgress;
                    }
                }

                lipSync.targetBlendShapeObject = targets[targetIndexs[currentProgress]];
            }
        }

        void OnValidate()
        {
            if (keyTimePoints.Length > 0)
            {
                keyTimePoints[0] = Mathf.Max(keyTimePoints[0], 0);
                for (int i = 0; i < targetIndexs.Length - 1; ++i)
                {
                    keyTimePoints[i] = Mathf.Min(keyTimePoints[i], keyTimePoints[i + 1]);
                }
            }

            for (int j = 0; j < targets.Length; ++j)
            {
                targetIndexs[j] = Mathf.Clamp(targetIndexs[j], 0, targetIndexs.Length - 1);
            }
        }
    }
}
