//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// entity count slot contraint
    /// </summary>
    public class EntityCountSlotConstraint : SlotConstraint
    {
        /// <summary>
        /// slot constraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotConstraintType Type => SlotConstraintType.EntityCount;

        /// <summary>
        /// maximum of entity count
        /// </summary>
        [JsonProperty(Required = Required.AllowNull)]
        public int? Max { get; set; }
    }
}
