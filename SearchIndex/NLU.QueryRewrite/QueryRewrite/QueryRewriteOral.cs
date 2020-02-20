//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.BizQA.NLU.QueryRewrite.QueryRewrite
{
    /// <summary>
    /// Do oral normalization to user queries. From common sense:
    /// 1. "How can I" means the exact same with "How to", so we can use "How to" to replace "How can I".
    /// 2. "Could you help me with" basically doesn’t make any sense in understanding the user intent, we can remove it at all.
    /// Example: Raw query: "How can I upgrade my windows 8 to windows 10" will be normalized to "How to upgrade my windows 8 to windows 10".
    /// </summary>
    public sealed class QueryRewriteOral : IQueryRewriteStep
    {
        private readonly ITokenizer _tokenizer;

        private readonly IOrderedEnumerable<Tuple<string, string, bool>> _oralMapInfo;

        public QueryRewriteOral(QueryRewriteOralOptions options, ITokenizer tokenizer)
        {
            Enabled = options != null ? options.OralEnabled : false;
            if (Enabled)
            {
                _tokenizer = tokenizer;

                if (File.Exists(options.OralFilePath))
                {
                    var oralMapInfo = new List<Tuple<string, string, bool>>();
                    using (var reader = new StreamReader(options.OralFilePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line.StartsWith("#", StringComparison.Ordinal))
                            {
                                continue;
                            }

                            var cols = line.Split('\t');
                            if (cols.Length > 1)
                            {
                                bool prefixMatch = false;
                                if (cols.Length > 2)
                                {
                                    prefixMatch = bool.Parse(cols[2]);
                                }

                                // source can't be empty
                                if (!string.IsNullOrWhiteSpace(cols[0]))
                                {
                                    oralMapInfo.Add(new Tuple<string, string, bool>(cols[0].Trim(), cols[1].Trim().ToLowerInvariant(), prefixMatch));
                                }
                            }
                        }
                    }

                    // sorted by the length of the key, if the same length then sorted by alphabetically.
                    _oralMapInfo = from t in oralMapInfo
                                   orderby t.Item1.Length descending, t.Item1
                                   select t;
                }
                else
                {
                    _oralMapInfo = Enumerable.Empty<Tuple<string, string, bool>>().OrderBy(t => t.Item1);
                }
            }
        }

        public bool Enabled { get; }

        public QueryRewriteOperations RewriteOperation { get; } = QueryRewriteOperations.OralRewriting;

        public QueryRewriteStepResult Rewrite(QueryRewriteStepResult prevStepResult)
        {
            if (!Enabled)
            {
                return prevStepResult;
            }

            var prevQuery = prevStepResult.RewrittenQuery;
            if (string.IsNullOrWhiteSpace(prevQuery))
            {
                return new QueryRewriteStepResult(prevStepResult.RewriteTokens, prevStepResult.RewriteOperations | RewriteOperation);
            }

            var oralRewrittenTokens = new List<QueryRewriteToken>(prevStepResult.RewriteTokens);
            foreach (var oralTuple in _oralMapInfo)
            {
                int matchIndex;
                if (oralTuple.Item3)
                {
                    matchIndex = prevQuery.StartsWith(oralTuple.Item1, StringComparison.InvariantCultureIgnoreCase) ? 0 : -1;
                }
                else
                {
                    matchIndex = prevQuery.IndexOf(oralTuple.Item1, 0, StringComparison.InvariantCultureIgnoreCase);
                }

                if (matchIndex >= 0)
                {
                    var prevQueryOffsets = new int[oralRewrittenTokens.Count];
                    prevQueryOffsets[0] = 0;
                    for (int i = 1; i < oralRewrittenTokens.Count; i++)
                    {
                        prevQueryOffsets[i] = prevQueryOffsets[i - 1];
                        if (!string.IsNullOrEmpty(oralRewrittenTokens[i - 1].Text))
                        {
                            prevQueryOffsets[i] += oralRewrittenTokens[i - 1].Text.Length + 1;
                        }
                    }

                    int startTokenIndex = -1;
                    int endTokenIndex = -1;

                    // must match on token boundary, for example, you can not remove "show me" in "show mexico sales numbers"
                    for (int i = 0; i < prevQueryOffsets.Length; i++)
                    {
                        if (prevQueryOffsets[i] == matchIndex)
                        {
                            startTokenIndex = i;
                        }

                        if ((prevQueryOffsets[i] + oralRewrittenTokens[i].Text.Length) == (matchIndex + oralTuple.Item1.Length))
                        {
                            endTokenIndex = i + 1;
                        }
                    }

                    if (startTokenIndex >= 0 && endTokenIndex > 0 && endTokenIndex > startTokenIndex)
                    {
                        bool containsNoAlterToken = false;
                        for (int i = startTokenIndex; i < endTokenIndex; i++)
                        {
                            if (oralRewrittenTokens[i].Tags.HasFlag(QueryRewriteTags.NoAlter))
                            {
                                containsNoAlterToken = true;
                                break;
                            }
                        }

                        if (containsNoAlterToken)
                        {
                            continue;
                        }

                        var newOralRewrittenTokens = new List<QueryRewriteToken>();
                        for (int i = 0; i < startTokenIndex; i++)
                        {
                            newOralRewrittenTokens.Add(oralRewrittenTokens[i]);
                        }

                        if (!string.IsNullOrWhiteSpace(oralTuple.Item2))
                        {
                            var replaceTokens = _tokenizer.Tokenize(oralTuple.Item2);
                            if (replaceTokens != null && replaceTokens.Any())
                            {
                                int rawStartIndex = oralRewrittenTokens[startTokenIndex].Start;
                                int rawEndIndex = oralRewrittenTokens[startTokenIndex].End;

                                for (int i = startTokenIndex + 1; i < endTokenIndex; i++)
                                {
                                    if (rawStartIndex > oralRewrittenTokens[i].Start)
                                    {
                                        rawStartIndex = oralRewrittenTokens[i].Start;
                                    }

                                    if (rawEndIndex < oralRewrittenTokens[i].End)
                                    {
                                        rawEndIndex = oralRewrittenTokens[i].End;
                                    }
                                }

                                newOralRewrittenTokens.AddRange(replaceTokens.Select(t => new QueryRewriteToken(t, rawStartIndex, rawEndIndex - rawStartIndex)));
                            }
                        }

                        for (int i = endTokenIndex; i < oralRewrittenTokens.Count; i++)
                        {
                            newOralRewrittenTokens.Add(oralRewrittenTokens[i]);
                        }

                        oralRewrittenTokens = newOralRewrittenTokens;
                        if (!oralRewrittenTokens.Any())
                        {
                            break;
                        }

                        prevQuery = QueryRewriteStepResult.GetRewrittenQuery(oralRewrittenTokens);
                    }
                }
            }

            return new QueryRewriteStepResult(oralRewrittenTokens, prevStepResult.RewriteOperations | RewriteOperation);
        }
    }
}
