using System;
using System.Collections.Generic;

namespace IndexServer.Services
{
    public struct Token
    {
        public Token(string value, int offset)
        {
            Value = value;
            Offset = offset;
        }

        public string Value { get; }

        public int Offset { get; }
    }

    public sealed class TokenValueEqualityComparer : IEqualityComparer<Token>
    {
        public static readonly TokenValueEqualityComparer Ordinal = new TokenValueEqualityComparer(StringComparer.Ordinal);

        public static readonly TokenValueEqualityComparer OrdinalIgnoreCase = new TokenValueEqualityComparer(StringComparer.OrdinalIgnoreCase);

        private readonly StringComparer _valueComparer;

        public TokenValueEqualityComparer(StringComparer valueComparer) => _valueComparer = valueComparer;

        public bool Equals(Token x, Token y) => _valueComparer.Equals(x.Value, y.Value);

        public int GetHashCode(Token obj) => _valueComparer.GetHashCode(obj);
    }

    public static class TokenUtilities
    {
        /// <summary>
        /// Splits the given string to an array of tokens with whitespace as separator.
        /// </summary>
        public static Token[] ToTokenArray(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueAsSpan = value.AsSpan();

            var result = new List<Token>();

            int firstOffset = 0;

            while (true)
            {
                while (firstOffset < valueAsSpan.Length && valueAsSpan[firstOffset] == ' ')
                {
                    firstOffset++;
                }

                if (firstOffset == valueAsSpan.Length)
                {
                    break;
                }

                int secondOffset = firstOffset + 1;

                while (secondOffset < valueAsSpan.Length && valueAsSpan[secondOffset] != ' ')
                {
                    secondOffset++;
                }

                result.Add(new Token(valueAsSpan.Slice(firstOffset, secondOffset - firstOffset).ToString(), firstOffset));

                firstOffset = secondOffset;
            }

            return result.ToArray();
        }

        /// <summary>
        /// Finds the longest common consecutive subsequence between the two given sequences.
        /// </summary>
        /// <returns>
        /// It returns the subsequence as a span of the first argument.
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
