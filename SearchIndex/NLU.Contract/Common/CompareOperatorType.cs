//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CompareOperatorType
    {
        // build-in not equal operator.
        NotEqualsTo,

        // build-in equal operator.
        EqualsTo,

        // build-in greater than operator.
        GreaterThan,

        // build-in greater than or equal operator.
        GreaterThanOrEqual,

        // build-in less than operator.
        LessThan,

        // build-in less than or equal operator.
        LessThanOrEqual,

        // build-in between operator
        Between,

        // build-in in operator
        In,

        // build-in not-in operator
        NotIn,

        // build-in HasNoValue operator
        HasNoValue,

        // build-in HasValue operator
        HasValue,
    }
}
