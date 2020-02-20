//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    [Flags]
    public enum QueryRewriteOperations : int
    {
        None = 0,
        PreLowerCaseNormalization = 1,
        Tokenization = 2,
        TokenMapping = 4,
        SpellCorrection = 8,
        OralRewriting = 16,
        StopwordsRemoval = 32,
        Stemming = 64,
        PostLowerCaseNormalization = 128,

        AllRegularOperations = PreLowerCaseNormalization | Tokenization | TokenMapping | SpellCorrection | OralRewriting | StopwordsRemoval | Stemming | PostLowerCaseNormalization,

        // Additional operation
        TaggedStopwordsRemoval = 256,
    }
}
