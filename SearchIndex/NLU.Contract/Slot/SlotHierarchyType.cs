﻿//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// slot hierarchy type enum
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlotHierarchyType
    {
        None,
        Parent,
        Child,
        Ancestor,
        Descendant,
    }
}