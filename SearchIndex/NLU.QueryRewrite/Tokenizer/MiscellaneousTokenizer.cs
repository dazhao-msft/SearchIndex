//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.BizQA.NLU.QueryRewrite.Tokenizer
{
    internal class MiscellaneousTokenizer : BaseTokenizer
    {
        private readonly WhiteSpaceTokenizer _whiteSpaceTokenizer;

        public MiscellaneousTokenizer(HashSet<string> whiteLists, IReadOnlyList<RegexToken> regTokens)
            : base(whiteLists, regTokens)
        {
            _whiteSpaceTokenizer = new WhiteSpaceTokenizer(whiteLists, regTokens);
        }

        public override List<QueryRewriteToken> QueryRewriteTokenize(string text)
        {
            var inputTokens = _whiteSpaceTokenizer.QueryRewriteTokenize(text);
            var outputTokens = new List<QueryRewriteToken>();
            foreach (var inputToken in inputTokens)
            {
                var outputToken = new QueryRewriteToken()
                {
                    Text = inputToken.Text,
                    Start = inputToken.Start,
                    Length = inputToken.Length,
                };

                var outputSubTokens = SplitByPunctuation(outputToken);
                outputTokens.AddRange(outputSubTokens);
            }

            return outputTokens;
        }

        private List<QueryRewriteToken> SplitByPunctuation(QueryRewriteToken token)
        {
            var charArray = token.Text.ToCharArray();
            var subTokens = new List<QueryRewriteToken>();
            var subTokenChars = new List<char>();
            var i = 0;
            while (i < charArray.Length)
            {
                var ch = charArray[i];

                if (char.IsPunctuation(ch))
                {
                    if (subTokenChars.Any())
                    {
                        var subTokenLength = subTokenChars.Count;
                        subTokens.Add(new QueryRewriteToken()
                        {
                            Text = new string(subTokenChars.ToArray()),
                            Start = token.Start + i - subTokenLength,
                            Length = subTokenLength,
                        });

                        subTokenChars.Clear();
                    }

                    subTokens.Add(new QueryRewriteToken()
                    {
                        Text = char.ToString(ch),
                        Start = token.Start + i,
                        Length = 1,
                    });
                }
                else
                {
                    subTokenChars.Add(ch);
                }

                i++;
            }

            if (subTokenChars.Any())
            {
                var subTokenLength = subTokenChars.Count;
                subTokens.Add(new QueryRewriteToken()
                {
                    Text = new string(subTokenChars.ToArray()),
                    Start = token.Start + i - subTokenLength,
                    Length = subTokenLength,
                });

                subTokenChars.Clear();
            }

            return subTokens;
        }
    }
}
