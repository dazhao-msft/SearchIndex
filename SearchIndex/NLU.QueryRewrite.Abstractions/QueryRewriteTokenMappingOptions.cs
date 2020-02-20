//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public class QueryRewriteTokenMappingOptions
    {
        /// <summary>
        /// Whether to do the token mapping for the original text.
        /// </summary>
        public bool TokenMappingEnabled { get; set; }

        /// <summary>
        /// Token mapping file path.
        /// </summary>
        public string TokenMappingFilePath { get; set; }
    }
}
