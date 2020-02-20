//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// regular expression slot match
    /// </summary>
    public class RegexSlotMatch : SlotMatch
    {
        /// <summary>
        /// slot match type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotMatchType Type => SlotMatchType.Regex;

        /// <summary>
        /// list of regular expressions
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<string> RegexPatterns { get; set; }
    }
}
