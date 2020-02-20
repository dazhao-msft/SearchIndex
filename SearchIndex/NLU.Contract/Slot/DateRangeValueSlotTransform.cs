//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// daterange value slot transform
    /// </summary>
    public class DateRangeValueSlotTransform : SlotTransform
    {
        /// <summary>
        /// slot transform type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public override SlotTransformType Type => SlotTransformType.DateRangeValue;

        /// <summary>
        /// time shift of daterange value
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int TimeShift { get; set; }

        /// <summary>
        /// shift unit of daterange value
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public TimeUnit ShiftUnit { get; set; }
    }
}
