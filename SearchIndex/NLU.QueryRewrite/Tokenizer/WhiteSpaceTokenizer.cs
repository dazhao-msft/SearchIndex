//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.QueryRewrite.Tokenizer
{
    /// <summary>
    /// Tokenizes the text and returns sequences of non-whitespace characters as tokens.
    /// Note that any punctuation will be included in the returned tokens.
    /// </summary>
    internal class WhiteSpaceTokenizer : BaseTokenizer
    {
        public WhiteSpaceTokenizer(HashSet<string> whiteLists, IReadOnlyList<RegexToken> regTokens)
            : base(whiteLists, regTokens)
        {
        }

        public override List<QueryRewriteToken> QueryRewriteTokenize(string text)
        {
            var results = new List<QueryRewriteToken>();

            var charNormalizedText = CharNormalize(text);
            var start = 0;
            while (start < charNormalizedText.Length)
            {
                var next = SeekToNext(charNormalizedText, start, out var noAlter);

                if (next > start)
                {
                    var qrToken = new QueryRewriteToken(charNormalizedText.Substring(start, next - start), start, next - start);
                    if (noAlter)
                    {
                        qrToken.Tags |= QueryRewriteTags.NoAlter;
                    }

                    results.Add(qrToken);
                }

                start = next + 1;
            }

            return results;
        }

        private int SeekToNext(string input, int start, out bool noAlter)
        {
            noAlter = false;
            var nextInWhiteList = NextInWhiteList(input, start);
            if (nextInWhiteList >= 0)
            {
                return nextInWhiteList;
            }

            var nextInRegTokens = NextInRegexTokens(input, start, out noAlter);
            if (nextInRegTokens >= 0)
            {
                return nextInRegTokens;
            }

            for (var i = start; i <= input.Length; i++)
            {
                if (i >= input.Length || char.IsWhiteSpace(input[i]))
                {
                    return i;
                }
            }

            return input.Length;
        }
    }
}
