//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;

namespace Microsoft.BizQA.NLU.Common
{
    public class TimeInterval
    {
        public TimeInterval(DateTime? startTime, DateTime? endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
