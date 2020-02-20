//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.Common
{
    public class TextSpan : ISpan
    {
        public int Start { get; set; }

        public int Length { get; set; }

        public string MatchedText { get; set; }

        public string SpanTag { get; set; }
    }
}
