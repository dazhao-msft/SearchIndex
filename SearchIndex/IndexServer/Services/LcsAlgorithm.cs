using System;
using System.Collections.Generic;

namespace IndexServer.Services
{
    public static class LcsAlgorithm
    {
        /// <summary>
        /// Finds the longest common and consecutive subsequence between the two given sequences.
        /// </summary>
        /// <returns>
        /// It returns the subsequence as a sliced span of the first argument.
        /// </returns>
        /// <remarks>
        /// When <typeparamref name="T"/> is char, it becomes the longest common substring problem.
        /// </remarks>
        public static ReadOnlySpan<T> FindLcs<T>(ReadOnlySpan<T> first, ReadOnlySpan<T> second, IEqualityComparer<T> equalityComparer)
        {
            int[,] map = new int[first.Length + 1, second.Length + 1];

            for (int i = 1; i <= first.Length; i++)
            {
                for (int j = 1; j <= second.Length; j++)
                {
                    if (equalityComparer.Equals(first[i - 1], second[j - 1]))
                    {
                        map[i, j] = map[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        map[i, j] = 0;
                    }
                }
            }

            int length = -1;
            int index = -1;

            for (int i = 0; i <= first.Length; i++)
            {
                for (int j = 0; j <= second.Length; j++)
                {
                    if (length < map[i, j])
                    {
                        length = map[i, j];
                        index = i;
                    }
                }
            }

            return first.Slice(index - length, length);
        }
    }
}
