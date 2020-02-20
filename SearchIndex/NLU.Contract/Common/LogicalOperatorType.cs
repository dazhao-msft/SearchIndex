//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogicalOperatorType
    {
        // build-in and operator.
        And,

        // build-in or operator.
        Or,

        // build-in not operator.
        Not,
    }
}
