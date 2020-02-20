//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    public static partial class EntityTypeString
    {
        // entity type string
        public const string None = "None";
        public const string KeyPhrase = "KeyPhrase";
        public const string LuciaTerm = "LuciaTerm";

        public const string CDMEntityPrefix = "bizqa.cdm";

        public const string CDMTable = "bizqa.cdm.table";

        // For all entities which extracted from DB but without an explicit type defined
        public const string CDMCellValue = "bizqa.cdm.cell_value";

        // Table column types for testing codes
        public const string ColumnPrefix = "bizqa.cdm.table_column.";
        public const string OpportunityColumn = ColumnPrefix + "opportunity";
        public const string AccountColumn = ColumnPrefix + "account";
        public const string OpportunityNameCellValue = "bizqa.cdm.cell_value.opportunity_name";
        public const string AccountNameCellValue = "bizqa.cdm.cell_value.account_name";
        public const string UserFullNameCellValue = "bizqa.cdm.cell_value.user_fullname";
        public const string ContactFullNameCellValue = "bizqa.cdm.cell_value.contact_fullname";

        public static readonly IReadOnlyList<string> AllTypes = new List<string>();

        // Below convertible types are for designed for intent triggering purpose only, other components need to evaluate before using them
        public static readonly IReadOnlyList<List<string>> MutualConvertibleTypes = new List<List<string>>()
        {
            new List<string>()
            {
                Age,
                AgeInYear,
                AgeInQuarter,
                AgeInMonth,
                AgeInWeek,
                AgeInDay,
            },
            new List<string>()
            {
                AgeRange,
                AgeRangeInYear,
                AgeRangeInQuarter,
                AgeRangeInMonth,
                AgeRangeInWeek,
                AgeRangeInDay,
            },
            new List<string>()
            {
                DateRange,
                DateRangeInYear,
                DateRangeInQuarter,
                DateRangeInMonth,
                DateRangeInDay,
            },
            new List<string>()
            {
                Date,
                DateInYear,
                DateInQuarter,
                DateInMonth,
                DateInDay,
            },
            new List<string>()
            {
                Location,
                City,
                StateOrProvince,
                Country,
                Continent,
            },
        };

        // Below convertible types are for designed for intent triggering purpose only, other components need to evaluate before using them
        public static readonly IReadOnlyDictionary<string, List<string>> DirectionalConvertibleTypes = new Dictionary<string, List<string>>()
        {
            { NumberRange, new List<string>() { CurrencyRange } },
        };

        private static object s_lock = new object();

        static EntityTypeString()
        {
            ((List<string>)AllTypes).Add(None);
            ((List<string>)AllTypes).Add(KeyPhrase);
            ((List<string>)AllTypes).Add(LuciaTerm);
        }

        public static void AddType(string typeName)
        {
            lock (s_lock)
            {
                if (((List<string>)AllTypes).IndexOf(typeName) < 0)
                {
                    ((List<string>)AllTypes).Add(typeName);
                }
            }
        }
    }
}
