//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// entity set slot contraint
    /// </summary>
    public class EntitySetSlotConstraint : SlotConstraint
    {
        /// <summary>
        /// slot constraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotConstraintType Type => SlotConstraintType.EntitySet;

        /// <summary>
        /// list of entity ids
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<string> EntityIds { get; set; }
    }
}
