//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// keyphrase value slot transform
    /// </summary>
    public class KeyPhraseValueSlotTransform : SlotTransform
    {
        /// <summary>
        /// slot transform type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotTransformType Type => SlotTransformType.KeyPhraseValue;

        /// <summary>
        /// lookup of keyphrase value
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IDictionary<string, string> KeyPhraseValues { get; set; }
    }
}
