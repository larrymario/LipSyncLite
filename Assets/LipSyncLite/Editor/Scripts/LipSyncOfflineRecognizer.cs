using UnityEngine;

namespace LipSyncLite
{

    public class LipSyncOfflineRecognizer
    {
        private const int FILTER_SIZE = 7;
        private const float FILTER_DEVIATION_SQUARE = 5.0f;
        private const int FORMANT_COUNT = 1;

        private ERecognizerLanguage recognizingLanguage;

        private int windowSize;
        private int shiftStepSize;
        private float amplitudeThreshold;
        private float[] gaussianFilter;
        private float[] windowArray;

        private float amplitudeSum;
        private float[] smoothedAudioSpectrum;
        private float[] peakValues;
        private int[] peakPositions;

        private float frequencyUnit;
        private float[] formantArray;

        private string[] currentVowels;
        private float[] currentVowelFormantCeilValues;

        // TODO: Data-lization
        private string[] vowelsByFormantJP = { "i", "u", "e", "o", "a" };
        private float[] vowelFormantFloorJP = { 0.0f, 250.0f, 300.0f, 450.0f, 600.0f };
        private string[] vowelsByFormantCN = { "i", "v", "u", "e", "o", "a" };
        private float[] vowelFormantFloorCN = { 0.0f, 100.0f, 250.0f, 300.0f, 450.0f, 600.0f };

        public LipSyncOfflineRecognizer(ERecognizerLanguage recognizingLanguage, float amplitudeThreshold, int windowSize, int shiftStepSize)
        {
            this.recognizingLanguage = recognizingLanguage;
            this.windowSize = Mathf.ClosestPowerOfTwo(windowSize);
            this.shiftStepSize = shiftStepSize;
            
            this.amplitudeThreshold = amplitudeThreshold;
            this.gaussianFilter = MathToolBox.GenerateGaussianFilter(FILTER_SIZE, FILTER_DEVIATION_SQUARE);
            this.windowArray = MathToolBox.GenerateWindow(windowSize, MathToolBox.EWindowType.Hamming);

            this.smoothedAudioSpectrum = new float[this.windowSize];
            this.peakValues = new float[FORMANT_COUNT];
            this.peakPositions = new int[FORMANT_COUNT];
            this.formantArray = new float[FORMANT_COUNT];
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioClip"></param>
        /// <returns></returns>
        public string[] RecognizeAllByAudioClip(AudioClip audioClip)
        {
            int recognizeSampleCount = Mathf.CeilToInt((float)(audioClip.samples) / (float)(shiftStepSize));
            string[] result = new string[recognizeSampleCount];

            float[] currentAudioData = new float[this.windowSize];
            float[] currentAudioSpectrum = new float[this.windowSize];

            for (int i = 0; i < recognizeSampleCount; ++i)
            {
                audioClip.GetData(currentAudioData, i * shiftStepSize);
                for (int j = 0; j < windowSize; ++j)
                {
                    currentAudioData[j] *= windowArray[j]; 
                }
                currentAudioSpectrum = MathToolBox.DiscreteCosineTransform(currentAudioData);

                amplitudeSum = 0.0f;
                for (int k = 0; k < windowSize; ++k)
                {
                    amplitudeSum += currentAudioSpectrum[k];
                }

                if (amplitudeSum >= amplitudeThreshold)
                {
                    MathToolBox.Convolute(currentAudioSpectrum, gaussianFilter, MathToolBox.EPaddleType.Repeat, smoothedAudioSpectrum);
                    MathToolBox.FindLocalLargestPeaks(smoothedAudioSpectrum, peakValues, peakPositions);
                    frequencyUnit = audioClip.frequency / 2 / windowSize;
                    for (int l = 0; l < formantArray.Length; ++l)
                    {
                        formantArray[l] = peakPositions[l] * frequencyUnit;
                    }

                    switch (recognizingLanguage)
                    {
                        case ERecognizerLanguage.Japanese:
                            currentVowels = vowelsByFormantJP;
                            currentVowelFormantCeilValues = vowelFormantFloorJP;
                            break;
                        case ERecognizerLanguage.Chinese:
                            currentVowels = vowelsByFormantCN;
                            currentVowelFormantCeilValues = vowelFormantFloorCN;
                            break;
                    }
                    for (int m = 0; m < currentVowelFormantCeilValues.Length; ++m)
                    {
                        if (formantArray[0] > currentVowelFormantCeilValues[m])
                        {
                            result[i] = currentVowels[m];
                        }
                    }
                }
                else
                {
                    result[i] = null;
                }
            }

            return result;
        }

    }

}
