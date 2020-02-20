//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.BizQA.NLU.QueryRewrite.Tokenizer
{
    internal abstract class BaseTokenizer : ITokenizer
    {
        private readonly IReadOnlyDictionary<string, string> _stringMappings = new Dictionary<string, string>
        {
            // left single quotation mark
            { "\u2018", "\'" },

            // right single quotation mark
            { "\u2019", "'" },

            // left double quotation mark
            { "\u201c", "\"" },

            // right double quotation mark
            { "\u201d", "\"" },

            // fullwidth quotation mark
            { "\uff02", "\"" },

            // fullwidth question mark
            { "\uff1f", "?" },

            // zero width space
            { "\u200b", " " },

            // zero width no-break space
            { "\ufeff", " " },
        };

        private readonly HashSet<string> _tokenWhiteList;

        private readonly IReadOnlyList<RegexToken> _regexTokens;

        protected BaseTokenizer(HashSet<string> tokenWhiteList, IReadOnlyList<RegexToken> regexTokens)
        {
            _tokenWhiteList = tokenWhiteList;
            _regexTokens = regexTokens;
        }

        public virtual List<string> Tokenize(string text)
        {
            return QueryRewriteTokenize(text).Select(tk => tk.Text).ToList();
        }

        public abstract List<QueryRewriteToken> QueryRewriteTokenize(string text);

        protected int NextInWhiteList(string text, int start)
        {
            if (_tokenWhiteList == null || _tokenWhiteList.Count < 1)
            {
                return -1;
            }

            var subString = text.Substring(start);
            var candidateToken = _tokenWhiteList.FirstOrDefault(x => subString.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
            if (candidateToken?.Length > 0)
            {
                var end = start + candidateToken.Length;
                if (end >= text.Length || char.IsWhiteSpace(text[end]) || char.IsPunctuation(text[end]))
                {
                    return end;
                }
            }

            return -1;
        }

        protected int NextInRegexTokens(string text, int start, out bool noAlter)
        {
            noAlter = false;
            if (_regexTokens == null || !_regexTokens.Any())
            {
                return -1;
            }

            var subString = text.Substring(start);
            var regexTokenMatches = _regexTokens.Select(x => new Tuple<Match, bool>(x.TokenRegex.Match(subString), x.NoAlter)).Where(t => t.Item1.Success);
            if (regexTokenMatches.Any())
            {
                int longestMatchLength = regexTokenMatches.Max(t => t.Item1.Length);
                if (longestMatchLength > 0)
                {
                    var end = start + longestMatchLength;
                    noAlter = regexTokenMatches.Any(t => (t.Item1.Length == longestMatchLength) && t.Item2);
                    if (end >= text.Length || !char.IsLetterOrDigit(text[end]) || char.IsLetterOrDigit(text[end - 1]))
                    {
                        return end;
                    }
                }
            }

            return -1;
        }

        protected string CharNormalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var normalized = text;
            foreach (var kvp in _stringMappings)
            {
                normalized = normalized.Replace(kvp.Key, kvp.Value);
            }

            return normalized;
        }
    }
}
