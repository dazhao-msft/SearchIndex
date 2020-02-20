//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// hierarchy value slot transform
    /// </summary>
    public class HierarchyValueSlotTransform : SlotTransform
    {
        /// <summary>
        /// slot transform type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotTransformType Type => SlotTransformType.HierarchyValue;

        /// <summary>
        /// slot function type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public SlotHierarchyType HierarchyType { get; set; }
    }
}
