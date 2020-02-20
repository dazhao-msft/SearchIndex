//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.BizQA.NLU.QueryRewrite.Tokenizer
{
    internal sealed class BertTokenizer : ITokenizer
    {
        private readonly MiscellaneousTokenizer _miscellaneousTokenizer;
        private readonly WordpieceTokenizer _wordpieceTokenizer;

        public BertTokenizer(QueryRewriteTokenizerOptions options)
        {
            var unknownVocab = options?.UnknownVocab ?? "[UNK]";
            var maxCharsPerWord = options?.MaxCharsPerWord ?? 200;
            var wordpieceVocabs = new HashSet<string>(StringComparer.Ordinal);
            var whiteList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var regexTokens = new List<RegexToken>();

            if (File.Exists(options?.WordpieceVocabFilePath))
            {
                wordpieceVocabs.UnionWith(CommonFileLoader.LoadFileAsList(options.WordpieceVocabFilePath));
            }

            if (File.Exists(options?.TokenizeWhiteListFilePath))
            {
                whiteList = CommonFileLoader.LoadFileAsHashSet(options.TokenizeWhiteListFilePath);
            }

            if (File.Exists(options?.RegexTokensFilePath))
            {
                using (var reader = new StreamReader(options.RegexTokensFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var cols = line.Split('\t');
                        if (cols.Length > 0)
                        {
                            var regexString = cols[0];
                            if (!string.IsNullOrWhiteSpace(regexString))
                            {
                                bool noAlter = false;
                                if (cols.Length > 1)
                                {
                                    noAlter = bool.Parse(cols[1]);
                                }

                                regexTokens.Add(new RegexToken()
                                {
                                    TokenRegex = new Regex(regexString.StartsWith("^", StringComparison.InvariantCultureIgnoreCase) ? regexString : ("^" + regexString), RegexOptions.IgnoreCase),
                                    NoAlter = noAlter,
                                });
                            }
                        }
                    }
                }
            }

            _miscellaneousTokenizer = new MiscellaneousTokenizer(whiteList, regexTokens);
            _wordpieceTokenizer = new WordpieceTokenizer(wordpieceVocabs, unknownVocab, maxCharsPerWord, whiteList, regexTokens);
        }

        public List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            return QueryRewriteTokenize(text).Select(x => x.Text).ToList();
        }

        public List<QueryRewriteToken> QueryRewriteTokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<QueryRewriteToken>();
            }

            var tokens = new List<QueryRewriteToken>();
            var miscTokens = _miscellaneousTokenizer.QueryRewriteTokenize(text);
            foreach (var miscToken in miscTokens)
            {
                var wordpieceToken = _wordpieceTokenizer.QueryRewriteTokenize(miscToken.Text);
                wordpieceToken.ForEach(x => x.Start = x.Start + miscToken.Start);
                tokens.AddRange(wordpieceToken);
            }

            return tokens;
        }
    }
}
