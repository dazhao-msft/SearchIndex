//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// entity type slot contraint
    /// </summary>
    public class EntityTypeSlotConstraint : SlotConstraint
    {
        /// <summary>
        /// slot constraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotConstraintType Type => SlotConstraintType.EntityType;

        /// <summary>
        /// list of entity type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<string> EntityTypes { get; set; }
    }
}
