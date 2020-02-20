//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// slot transform type enum
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlotTransformType
    {
        None,
        KeyPhraseValue,
        DateRangeValue,
        HierarchyValue,
    }
}
