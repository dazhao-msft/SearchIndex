//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// keyword pattern slot constraint
    /// </summary>
    public class KeywordSlotConstraint : SlotConstraint
    {
        /// <summary>
        /// slot constraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotConstraintType Type => SlotConstraintType.Keyword;

        /// <summary>
        /// list of keyword patterns
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<IList<string>> KeywordPatterns { get; set; }
    }
}
