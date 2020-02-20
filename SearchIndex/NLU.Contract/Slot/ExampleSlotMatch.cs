//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// example pattern slot match
    /// </summary>
    public class ExampleSlotMatch : SlotMatch
    {
        /// <summary>
        /// slot match type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotMatchType Type => SlotMatchType.Example;

        /// <summary>
        /// list of example patterns
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<string> ExamplePatterns { get; set; }
    }
}
