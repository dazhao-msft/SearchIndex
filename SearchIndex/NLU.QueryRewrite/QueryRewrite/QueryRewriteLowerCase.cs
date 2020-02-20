//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Linq;

namespace Microsoft.BizQA.NLU.QueryRewrite.QueryRewrite
{
    internal sealed class QueryRewriteLowerCase : IQueryRewriteStep
    {
        public QueryRewriteLowerCase(bool enabled, QueryRewriteOperations rewriteOperation)
        {
            Enabled = enabled;
            RewriteOperation = rewriteOperation;
        }

        public bool Enabled { get; }

        public QueryRewriteOperations RewriteOperation { get; }

        public QueryRewriteStepResult Rewrite(QueryRewriteStepResult prevStepResult)
        {
            if (!Enabled)
            {
                return prevStepResult;
            }

            var queryRewriteTokens = prevStepResult.RewriteTokens
                    .Select(t => new QueryRewriteToken(t.Text.ToLowerInvariant(), t.Start, t.Length)
                    {
                        Tags = t.Tags,
                    }).ToList();

            return new QueryRewriteStepResult(queryRewriteTokens, prevStepResult.RewriteOperations | RewriteOperation);
        }
    }
}
