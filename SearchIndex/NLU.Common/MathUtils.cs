//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;

namespace Microsoft.BizQA.NLU.Common
{
    public static class MathUtils
    {
        /// <summary>
        /// float margin
        /// </summary>
        public const float FloatMargin = 1.0E-5f;

        /// <summary>
        /// Normalize a float vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float[] NormalizeVector(float[] vector)
        {
            var sum = 0.0;
            for (int i = 0; i < vector.Length; i++)
            {
                sum += Math.Pow(vector[i], 2.0);
            }

            var norm = (float)Math.Sqrt(sum);
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] /= norm;
            }

            return vector;
        }

        /// <summary>
        /// Compute dot product between two float vectors.
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static double DotProduct(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                throw new ArgumentException("vector dimension doesn't match.");
            }

            var score = 0.0;
            for (int i = 0; i < vector1.Length; i++)
            {
                score += vector1[i] * vector2[i];
            }

            return score;
        }

        /// <summary>
        /// Determine if two float vectors are equal.
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static bool Equal(float[] vector1, float[] vector2)
        {
            if (vector1?.Length != vector2?.Length)
            {
                return false;
            }

            for (int i = 0; i < vector1.Length; i++)
            {
                if (!Equal(vector1[i], vector2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reference: http://floating-point-gui.de/errors/comparison/
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon">The error margin.</param>
        /// <returns></returns>
        public static bool Equal(float a, float b, float epsilon = 0.00001f)
        {
            if (a == b)
            {
                return true;
            }

            const float floatNormal = (1 << 23) * float.Epsilon;
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == 0.0f || b == 0.0f || diff < floatNormal)
            {
                return diff < (epsilon * floatNormal);
            }

            return (diff / Math.Min(absA + absB, float.MaxValue)) < epsilon;
        }
    }
}
