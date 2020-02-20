//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataOperator
    {
        // Default.
        None,

        // Select attribute value from records.
        Select,

        // Where clause, set attribute condition
        Where,

        // Rank records by an attribute descendingly
        RankByDesc,

        // Rank records by an attribute ascendingly
        RankByAsc,

        // Group records by an attribute
        GroupBy,

        // Explicitly access primary table, either with table name or key cell
        SelectFromPrimaryTable,
    }
}
