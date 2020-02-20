//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.Common
{
    public static class SpanUtils
    {
        /// <summary>
        /// Check to see whether there is overlapping between a and b.
        /// </summary>
        public static bool IsOverlap(ISpan a, ISpan b)
        {
            return IsOverlap(a.Start, a.Length, b.Start, b.Length);
        }

        // Check if a is included by b, but not the same as b
        public static bool IsTrueIncluded(ISpan a, ISpan b)
        {
            return IsIncluded(a, b) && !IsSameSpan(a, b);
        }

        public static bool IsIncluded(ISpan a, ISpan b)
        {
            return IsIncluded(a.Start, a.Length, b.Start, b.Length);
        }

        /// <summary>
        /// Check if a is the same span with b
        /// </summary>
        public static bool IsSameSpan(ISpan a, ISpan b)
        {
            return IsSameSpan(a.Start, a.Length, b.Start, b.Length);
        }

        private static bool IsOverlap(int leftStart, int leftLength, int rightStart, int rightLength)
        {
            return (leftStart >= rightStart && leftStart < rightStart + rightLength)
                || (rightStart > leftStart && rightStart < leftStart + leftLength);
        }

        private static bool IsIncluded(int leftStart, int leftLength, int rightStart, int rightLength)
        {
            return leftStart >= rightStart && leftStart + leftLength <= rightStart + rightLength;
        }

        private static bool IsSameSpan(int leftStart, int leftLength, int rightStart, int rightLength)
        {
            return leftStart == rightStart && leftLength == rightLength;
        }
    }
}
