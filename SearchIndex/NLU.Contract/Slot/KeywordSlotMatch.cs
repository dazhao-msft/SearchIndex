//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// keyword pattern slot match
    /// </summary>
    public class KeywordSlotMatch : SlotMatch
    {
        /// <summary>
        /// slot match type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotMatchType Type => SlotMatchType.Keyword;

        /// <summary>
        /// list of keyword patterns
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<IList<string>> KeywordPatterns { get; set; }
    }
}
