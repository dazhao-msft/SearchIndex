//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public interface ITokenizer
    {
        /// <summary>
        /// Tokenize text into a list of string tokens, if text doens't contain any valid token, an empty list will be returned
        /// </summary>
        List<string> Tokenize(string text);

        /// <summary>
        /// Tokenize text into a list of tokens, each token consists of Text, StartIndex and Length in char unit
        /// if text doens't contain any valid token, an empty list will be returned
        /// </summary>
        List<QueryRewriteToken> QueryRewriteTokenize(string text);
    }
}
