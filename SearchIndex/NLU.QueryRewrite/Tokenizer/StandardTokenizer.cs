//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.QueryRewrite.Tokenizer
{
    /// <summary>
    /// This <c>tokenizer</c> splits the input text into tokens, treating whitespace and punctuation as delimiters.
    /// Delimiter characters are discarded, with the following exceptions:
    ///  - Periods(dots) that are not followed by whitespace are kept as part of the token, including Internet domain names.
    ///  - The "@" character is among the set of token-splitting punctuation, so email addresses are not preserved as single tokens.
    /// Note that words are split at hyphens.
    /// After do the <c>tokenizer</c>, the '_', "'" and '$' will be kept, for example: "xbox, 06_2e_47" will be formatted to "xbox 06_2e_47".
    /// </summary>
    internal class StandardTokenizer : BaseTokenizer
    {
        public StandardTokenizer(HashSet<string> tokenWhiteList, IReadOnlyList<RegexToken> regexTokens)
            : base(tokenWhiteList, regexTokens)
        {
        }

        public override List<QueryRewriteToken> QueryRewriteTokenize(string text)
        {
            var results = new List<QueryRewriteToken>();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var charNormalizedText = CharNormalize(text);
                var start = 0;
                while (start < charNormalizedText.Length)
                {
                    var next = SeekToNext(charNormalizedText, start, out var noAlter);

                    if (next > start)
                    {
                        var tokenText = charNormalizedText.Substring(start, next - start);
                        var qrToken = new QueryRewriteToken(tokenText, start, tokenText.Length);
                        if (noAlter)
                        {
                            qrToken.Tags |= QueryRewriteTags.NoAlter;
                        }

                        results.Add(qrToken);
                    }

                    start = next + 1;
                }
            }

            return results;
        }

        private static bool IsPunctuation(char ch)
        {
            return ch != '_' && ch != '$' && char.IsPunctuation(ch);
        }

        private int SeekToNext(string input, int start, out bool noAlter)
        {
            noAlter = false;
            var nextInWhiteList = NextInWhiteList(input, start);
            if (nextInWhiteList >= 0)
            {
                return nextInWhiteList;
            }

            var nextInRegexTokens = NextInRegexTokens(input, start, out noAlter);
            if (nextInRegexTokens >= 0)
            {
                return nextInRegexTokens;
            }

            for (var i = start; i < input.Length; i++)
            {
                if (char.IsWhiteSpace(input[i]) || IsPunctuation(input[i]))
                {
                    return i;
                }
            }

            return input.Length;
        }

        private static bool IsSeparator(string input, int position)
        {
            return position >= input.Length || char.IsWhiteSpace(input[position]) || IsPunctuation(input[position]);
        }

        private static bool IsChar(string input, int position, char ch)
        {
            return position < input.Length && input[position] == ch;
        }
    }
}
