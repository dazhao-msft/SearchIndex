//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public class QueryRewriteSpellerOptions
    {
        public bool SpellerEnabled { get; set; }

        public string SpellerType { get; set; }

        public string SpellerEntityFilePath { get; set; }

        public string UnigramFilePath { get; set; }

        public string BigramFilePath { get; set; }

        /// <summary>
        /// When domain ngrams are sparse, it can be used to turn on backing off to common ngrams.
        /// </summary>
        public bool BackoffToCommonNgram { get; set; }

        public string CommonUnigramFilePath { get; set; }

        public string CommonBigramFilePath { get; set; }

        public string BlacklistFilePath { get; set; }

        public string WhitelistFilePath { get; set; }
    }
}
