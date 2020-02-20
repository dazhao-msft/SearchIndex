//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.Common
{
    public interface ISpan
    {
        int Start { get; }

        int Length { get; }
    }
}
