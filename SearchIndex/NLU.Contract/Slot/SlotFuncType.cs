//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// slot function type enum
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlotFuncType
    {
        None,
        Date2Age,
        NumberRange2CurrencyRange,
        KeyPhrase2Enum,
    }
}
