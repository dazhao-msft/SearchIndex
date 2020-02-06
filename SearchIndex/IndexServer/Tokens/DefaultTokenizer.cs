using System;
using System.Collections.Generic;

namespace IndexServer.Tokens
{
    /// <summary>
    /// Splits the given string into tokens using whitespace as separator.
    /// </summary>
    public sealed class DefaultTokenizer : ITokenizer
    {
        public IEnumerable<(string, int)> Tokenize(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueAsMemory = value.AsMemory();

            int leftOffset = 0;

            while (true)
            {
                while (leftOffset < valueAsMemory.Length && valueAsMemory.Span[leftOffset] == ' ')
                {
                    leftOffset++;
                }

                if (leftOffset == valueAsMemory.Length)
                {
                    yield break;
                }

                int rightOffset = leftOffset + 1;

                while (rightOffset < valueAsMemory.Length && valueAsMemory.Span[rightOffset] != ' ')
                {
                    rightOffset++;
                }

                yield return (valueAsMemory.Slice(leftOffset, rightOffset - leftOffset).ToString(), leftOffset);

                leftOffset = rightOffset;
            }
        }
    }
}
