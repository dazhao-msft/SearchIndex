//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    [Serializable]
    public class QueryRewriteResult
    {
        /// <summary>
        /// For json deserialization, do not remove.
        /// </summary>
        public QueryRewriteResult()
        {
        }

        public QueryRewriteResult(string rawQuery)
        {
            RawQuery = rawQuery;
            QueryRewriteStepResults = new List<QueryRewriteStepResult>();
            var rawQueryToken = new QueryRewriteToken(RawQuery, 0, RawQuery.Length);
            QueryRewriteStepResults.Add(new QueryRewriteStepResult(new List<QueryRewriteToken>() { rawQueryToken }, QueryRewriteOperations.None));
        }

        [JsonProperty]
        public string RawQuery { get; set; }

        [JsonProperty]
        public List<QueryRewriteStepResult> QueryRewriteStepResults { get; set; }

        [JsonIgnore]
        public QueryRewriteStepResult LastStepResult
        {
            get
            {
                var resultCount = QueryRewriteStepResults.Count;
                return resultCount > 0 ? QueryRewriteStepResults[resultCount - 1] : null;
            }
        }

        [JsonIgnore]
        public IReadOnlyList<QueryRewriteToken> LastStepRewriteTokens
        {
            get
            {
                return LastStepResult?.RewriteTokens ?? new List<QueryRewriteToken>();
            }
        }

        [JsonIgnore]
        public List<string> RewrittenQueries
        {
            get
            {
                return QueryRewriteStepResults.Select(r => r.RewrittenQuery).ToList();
            }
        }

        [JsonIgnore]
        public string LastRewrittenQuery
        {
            get
            {
                return LastStepResult?.RewrittenQuery;
            }
        }

        public QueryRewriteStepResult RewriteResultBeforeOperation(QueryRewriteOperations rewriteOperation)
        {
            return QueryRewriteStepResults.LastOrDefault(r => !r.RewriteOperations.HasFlag(rewriteOperation));
        }

        public QueryRewriteStepResult RewriteResultAfterOperation(QueryRewriteOperations rewriteOperation)
        {
            return QueryRewriteStepResults.FirstOrDefault(r => r.RewriteOperations.HasFlag(rewriteOperation));
        }
    }
}
