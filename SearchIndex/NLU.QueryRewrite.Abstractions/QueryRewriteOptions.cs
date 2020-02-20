//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public class QueryRewriteOptions
    {
        /// <summary>
        /// The locale info. Such as "en-US" etc.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// The QueryRewriteTypeName is the Query Rewriter class Name. For now,
        /// we only expose class DefaultQueryRewriter outside the module, so it
        /// only supports class name of DefaultQueryRewriter.
        /// </summary>
        public string QueryRewriteTypeName { get; set; }

        /// <summary>
        /// Whether to convert text to lower case before tokenizer.
        /// </summary>
        public bool PreLowerCaseEnabled { get; set; }

        /// <summary>
        /// Whether to convert text to lower case as the last rewrite step.
        /// </summary>
        public bool PostLowerCaseEnabled { get; set; }

        public QueryRewriteTokenizerOptions TokenizerOptions { get; set; }

        public QueryRewriteTokenMappingOptions TokenMappingOptions { get; set; }

        public QueryRewriteSpellerOptions SpellerOptions { get; set; }

        public QueryRewriteOralOptions OralOptions { get; set; }

        public QueryRewriteStemmerOptions StemmerOptions { get; set; }

        public QueryRewriteStopwordsOptions StopwordsOptions { get; set; }
    }
}
