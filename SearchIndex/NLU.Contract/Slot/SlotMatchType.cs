//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// slot match type enum
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlotMatchType
    {
        None,
        Keyword,
        Regex,
        Function,
        Example,
    }
}
