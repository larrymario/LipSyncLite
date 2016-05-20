using UnityEngine;
using System;
using System.Collections.Generic;

namespace LipSyncLite 
{
    public class MathToolBox 
    {
        /// <summary>
        /// Discrete Cosine Transform.
        /// </summary>
        /// <param name="data">Source data.</param>
        /// <returns></returns>
        public static float[] DiscreteCosineTransform(float[] data)
        {
            float[] result = new float[data.Length];

            float sumCos;
            for (int m = 0; m < data.Length; ++m)
            {
                sumCos = 0.0f;
                for (int k = 0; k < data.Length; ++k)
                {
                    sumCos += data[k] * Mathf.Cos((Mathf.PI / data.Length) * m * (k + 0.5f));
                }
                result[m] = (sumCos > 0) ? sumCos : -sumCos;
            }

            return result;
        }

        /// <summary>
        /// Convolute data and filter. Result is sent to output, which must not be shorter than data.
        /// </summary>
        /// <param name="output">Array to store output. Must not be shorter than data.</param>
        /// <param name="data">Source data array.</param>
        /// <param name="filter">Filter array.</param>
        /// <param name="paddleType">Paddle type.</param>
        public static void Convolute(float[] data, float[] filter, EPaddleType paddleType, float[] output)
        {
            int filterMiddlePoint = Mathf.FloorToInt(filter.Length / 2);
            for (int n = 0; n < data.Length; ++n)
            {
                output[n] = 0.0f;
                for (int m = 0; m < filter.Length; ++m)
                {
                    output[n] +=
                        MathToolBox.GetValueFromArray(data, n - filterMiddlePoint + m, paddleType) *
                        filter[filter.Length - m - 1];
                }
            }
        }

        /// <summary>
        /// Find (length of peakvalue) local largest peak(s). 
        /// </summary>
        /// <param name="data">Source data.</param>
        /// <param name="peakValue">Array to store peak values.</param>
        /// <param name="peakPosition">Array to store peak values' positions.</param>
        public static void FindLocalLargestPeaks(float[] data, float[] peakValue, int[] peakPosition)
        {
            int peakNum = 0;
            float lastPeak = 0.0f;
            int lastPeakPosition = 0;
            bool isIncreasing = false;
            bool isPeakIncreasing = false;

            for (int i = 0; i < data.Length - 1; ++i)
            {
                if (data[i] < data[i + 1])
                {
                    isIncreasing = true;
                }
                else
                {
                    if (isIncreasing == true)
                    {
                        // Peak found.
                        if (lastPeak < data[i])
                        {
                            isPeakIncreasing = true;
                        }
                        else
                        {
                            if (isPeakIncreasing == true)
                            {
                                // Local largest peak found.
                                peakValue[peakNum] = lastPeak;
                                peakPosition[peakNum] = lastPeakPosition;
                                ++peakNum;
                            }

                            isPeakIncreasing = false;
                        }
                        
                        lastPeak = data[i];
                        lastPeakPosition = i;
                    }
                    isIncreasing = false;
                }
                if (peakNum >= peakValue.Length)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Generate a Gaussian filter.
        /// Make sure not to call this function too frequently. Recommend caching the result.
        /// </summary>
        /// <param name="size">Size of the filter.</param>
        /// <param name="deviationSquare">Deviation's square.</param>
        /// <returns></returns>
        public static float[] GenerateGaussianFilter(int size, float deviationSquare)
        {
            float[] result = new float[size];

            float sum = 0.0f;
            float middlePoint = (float)(size - 1) / 2;
            for (int i = 0; i < size; ++i)
            {
                float param = -((i - middlePoint) * (i - middlePoint)) / (2 * deviationSquare);
                result[i] = Mathf.Exp(param);
                sum += result[i];
            }

            for (int j = 0; j < size; ++j)
            {
                result[j] /= sum;
            }

            return result;
        }

        public static float[] GenerateWindow(int size, EWindowType windowType)
        {
            float[] result = new float[size];

            switch (windowType)
            {
                case EWindowType.Hamming:
                    for (int i = 0; i < size; ++i)
                    {
                        result[i] = 0.53836f - 0.46164f * Mathf.Cos((2 * Mathf.PI * i) / (size - 1));
                    }
                    break;
            }

            return result;
        }

        public static float GetValueFromArray(float[] data, int index, EPaddleType paddleType)
        {
            if (index >= 0 && index < data.Length)
            {
                return data[index];
            }
            else 
            { 
                switch (paddleType)
                {
                    case EPaddleType.Zero:
                        return 0;

                    case EPaddleType.Repeat:
                        if (index < 0)
                        {
                            return data[0];
                        }
                        else
                        {
                            return data[data.Length - 1];
                        }

                    case EPaddleType.Loop:
                        int actualIndex = index;
                        while (actualIndex < 0)
                        {
                            actualIndex += data.Length;
                        }
                        actualIndex %= data.Length;
                        return data[actualIndex];

                    default:
                        return 0;
                }
            }
        }

        public enum EPaddleType
        {
            /// <summary>
            /// Paddle with zeros.
            /// </summary>
            Zero = 0,
            /// <summary>
            /// Repeat the first value on the left and the last value on the right.
            /// </summary>
            Repeat = 1,
            /// <summary>
            /// Loop the array.
            /// </summary>
            Loop = 2
        }

        public enum EWindowType 
        {
            Rectangular,
            Triangle,
            Hamming,
            Hanning,
            BlackMan,
            BlackmanHarris
        }
    }

}

