//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// slot constraint type enum
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlotConstraintType
    {
        None,
        EntityType,
        EntitySet,
        EntityCount,
        EntityBelongsTo,
        Keyword,
        Regex,
        Example,
    }
}
