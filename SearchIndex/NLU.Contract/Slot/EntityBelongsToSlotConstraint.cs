//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// entity belongs-to slot contraint
    /// </summary>
    public class EntityBelongsToSlotConstraint : SlotConstraint
    {
        /// <summary>
        /// slot constraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotConstraintType Type => SlotConstraintType.EntityBelongsTo;

        /// <summary>
        /// list of belongs-to entity ids
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<string> BelongsToEntityIds { get; set; }
    }
}
