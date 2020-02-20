//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataAnalysisConstraintValue
    {
        // The data analysis expression is optional for the ability
        None,

        // To trigger the ability, a query must contain the data analysis exression
        Required,

        // To trigger the ability, a query must NOT contain the data analysis exression
        Forbidden,
    }
}
