//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;

namespace Microsoft.BizQA.NLU.Common
{
    public static class DateTimeUtil
    {
        public static bool IsValidDate(int year, int month, int day)
        {
            if (!IsValidYear(year) || !IsValidMonth(month))
            {
                return false;
            }

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }

        public static bool IsValidYear(int year)
        {
            return year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year ? false : true;
        }

        public static bool IsValidMonth(int month)
        {
            return month < 1 || month > 12 ? false : true;
        }
    }
}
