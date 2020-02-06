using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexServer.Services
{
    public sealed class DefaultTokenizer : ITokenizer
    {
        private static readonly IReadOnlyList<char> Separators = new[] { ' ', '.', ',', '?', '!', '"', };

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
                while (leftOffset < valueAsMemory.Length && Separators.Any(p => p == valueAsMemory.Span[leftOffset]))
                {
                    leftOffset++;
                }

                if (leftOffset == valueAsMemory.Length)
                {
                    yield break;
                }

                int rightOffset = leftOffset + 1;

                while (rightOffset < valueAsMemory.Length && !Separators.Any(p => p == valueAsMemory.Span[rightOffset]))
                {
                    rightOffset++;
                }

                yield return (valueAsMemory.Slice(leftOffset, rightOffset - leftOffset).ToString(), leftOffset);

                leftOffset = rightOffset;
            }
        }
    }
}
