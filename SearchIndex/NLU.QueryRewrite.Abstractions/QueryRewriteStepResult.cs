//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    [Serializable]
    public class QueryRewriteStepResult
    {
        private const string DefaultJoinSeparator = " ";

        private string _rewrittenQuery = null;

        /// <summary>
        /// For json deserialization, do not remove.
        /// </summary>
        public QueryRewriteStepResult()
        {
        }

        public QueryRewriteStepResult(IReadOnlyList<QueryRewriteToken> queryRewriteTokens, QueryRewriteOperations rewriteOperations)
        {
            RewriteTokens = queryRewriteTokens;
            RewriteOperations = rewriteOperations;
        }

        [JsonProperty]
        public QueryRewriteOperations RewriteOperations { get; set; } = QueryRewriteOperations.None;

        [JsonProperty]
        public IReadOnlyList<QueryRewriteToken> RewriteTokens { get; set; }

        [JsonIgnore]
        public string RewrittenQuery
        {
            get
            {
                _rewrittenQuery = _rewrittenQuery ?? GetRewrittenQuery(RewriteTokens);

                return _rewrittenQuery;
            }
        }

        [JsonIgnore]
        public List<QueryRewriteToken> TokensInRewrittenQueryPosition
        {
            get
            {
                var tokensWithRewrittenQueryPosition = new List<QueryRewriteToken>();
                if (RewriteTokens != null)
                {
                    var start = 0;
                    foreach (var token in RewriteTokens)
                    {
                        tokensWithRewrittenQueryPosition.Add(new QueryRewriteToken(token.Text, start, token.Text.Length, token.Tags));
                        start += token.Text.Length + DefaultJoinSeparator.Length;
                    }
                }

                return tokensWithRewrittenQueryPosition;
            }
        }

        public static string GetRewrittenQuery(IReadOnlyList<QueryRewriteToken> rewriteTokens)
        {
            if (rewriteTokens == null || !rewriteTokens.Any())
            {
                return string.Empty;
            }
            else
            {
                return string.Join(DefaultJoinSeparator, rewriteTokens.Select(t => t.Text));
            }
        }

        public string GetRewrittenQueryWoStopwords()
        {
            if (RewriteTokens == null || RewriteTokens.Count < 1)
            {
                return string.Empty;
            }
            else
            {
                var sbRewrittenQuery = new StringBuilder();
                for (int i = 0; i < RewriteTokens.Count; i++)
                {
                    if (!RewriteTokens[i].IsStopword)
                    {
                        if (sbRewrittenQuery.Length == 0)
                        {
                            sbRewrittenQuery.Append(RewriteTokens[i].Text);
                        }
                        else
                        {
                            sbRewrittenQuery.Append(DefaultJoinSeparator + RewriteTokens[i].Text);
                        }
                    }
                }

                return sbRewrittenQuery.ToString();
            }
        }
    }
}
