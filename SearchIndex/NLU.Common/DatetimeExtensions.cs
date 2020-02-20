//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Contract;
using System;

namespace Microsoft.BizQA.NLU.Common
{
    public static class DatetimeExtensions
    {
        public static TimeInterval GetPastTimeInterval(this DateTime currentTime, TimeUnit timeUnit, int timeSpan, int startMonth = 1, int startDay = 1)
        {
            if (timeSpan <= 0)
            {
                throw new ArgumentOutOfRangeException($"{timeSpan} should be greater than 0.");
            }

            DateTime? startTime = null;
            DateTime? endTime = null;
            switch (timeUnit)
            {
                case TimeUnit.Day:
                    var dayPivotTime = currentTime.Date.AddDays(-1);
                    endTime = new DateTime(dayPivotTime.Year, dayPivotTime.Month, dayPivotTime.Day, 23, 59, 59);
                    dayPivotTime = endTime.Value.AddDays((timeSpan - 1) * -1);
                    startTime = new DateTime(dayPivotTime.Year, dayPivotTime.Month, dayPivotTime.Day, 0, 0, 0);
                    break;
                case TimeUnit.Week:
                    var weekPivotNumber = (int)currentTime.DayOfWeek;
                    var weekPivotTime = currentTime.AddDays((weekPivotNumber + 1) * -1);
                    endTime = new DateTime(weekPivotTime.Year, weekPivotTime.Month, weekPivotTime.Day, 23, 59, 59);
                    weekPivotTime = endTime.Value.AddDays((timeSpan * -7) + 1);
                    startTime = new DateTime(weekPivotTime.Year, weekPivotTime.Month, weekPivotTime.Day, 0, 0, 0);
                    break;
                case TimeUnit.Month:
                    var monthPivotTime = new DateTime(currentTime.Year, currentTime.Month, 1).AddDays(-1);
                    endTime = new DateTime(monthPivotTime.Year, monthPivotTime.Month, monthPivotTime.Day, 23, 59, 59);
                    monthPivotTime = endTime.Value.AddMonths((timeSpan * -1) + 1);
                    startTime = new DateTime(monthPivotTime.Year, monthPivotTime.Month, 1, 0, 0, 0);
                    break;
                case TimeUnit.Quarter:
                    var quarterPivotNumber = (int)Math.Ceiling(currentTime.Month / 3.0);
                    var quarterPivotTime = new DateTime(currentTime.Year, 1, 1).AddMonths((quarterPivotNumber - 1) * 3).AddDays(-1);
                    endTime = new DateTime(quarterPivotTime.Year, quarterPivotTime.Month, quarterPivotTime.Day, 23, 59, 59);
                    quarterPivotTime = endTime.Value.AddMonths((timeSpan * -3) + 1);
                    startTime = new DateTime(quarterPivotTime.Year, quarterPivotTime.Month, 1, 0, 0, 0);
                    break;
                case TimeUnit.Year:
                    var yearPivotTime = currentTime.AddYears(-1);
                    endTime = new DateTime(yearPivotTime.Year, 12, 31, 23, 59, 59);
                    yearPivotTime = endTime.Value.AddYears((timeSpan * -1) + 1);
                    startTime = new DateTime(yearPivotTime.Year, 1, 1, 0, 0, 0);
                    break;
                case TimeUnit.FiscalYear:
                    if (!DateTimeUtil.IsValidDate(currentTime.Year, startMonth, startDay))
                    {
                        if (DateTimeUtil.IsValidMonth(startMonth))
                        {
                            startDay = 1;
                        }
                        else
                        {
                            startMonth = 1;
                            startDay = 1;
                        }
                    }

                    startTime = currentTime.Month >= startMonth ? new DateTime(currentTime.Year, startMonth, startDay, 0, 0, 0) : new DateTime(currentTime.Year, startMonth, startDay, 0, 0, 0).AddYears(-1);
                    startTime = startTime == null ? startTime : ((DateTime)startTime).AddYears(-1);
                    endTime = startTime == null ? startTime : ((DateTime)startTime).AddYears(1).AddSeconds(-1);
                    break;
            }

            return new TimeInterval(startTime, endTime);
        }

        public static TimeInterval GetFutureTimeInterval(this DateTime currentTime, TimeUnit timeUnit, int timeSpan)
        {
            if (timeSpan <= 0)
            {
                throw new ArgumentOutOfRangeException($"{timeSpan} should be greater than 0.");
            }

            DateTime? startTime = null;
            DateTime? endTime = null;
            switch (timeUnit)
            {
                case TimeUnit.Day:
                    DateTime dayPivotTime = currentTime.Date.AddDays(1);
                    startTime = new DateTime(dayPivotTime.Year, dayPivotTime.Month, dayPivotTime.Day, 0, 0, 0);
                    dayPivotTime = startTime.Value.AddDays(timeSpan - 1);
                    endTime = new DateTime(dayPivotTime.Year, dayPivotTime.Month, dayPivotTime.Day, 23, 59, 59);
                    break;
                case TimeUnit.Week:
                    var weekPivotNumber = 7 - (int)currentTime.DayOfWeek;
                    var weekPivotTime = currentTime.AddDays(weekPivotNumber);
                    startTime = new DateTime(weekPivotTime.Year, weekPivotTime.Month, weekPivotTime.Day, 0, 0, 0);
                    weekPivotTime = startTime.Value.AddDays((timeSpan * 7) - 1);
                    endTime = new DateTime(weekPivotTime.Year, weekPivotTime.Month, weekPivotTime.Day, 23, 59, 59);
                    break;
                case TimeUnit.Month:
                    var monthPivotTime = currentTime.AddMonths(1);
                    startTime = new DateTime(monthPivotTime.Year, monthPivotTime.Month, 1, 0, 0, 0);
                    monthPivotTime = startTime.Value.AddMonths(timeSpan).AddDays(-1);
                    endTime = new DateTime(monthPivotTime.Year, monthPivotTime.Month, monthPivotTime.Day, 23, 59, 59);
                    break;
                case TimeUnit.Quarter:
                    var quarterPivotNumber = (int)Math.Ceiling(currentTime.Month / 3.0);
                    var quarterPivotTime = new DateTime(currentTime.Year, 1, 1).AddMonths(quarterPivotNumber * 3);
                    startTime = new DateTime(quarterPivotTime.Year, quarterPivotTime.Month, 1, 0, 0, 0);
                    quarterPivotTime = startTime.Value.AddMonths(timeSpan * 3).AddDays(-1);
                    endTime = new DateTime(quarterPivotTime.Year, quarterPivotTime.Month, quarterPivotTime.Day, 23, 59, 59);
                    break;
                case TimeUnit.Year:
                    var yearPivotTime = currentTime.AddYears(1);
                    startTime = new DateTime(yearPivotTime.Year, 1, 1, 0, 0, 0);
                    yearPivotTime = startTime.Value.AddYears(timeSpan - 1);
                    endTime = new DateTime(yearPivotTime.Year, 12, 31, 23, 59, 59);
                    break;
                default:
                    startTime = currentTime;
                    endTime = currentTime;
                    break;
            }

            return new TimeInterval(startTime, endTime);
        }

        public static TimeInterval GetFutureTimeFromNowInterval(this DateTime currentTime, TimeUnit timeUnit, int timeSpan)
        {
            if (timeSpan <= 0)
            {
                throw new ArgumentOutOfRangeException($"{timeSpan} should be greater than 0.");
            }

            DateTime? startTime = currentTime.Date.AddDays(1);
            DateTime? endTime = null;
            switch (timeUnit)
            {
                case TimeUnit.Day:
                    endTime = startTime.Value.AddDays(timeSpan).AddSeconds(-1);
                    break;
                case TimeUnit.Week:
                    endTime = startTime.Value.AddDays(timeSpan * 7).AddSeconds(-1);
                    break;
                case TimeUnit.Month:
                    endTime = startTime.Value.AddMonths(timeSpan).AddSeconds(-1);
                    break;
                case TimeUnit.Quarter:
                    endTime = startTime.Value.AddMonths(timeSpan * 3).AddSeconds(-1);
                    break;
                case TimeUnit.Year:
                    endTime = startTime.Value.AddYears(timeSpan).AddSeconds(-1);
                    break;
                default:
                    endTime = startTime;
                    break;
            }

            return new TimeInterval(startTime, endTime);
        }

        public static TimeInterval GetPastTimeFromNowInterval(this DateTime currentTime, TimeUnit timeUnit, int timeSpan)
        {
            if (timeSpan <= 0)
            {
                throw new ArgumentOutOfRangeException($"{timeSpan} should be greater than 0.");
            }

            DateTime? startTime = null;
            DateTime? endTime = currentTime.Date;
            switch (timeUnit)
            {
                case TimeUnit.Day:
                    startTime = endTime.Value.AddDays(timeSpan * -1);
                    break;
                case TimeUnit.Week:
                    startTime = endTime.Value.AddDays(timeSpan * 7 * -1);
                    break;
                case TimeUnit.Month:
                    startTime = endTime.Value.AddMonths(timeSpan * -1);
                    break;
                case TimeUnit.Quarter:
                    startTime = endTime.Value.AddMonths(timeSpan * 3 * -1);
                    break;
                case TimeUnit.Year:
                    startTime = endTime.Value.AddYears(timeSpan * -1);
                    break;
                default:
                    startTime = endTime;
                    break;
            }

            return new TimeInterval(startTime, ((DateTime)endTime).AddSeconds(-1));
        }

        public static TimeInterval GetFutureTimeFromNowToEndOfInterval(this DateTime currentTime, TimeUnit timeUnit, int timeSpan, bool beforeOrAfter = true)
        {
            if (timeSpan < 0)
            {
                throw new ArgumentOutOfRangeException($"{timeSpan} should be greater or equal to 0.");
            }

            DateTime? startTime = null;
            DateTime? endTime = null;
            startTime = currentTime.Date;
            switch (timeUnit)
            {
                case TimeUnit.Day:
                    endTime = startTime.Value.AddDays(timeSpan).AddDays(1).AddSeconds(-1);
                    break;
                case TimeUnit.Week:
                    var weekPivotNumber = (((int)startTime.Value.DayOfWeek + 6) % 7) * -1;
                    var weekPivotTime = startTime.Value.Date.AddDays(weekPivotNumber); // current week start
                    endTime = weekPivotTime.AddDays(timeSpan * 7).AddDays(7).AddSeconds(-1);
                    break;
                case TimeUnit.Month:
                    endTime = startTime.Value.AddMonths(timeSpan);
                    endTime = new DateTime(endTime.Value.Year, endTime.Value.Month, 1).AddMonths(1).AddSeconds(-1);
                    break;
                case TimeUnit.Quarter:
                    var quarterPivotNumber = (int)Math.Ceiling(startTime.Value.Month / 3.0);
                    var quarterPivotTime = new DateTime(startTime.Value.Year, 1, 1).AddMonths((quarterPivotNumber - 1) * 3); // current quarter start
                    endTime = quarterPivotTime.AddMonths(timeSpan * 3).AddMonths(3).AddSeconds(-1);
                    break;
                case TimeUnit.Year:
                    endTime = new DateTime(startTime.Value.Year, 1, 1).AddYears(timeSpan).AddYears(1).AddSeconds(-1);
                    break;
                default:
                    endTime = startTime;
                    break;
            }

            startTime = beforeOrAfter ? (DateTime?)null : endTime.Value.AddSeconds(1);
            endTime = beforeOrAfter ? endTime : null;

            return new TimeInterval(startTime, endTime);
        }
    }
}
