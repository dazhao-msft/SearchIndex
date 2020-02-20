//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.BizQA.NLU.QueryRewrite.Tokenizer
{
    /// <summary>
    /// The tokenizer splits the input text into tokens, treating whitespace and punctuation as delimiters.
    /// Delimiter characters are discarded, with the following exceptions:
    /// 1. Periods(dots) that are not followed by whitespace are kept as part of the token, including Internet domain names.
    /// 2. The "@" character is among the set of token-splitting punctuation, so email addresses are not preserved as single tokens.
    /// </summary>
    internal sealed class DefaultTokenizer : ITokenizer
    {
        private readonly ITokenizer _standardTokenizer;

        public DefaultTokenizer(QueryRewriteTokenizerOptions options)
        {
            var whiteList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var regexTokens = new List<RegexToken>();

            // As we need use standard tokenizer for tokenization, so we need to
            // load the resource no matter whether tokenizer is Enabled or not.
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

                                // must match from beginning
                                regexTokens.Add(new RegexToken
                                {
                                    TokenRegex = new Regex(
                                            regexString.StartsWith("^", StringComparison.InvariantCultureIgnoreCase) ? regexString : ("^" + regexString),
                                            RegexOptions.IgnoreCase),
                                    NoAlter = noAlter,
                                });
                            }
                        }
                    }
                }
            }

            _standardTokenizer = new StandardTokenizer(whiteList, regexTokens);
        }

        public List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            return _standardTokenizer.Tokenize(text);
        }

        public List<QueryRewriteToken> QueryRewriteTokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<QueryRewriteToken>();
            }

            return _standardTokenizer.QueryRewriteTokenize(text);
        }
    }
}
