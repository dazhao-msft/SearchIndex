//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    internal class DictCorrectSpeller : ISpeller
    {
        private readonly IDictionary<string, string> _correctionDict;
        private readonly ITokenizer _tokenizer;

        public DictCorrectSpeller(string spellerEntityFilePath, ITokenizer tokenizer)
        {
            if (File.Exists(spellerEntityFilePath))
            {
                _correctionDict = CommonFileLoader.LoadFileAsDic(spellerEntityFilePath, true);
            }
            else
            {
                _correctionDict = new Dictionary<string, string>();
            }

            _tokenizer = tokenizer;
        }

        public string SpellCorrect(string text)
        {
            var tokens = text.Split(' ');
            for (var i = 0; i < tokens.Length; i++)
            {
                int start = 0, end = tokens[i].Length;
                while (start < end && !char.IsLetter(tokens[i][start]))
                {
                    ++start;
                }

                // it means the whole token doesn't contain any alphabet.
                if (start >= end)
                {
                    continue;
                }

                var pre = start == 0 ? string.Empty : tokens[i].Substring(0, start);
                while (!char.IsLetter(tokens[i][end - 1]))
                {
                    --end;
                }

                var post = end == tokens[i].Length ? string.Empty : tokens[i].Substring(end);
                var token = tokens[i].Substring(start, end - start);
                if (_correctionDict.ContainsKey(token))
                {
                    tokens[i] = $"{pre}{_correctionDict[token]}{post}";
                }
            }

            return string.Join(" ", tokens);
        }

        public List<QueryRewriteToken> SpellCorrect(IReadOnlyList<QueryRewriteToken> inputTokens)
        {
            var correctedTokens = new List<QueryRewriteToken>();
            foreach (var token in inputTokens)
            {
                if (!token.Tags.HasFlag(QueryRewriteTags.NoAlter) && _correctionDict.TryGetValue(token.Text, out var tokenReplacement))
                {
                    var subTokens = _tokenizer.Tokenize(tokenReplacement);
                    foreach (var subToken in subTokens)
                    {
                        correctedTokens.Add(new QueryRewriteToken(subToken, token.Start, token.Length));
                    }
                }
                else
                {
                    correctedTokens.Add(token);
                }
            }

            return correctedTokens;
        }
    }
}
