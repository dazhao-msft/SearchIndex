//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// function slot match
    /// </summary>
    public class FunctionSlotMatch : SlotMatch
    {
        /// <summary>
        /// slot match type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotMatchType Type => SlotMatchType.Function;

        /// <summary>
        /// slot function type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public SlotFuncType FuncType { get; set; }
    }
}
