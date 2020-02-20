//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.BizQA.NLU.QueryRewrite.QueryRewrite
{
    internal sealed class QueryRewriteStopwords : IQueryRewriteStep
    {
        // key is stopword, value is <whether to remove directly, whether the keyword is preposition>
        // preposition tag can be replaced with POS tagging in the future
        private readonly Dictionary<string, Tuple<bool, bool>> _stopwords;

        public QueryRewriteStopwords(QueryRewriteStopwordsOptions options)
        {
            Enabled = options?.StopwordsEnabled ?? false;
            if (Enabled)
            {
                _stopwords = new Dictionary<string, Tuple<bool, bool>>(StringComparer.OrdinalIgnoreCase);
                if (File.Exists(options.StopwordsFilePath))
                {
                    using (var reader = new StreamReader(options.StopwordsFilePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line.StartsWith("#", StringComparison.Ordinal))
                            {
                                continue;
                            }

                            var cols = line.Split('\t');
                            var stopword = cols[0].Trim();
                            if (!string.IsNullOrWhiteSpace(stopword))
                            {
                                if (!(cols.Length > 1 && bool.TryParse(cols[1].Trim(), out bool remove)))
                                {
                                    remove = false;
                                }

                                if (!(cols.Length > 2 && bool.TryParse(cols[2].Trim(), out bool isPreposition)))
                                {
                                    isPreposition = false;
                                }

                                _stopwords[stopword] = new Tuple<bool, bool>(remove, isPreposition);
                            }
                        }
                    }
                }
            }
        }

        public bool Enabled { get; }

        public QueryRewriteOperations RewriteOperation { get; } = QueryRewriteOperations.StopwordsRemoval;

        public QueryRewriteStepResult Rewrite(QueryRewriteStepResult prevStepResult)
        {
            if (!Enabled)
            {
                return prevStepResult;
            }

            var stopwordsTaggedTokens = new List<QueryRewriteToken>();
            foreach (var token in prevStepResult.RewriteTokens)
            {
                if (!token.Tags.HasFlag(QueryRewriteTags.NoAlter) && _stopwords.TryGetValue(token.Text, out var removePrepositionTuple))
                {
                    if (!removePrepositionTuple.Item1)
                    {
                        var stopwordToken = token.Clone();
                        stopwordToken.Tags |= QueryRewriteTags.Stopword;
                        if (removePrepositionTuple.Item2)
                        {
                            stopwordToken.Tags |= QueryRewriteTags.Preposition;
                        }

                        stopwordsTaggedTokens.Add(stopwordToken);
                    }
                }
                else
                {
                    stopwordsTaggedTokens.Add(token);
                }
            }

            return new QueryRewriteStepResult(stopwordsTaggedTokens, prevStepResult.RewriteOperations | RewriteOperation);
        }
    }
}
