//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    [Flags]
    public enum QueryRewriteTags : int
    {
        None = 0,

        /// <summary>
        /// A token with NoAlter tag will not be changed during query rewrite except by case normalization if it's enabled
        /// </summary>
        NoAlter = 1,

        /// <summary>
        /// This token is a stopword
        /// </summary>
        Stopword = 2,

        /// <summary>
        /// This token is a preposition
        /// </summary>
        Preposition = 4,
    }
}
