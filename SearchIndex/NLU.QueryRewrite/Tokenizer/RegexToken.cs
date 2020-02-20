//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Microsoft.BizQA.NLU.QueryRewrite.Tokenizer
{
    public class RegexToken
    {
        public Regex TokenRegex { get; set; }

        public bool NoAlter { get; set; }
    }
}
