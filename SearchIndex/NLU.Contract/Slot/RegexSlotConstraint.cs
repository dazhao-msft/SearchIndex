//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// regular expression slot constraint
    /// </summary>
    public class RegexSlotConstraint : SlotConstraint
    {
        /// <summary>
        /// slot constraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotConstraintType Type => SlotConstraintType.Regex;

        /// <summary>
        /// list of regular expressions
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<string> RegexPatterns { get; set; }
    }
}
