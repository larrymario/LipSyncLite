using UnityEngine;
using System.Collections;

namespace LipSyncLite
{
    public class LipSyncRuntimeRecognizer
    {
        private const int FILTER_SIZE = 7;
        private const float FILTER_DEVIATION_SQUARE = 5.0f;
        private const int FORMANT_COUNT = 1;

        private ERecognizerLanguage recognizingLanguage;

        private int windowSize;
        private float amplitudeThreshold;
        private float[] playingAudioData;
        private float[] playingAudioSpectrum;
        private float[] gaussianFilter;
    
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

        public LipSyncRuntimeRecognizer(ERecognizerLanguage recognizingLanguage, int windowSize, float amplitudeThreshold)
        {
            this.recognizingLanguage = recognizingLanguage;
            this.windowSize = Mathf.ClosestPowerOfTwo(windowSize);
            this.playingAudioData = new float[this.windowSize];
            this.playingAudioSpectrum = new float[this.windowSize];
            this.amplitudeThreshold = amplitudeThreshold;
            this.gaussianFilter = MathToolBox.GenerateGaussianFilter(FILTER_SIZE, FILTER_DEVIATION_SQUARE);

            this.smoothedAudioSpectrum = new float[this.windowSize];
            this.peakValues = new float[FORMANT_COUNT];
            this.peakPositions = new int[FORMANT_COUNT];
            this.formantArray = new float[FORMANT_COUNT];
            
        }

        public string RecognizeByAudioSource(AudioSource audioSource)
        {
            string result = null;

            audioSource.GetOutputData(playingAudioData, 0);
            audioSource.GetSpectrumData(playingAudioSpectrum, 0, FFTWindow.BlackmanHarris);

            if (audioSource.isPlaying == true)
            {
                amplitudeSum = 0.0f;
                for (int i = 0; i < playingAudioSpectrum.Length; ++i)
                {
                    amplitudeSum += playingAudioSpectrum[i];
                }

                if (amplitudeSum >= amplitudeThreshold)
                {
                    MathToolBox.Convolute(playingAudioSpectrum, gaussianFilter, MathToolBox.EPaddleType.Repeat, smoothedAudioSpectrum);
                    MathToolBox.FindLocalLargestPeaks(smoothedAudioSpectrum, peakValues, peakPositions);
                    frequencyUnit = audioSource.clip.frequency / 2 / windowSize;
                    for (int i = 0; i < formantArray.Length; ++i)
                    {
                        formantArray[i] = peakPositions[i] * frequencyUnit;
                    }
                    
                    // TODO: Recognization by multiple formant
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
                    for (int i = 0; i < currentVowelFormantCeilValues.Length; ++i)
                    {
                        if (formantArray[0] > currentVowelFormantCeilValues[i])
                        {
                            result = currentVowels[i];
                        }
                    }
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

    }

    public enum ERecognizerLanguage
    {
        Japanese,
        Chinese
    }
}