//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    /// <summary>
    /// Flagged token object as container for Office Spller result
    /// </summary>
    public class FlaggedToken
    {
        public uint Offset { get; set; }

        public string Token { get; set; }

        /// <summary>
        /// A list of suggestions.
        /// </summary>
        public List<string> Suggestions { get; set; } = new List<string>();
    }
}
