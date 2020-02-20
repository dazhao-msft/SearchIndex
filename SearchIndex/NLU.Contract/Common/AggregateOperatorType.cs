//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AggregateOperatorType
    {
        // sum total operator.
        Sum,

        // average operator.
        Mean,

        // max operator.
        Max,

        // min operator.
        Min,

        // group by operator.
        GroupBy,

        // ranked by operator.
        RankedBy,
    }
}
