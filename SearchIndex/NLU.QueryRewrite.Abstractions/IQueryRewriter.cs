//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public interface IQueryRewriter : ITokenizer
    {
        QueryRewriteResult Rewrite(string query);

        /// <summary>
        /// Rewrite query with customized operations:
        /// The intersection of rewriteOperations and enabled operations in QueryRewriter will be executed
        /// </summary>
        /// <param name="query">input query</param>
        /// <param name="rewriteOperations">query rewrite operations</param>
        /// <returns></returns>
        QueryRewriteResult Rewrite(string query, QueryRewriteOperations rewriteOperations);

        /// <summary>
        /// Rewrite query with customized operations and spans rewriter should not alter:
        /// The intersection of rewriteOperations and enabled operations in QueryRewriter will be executed
        /// </summary>
        /// <param name="query">input query</param>
        /// <param name="noAlterSpans">spans query rewriter must not alter</param>
        /// <param name="rewriteOperations">query rewrite operations</param>
        /// <returns></returns>
        QueryRewriteResult Rewrite(string query, IReadOnlyList<ISpan> noAlterSpans, QueryRewriteOperations rewriteOperations = QueryRewriteOperations.AllRegularOperations);
    }
}
