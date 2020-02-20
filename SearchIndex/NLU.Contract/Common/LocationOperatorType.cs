//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LocationOperatorType
    {
        // build-in and operator.
        Near,
    }
}
