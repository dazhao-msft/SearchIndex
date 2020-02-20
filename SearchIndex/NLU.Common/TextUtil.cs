//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.QueryRewrite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.BizQA.NLU.Common
{
    public static class TextUtil
    {
        /// <summary>
        /// Compute the edit distance between 2 string
        /// </summary>
        public static int EditDistance(string text1, string text2)
        {
            var distances = Enumerable.Range(0, text1.Length + 1).ToList();
            for (var j = 1; j <= text2.Length; j++)
            {
                var tempDistance = distances[0];
                distances[0]++;

                for (var i = 1; i <= text1.Length; i++)
                {
                    var distance1 = text1[i - 1] == text2[j - 1] ? tempDistance : tempDistance + 1;
                    var distance2 = distances[i - 1] + 1;
                    var distance3 = distances[i] + 1;

                    tempDistance = distances[i];
                    distances[i] = Math.Min(Math.Min(distance1, distance2), distance3);
                }
            }

            return distances[text1.Length];
        }

        /// <summary>
        /// Compute the edit distance between 2 strings with swapping of successive symbols counted as 1
        /// a.k.a. Damerau–Levenshtein distance: https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
        /// for example the distance between "abcde" and "abdce" is 1
        /// </summary>
        public static int EditDistanceWithSwap(string text1, string text2)
        {
            var len1 = text1.Length;
            var len2 = text2.Length;
            int[,] dp = new int[len1 + 1, len2 + 1];
            for (int i = 0; i <= len2; i++)
            {
                dp[0, i] = i;
            }

            for (int i = 0; i <= len1; i++)
            {
                dp[i, 0] = i;
            }

            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    dp[i + 1, j + 1] = text1[i] == text2[j] ? dp[i, j] : Math.Min(dp[i, j + 1], dp[i + 1, j]) + 1;
                    if (i > 0 && j > 0 && text1[i - 1] == text2[j] && text1[i] == text2[j - 1])
                    {
                        dp[i + 1, j + 1] = Math.Min(dp[i + 1, j + 1], dp[i - 1, j - 1] + 1);
                    }
                }
            }

            return dp[len1, len2];
        }

        /// <summary>
        /// Relaxed exact match between text1 and text2
        /// </summary>
        public static bool RelaxedExactMatch(string text1, string text2, ITokenizer tokenizer = null)
        {
            var txt1 = text1?.Trim()?.ToLowerInvariant();
            var txt2 = text2?.Trim()?.ToLowerInvariant();
            var textScore = EditDistance(txt1, txt2);
            if (textScore <= 1)
            {
                return true;
            }

            var tokens1 = tokenizer == null ? txt1?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)?.ToList() : tokenizer.QueryRewriteTokenize(text1)?.Select(x => x?.Text?.Trim()?.ToLowerInvariant())?.ToList();
            var tokens2 = tokenizer == null ? txt2?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)?.ToList() : tokenizer.QueryRewriteTokenize(text2)?.Select(x => x?.Text?.Trim()?.ToLowerInvariant())?.ToList();

            if (tokens1?.Count != tokens2?.Count)
            {
                return false;
            }

            var tokenScores = tokens1.Zip(tokens2, (t1, t2) => EditDistance(t1, t2)).ToList();
            return tokenScores.All(x => x <= 1);
        }

        /// <summary>
        /// Convert char span to token span
        /// </summary>
        public static TextSpan ConvertCharSpan2TokenSpan(TextSpan charSpan, IReadOnlyList<ISpan> tokens)
        {
            int tokenSpanStart = -1;
            int tokenSpanLength = 0;
            int firstMatchedToken = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                // start not found
                if (tokenSpanStart < 0)
                {
                    if (charSpan.Start < tokens[i].Start + tokens[i].Length)
                    {
                        tokenSpanStart = i;
                        firstMatchedToken = charSpan.Start + charSpan.Length >= tokens[i].Start ? 1 : 0;
                    }
                }

                // start found
                if (tokenSpanStart >= 0)
                {
                    // end before start of next token
                    if (i == tokens.Count - 1 || charSpan.Start + charSpan.Length <= tokens[i + 1].Start)
                    {
                        tokenSpanLength = i - tokenSpanStart + firstMatchedToken;
                        break;
                    }
                }
            }

            if (tokenSpanStart < 0)
            {
                tokenSpanStart = tokens.Count;
            }

            return new TextSpan()
            {
                Start = tokenSpanStart,
                Length = tokenSpanLength,
                MatchedText = charSpan.MatchedText,
                SpanTag = charSpan.SpanTag,
            };
        }

        public static int ConvertCharSpan2TokenIndex(int start, int len, IReadOnlyList<ISpan> tokens, out int extraIdx)
        {
            extraIdx = -1;
            int tokenSpanStart = -1;
            for (int i = 0; i < tokens.Count; i++)
            {
                // start not found
                if (tokenSpanStart < 0)
                {
                    if (start < tokens[i].Start + tokens[i].Length)
                    {
                        tokenSpanStart = i;
                        if (start + len >= tokens[i].Start)
                        {
                            // handle: thankyou --> thank you
                            if (i + 1 < tokens.Count && start + len > tokens[i + 1].Start + 1)
                            {
                                extraIdx = i + 1;
                            }

                            return tokenSpanStart;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }

            return tokens.Count;
        }
    }
}
