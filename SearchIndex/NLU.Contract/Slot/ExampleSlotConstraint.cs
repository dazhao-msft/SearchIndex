//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// example pattern slot constraint
    /// </summary>
    public class ExampleSlotConstraint : SlotConstraint
    {
        /// <summary>
        /// slot constraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotConstraintType Type => SlotConstraintType.Example;

        /// <summary>
        /// list of example patterns
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<string> ExamplePatterns { get; set; }
    }
}
