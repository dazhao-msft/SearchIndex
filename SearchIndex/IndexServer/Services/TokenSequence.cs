using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexServer.Services
{
    /// <summary>
    /// Splits the given string to an array of tokens with whitespace as separator.
    /// </summary>
    public sealed class TokenSequence
    {
        private readonly string _value;
        private readonly Token[] _tokens;

        public TokenSequence(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));

            var valueAsSpan = value.AsSpan();

            var tokens = new List<Token>();

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

                tokens.Add(new Token(valueAsSpan.Slice(firstOffset, secondOffset - firstOffset).ToString(), firstOffset));

                firstOffset = secondOffset;
            }

            _tokens = tokens.ToArray();
        }

        public string AsValue() => _value;

        public string[] GetTokens() => _tokens.Select(token => token.Value).ToArray();

        public string FindLcs(TokenSequence other, StringComparer valueComparer)
        {
            var lcsTokens = LcsAlgorithm.FindLcs(_tokens, other._tokens, new TokenValueEqualityComparer(valueComparer));

            if (lcsTokens.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                return _value.Substring(lcsTokens[0].Offset, lcsTokens[^1].Offset + lcsTokens[^1].Value.Length - lcsTokens[0].Offset);
            }
        }

        #region private token helpers

        private struct Token
        {
            public Token(string value, int offset)
            {
                Value = value;
                Offset = offset;
            }

            public string Value { get; }

            public int Offset { get; }
        }

        private sealed class TokenValueEqualityComparer : IEqualityComparer<Token>
        {
            private readonly StringComparer _valueComparer;

            public TokenValueEqualityComparer(StringComparer valueComparer) => _valueComparer = valueComparer;

            public bool Equals(Token x, Token y) => _valueComparer.Equals(x.Value, y.Value);

            public int GetHashCode(Token obj) => _valueComparer.GetHashCode(obj);
        }

        #endregion private token helpers
    }
}
