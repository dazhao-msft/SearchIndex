//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.QueryRewrite.Tokenizer
{
    internal class WordpieceTokenizer : BaseTokenizer
    {
        private readonly HashSet<string> _wordpieceVocabs;
        private readonly string _unknownVocab;
        private readonly int _maxCharsPerWord;
        private readonly WhiteSpaceTokenizer _whiteSpaceTokenizer;

        public WordpieceTokenizer(
            HashSet<string> wordpieceVocabs,
            string unknownVocab,
            int maxCharsPerWord,
            HashSet<string> whiteLists,
            IReadOnlyList<RegexToken> regTokens)
            : base(whiteLists, regTokens)
        {
            _wordpieceVocabs = wordpieceVocabs;
            _unknownVocab = unknownVocab;
            _maxCharsPerWord = maxCharsPerWord;
            _whiteSpaceTokenizer = new WhiteSpaceTokenizer(whiteLists, regTokens);
        }

        public override List<QueryRewriteToken> QueryRewriteTokenize(string text)
        {
            var inputTokens = _whiteSpaceTokenizer.QueryRewriteTokenize(text);
            var outputTokens = new List<QueryRewriteToken>();
            foreach (var token in inputTokens)
            {
                if (token.Length > _maxCharsPerWord)
                {
                    outputTokens.Add(new QueryRewriteToken()
                    {
                        Text = _unknownVocab,
                        Start = token.Start,
                        Length = token.Length,
                    });

                    continue;
                }

                var invalid = false;
                var start = 0;
                var subTokens = new List<QueryRewriteToken>();
                while (start < token.Length)
                {
                    var end = token.Length;
                    var subToken = new QueryRewriteToken()
                    {
                        Text = string.Empty,
                    };

                    while (start < end)
                    {
                        var length = end - start;
                        var subString = token.Text.Substring(start, length);
                        if (start > 0)
                        {
                            subString = "##" + subString;
                        }

                        if (_wordpieceVocabs.Contains(subString))
                        {
                            subToken.Text = subString;
                            subToken.Start = token.Start + start;
                            subToken.Length = length;

                            break;
                        }

                        end--;
                    }

                    if (string.IsNullOrEmpty(subToken.Text))
                    {
                        invalid = true;
                        break;
                    }

                    subTokens.Add(subToken);
                    start = end;
                }

                if (invalid)
                {
                    outputTokens.Add(new QueryRewriteToken()
                    {
                        Text = _unknownVocab,
                        Start = token.Start,
                        Length = token.Length,
                    });
                }
                else
                {
                    outputTokens.AddRange(subTokens);
                }
            }

            return outputTokens;
        }
    }
}
