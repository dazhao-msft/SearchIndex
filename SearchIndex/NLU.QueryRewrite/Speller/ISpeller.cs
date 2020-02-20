//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    public interface ISpeller
    {
        string SpellCorrect(string text);

        List<QueryRewriteToken> SpellCorrect(IReadOnlyList<QueryRewriteToken> inputTokens);
    }
}
