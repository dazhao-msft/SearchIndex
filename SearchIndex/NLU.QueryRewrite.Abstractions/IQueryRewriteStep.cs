//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public interface IQueryRewriteStep
    {
        bool Enabled { get; }

        QueryRewriteOperations RewriteOperation { get; }

        QueryRewriteStepResult Rewrite(QueryRewriteStepResult prevStepResult);
    }
}
