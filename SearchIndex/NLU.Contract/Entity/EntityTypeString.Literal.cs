//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// Defined well known literal entities.
    /// </summary>
    public static partial class EntityTypeString
    {
        /// <summary>
        /// Age
        /// </summary>
        public const string AgeEntityTypeId = "00852a40-1d72-4ac2-89d3-4a1f5995030b";
        public const string Age = "bltin.Age";

        public const string AgeInYearEntityId = "de233eab-4c90-44c7-b428-4dfa61dd7fb3";
        public const string AgeInYear = "bltin.AgeInYear";

        public const string AgeInQuarterEntityId = "88307ba9-7c3d-4ffb-b14d-9c63a39a417c";
        public const string AgeInQuarter = "bltin.AgeInQuarter";

        public const string AgeInMonthEntityId = "4d9a24d2-1bad-47e1-aa29-655c783ae0ba";
        public const string AgeInMonth = "bltin.AgeInMonth";

        public const string AgeInWeekEntityId = "00cfc9e0-913b-4ccc-91e9-915ec47ba81e";
        public const string AgeInWeek = "bltin.AgeInWeek";

        public const string AgeInDayEntityId = "ab30cd64-c207-4d2a-9d4e-365fec84f309";
        public const string AgeInDay = "bltin.AgeInDay";

        /// <summary>
        /// Age Range
        /// </summary>
        public const string AgeRangeEntityId = "4386ec76-65cf-4835-823f-d847d535ad85";
        public const string AgeRange = "bltin.AgeRange";

        public const string AgeRangeInYearEntityId = "2a3a42f1-b081-4da6-9c3c-9d1850a8b0d3";
        public const string AgeRangeInYear = "bltin.AgeRangeInYear";

        public const string AgeRangeInQuarterEntityId = "8f017099-20c9-41a6-a283-b489065b0e4d";
        public const string AgeRangeInQuarter = "bltin.AgeRangeInQuarter";

        public const string AgeRangeInMonthEntityId = "5a659190-6633-438d-8d8d-456ba03bedc9";
        public const string AgeRangeInMonth = "bltin.AgeRangeInMonth";

        public const string AgeRangeInWeekEntityId = "e134cebc-7016-4c97-944d-4428eb56e825";
        public const string AgeRangeInWeek = "bltin.AgeRangeInWeek";

        public const string AgeRangeInDayEntityId = "f09a9abb-da4c-4e6d-ae9f-3c90062c3341";
        public const string AgeRangeInDay = "bltin.AgeRangeInDay";

        /// <summary>
        /// Currency
        /// </summary>
        public const string CurrencyEntityId = "a1553fc6-fdec-4938-8bf8-df7af7568a9b";
        public const string Currency = "bltin.Currency";

        /// <summary>
        /// Currency Range
        /// </summary>
        public const string CurrencyRangeEntityId = "8b0a141d-8e34-461f-81f7-9661825d2646";
        public const string CurrencyRange = "bltin.CurrencyRange";

        /// <summary>
        /// Number
        /// </summary>
        public const string NumberEntityId = "29d8de09-996e-4adb-a525-a7cca0f3bab1";
        public const string Number = "bltin.Number";

        /// <summary>
        /// Number Range
        /// </summary>
        public const string NumberRangeEntityId = "0220f825-604e-4423-a7c1-b47ed95dbcc4";
        public const string NumberRange = "bltin.NumberRange";

        /// <summary>
        /// Phone number
        /// </summary>
        public const string PhoneNumberEntityId = "ac8ef82b-fec4-42c6-aaa4-f62230cd8770";
        public const string PhoneNumber = "bltin.PhoneNumber";

        /// <summary>
        /// Email
        /// </summary>
        public const string EmailEntityId = "d683f57b-d224-47ab-aca9-be69fc5549c9";
        public const string Email = "bltin.Email";

        /// <summary>
        /// DateTime
        /// </summary>
        public const string DateTimeEntityId = "b9ea8f78-3bcc-4e5a-b589-9dbe3fed8c40";
        public const string DateTime = "bltin.DateTime";

        /// <summary>
        /// DateRange
        /// </summary>
        public const string DateRangeEntityId = "c18f54f0-7c8e-4e78-bb74-65311a52ace6";
        public const string DateRange = "bltin.DateRange";

        public const string DateRangeInYearEntityId = "9e0387d0-68c1-4225-9a08-f7aab18207fa";
        public const string DateRangeInYear = "bltin.DateRangeInYear";

        public const string DateRangeInQuarterEntityId = "bf752e83-e0fc-4776-9ba3-7c6b0e0db150";
        public const string DateRangeInQuarter = "bltin.DateRangeInQuarter";

        public const string DateRangeInMonthEntityId = "d8075918-60b3-4c8b-b6bd-12b67216914e";
        public const string DateRangeInMonth = "bltin.DateRangeInMonth";

        public const string DateRangeInDayEntityId = "20d63725-a63b-416e-b16f-ad280dbe9aad";
        public const string DateRangeInDay = "bltin.DateRangeInDay";

        /// <summary>
        /// FiscalQuarter
        /// </summary>
        public const string DateEntityId = "82247535-931c-4ce3-b741-a54c124fe1de";
        public const string Date = "bltin.Date";

        public const string DateInYearEntityId = "a42601fc-e62a-4b83-a7b8-a8c2ee56d29c";
        public const string DateInYear = "bltin.DateInYear";

        public const string DateInQuarterEntityId = "1631efcc-293a-4ee1-911b-0cd85b7da3ff";
        public const string DateInQuarter = "bltin.DateInQuarter";

        public const string DateInMonthEntityId = "7580a21e-4485-43ac-88f1-9c98b7b589d3";
        public const string DateInMonth = "bltin.DateInMonth";

        public const string DateInDayEntityId = "1ea6c83b-5bf7-4d60-b22b-7556532eb30a";
        public const string DateInDay = "bltin.DateInDay";

        /// <summary>
        /// Person Name
        /// </summary>
        public const string PersonNameEntityId = "a0c61a99-1d08-4aa7-87e9-bb0f08be87d7";
        public const string PersonName = "bltin.PersonName";

        /// <summary>
        /// Location
        /// </summary>
        public const string LocationEntityId = "18444a65-c49a-4d4f-bb16-029581babcd5";
        public const string Location = "bltin.Location";

        public const string ContinentEntityId = "1c18f319-eb15-415c-8c81-a490be3dd5cf";
        public const string Continent = "bltin.Continent";

        public const string CountryEntityId = "b406378b-3e8a-46e1-aea2-7659ee07cd45";
        public const string Country = "bltin.Country";

        public const string StateOrProvinceEntityId = "cc602db5-b0f5-450b-956e-059995ca39df";
        public const string StateOrProvince = "bltin.StateOrProvince";

        public const string CityEntityId = "74dd8d30-f77c-4964-ac79-1d3aeb8ceae9";
        public const string City = "bltin.City";

        public const string PostalCodeEntityId = "c43bf4b7-fef3-4766-8db6-4efd6c518804";
        public const string PostalCode = "bltin.PostalCode";

        /// <summary>
        /// Product Name
        /// </summary>
        public const string ProductNameEntityId = "551f0a7a-e09f-4c3B-bd48-7ff1671cd114";
        public const string ProductName = "bltin.ProductName";

        /// <summary>
        /// Organization Name
        /// </summary>
        public const string OrganizationEntityId = "af4b1cfc-a567-418c-b1e6-6b648a9d349d";
        public const string Organization = "bltin.Organization";

        /// <summary>
        /// Equal Operator
        /// </summary>
        public const string CompareOperatorId = "246aedad-0a2c-4e9e-9c9e-d7a4be842169";
        public const string CompareOperator = "bltin.CompareOperator";

        /// <summary>
        /// Aggregate Operator
        /// </summary>
        public const string AggregateOperatorId = "c830475f-4ba2-4673-a82f-3968efe15c73";
        public const string AggregateOperator = "bltin.AggregateOperator";

        /// <summary>
        /// Logical Operator
        /// </summary>
        public const string LogicalOperatorId = "ef74a626-3d80-471b-a05f-427ebc969c73";
        public const string LogicalOperator = "bltin.LogicalOperator";

        /// <summary>
        /// Location Operator
        /// </summary>
        public const string LocationOperatorId = "8ba0c9c3-e6aa-44a2-92d1-447d2bb0a41e";
        public const string LocationOperator = "bltin.LocationOperator";

        /// <summary>
        /// Question Type
        /// </summary>
        public const string QuestionTypeId = "94111a0a-51d6-493c-a2b0-8ef6b84c3e61";
        public const string QuestionType = "bltin.QuestionType";

        /// <summary>
        /// TopN
        /// </summary>
        public const string TopNTypeId = "27a4546f-6414-4fd4-afab-426abf1c3708";
        public const string TopN = "bltin.TopN";

        /// <summary>
        /// Enum
        /// </summary>
        public const string EnumEntityId = "851fc47d-9d44-411f-808a-f35b61ce4af6";
        public const string Enum = "bltin.Enum";

        /// <summary>
        /// Boolean
        /// </summary>
        public const string BooleanEntityId = "61094ec7-86d4-41c7-af05-91136b3c1197";
        public const string Boolean = "bltin.Boolean";

        /// <summary>
        /// BizqaCdmTableColumn
        /// </summary>
        public const string BizqaCdmTableColumn = "bizqa.cdm.table_column";

        /// <summary>
        /// Contains all literal entities and their keys.
        /// </summary>
        public static readonly IDictionary<string, string> AllTypePairs = new Dictionary<string, string>()
        {
            { AgeEntityTypeId, Age },
            { AgeInYearEntityId, AgeInYear },
            { AgeInQuarterEntityId, AgeInQuarter },
            { AgeInMonthEntityId, AgeInMonth },
            { AgeInWeekEntityId, AgeInWeek },
            { AgeInDayEntityId, AgeInDay },
            { AgeRangeEntityId, AgeRange },
            { AgeRangeInYearEntityId, AgeRangeInYear },
            { AgeRangeInQuarterEntityId, AgeRangeInQuarter },
            { AgeRangeInMonthEntityId, AgeRangeInMonth },
            { AgeRangeInWeekEntityId, AgeRangeInWeek },
            { AgeRangeInDayEntityId, AgeRangeInDay },
            { CurrencyEntityId, Currency },
            { CurrencyRangeEntityId, CurrencyRange },
            { NumberEntityId, Number },
            { NumberRangeEntityId, NumberRange },
            { PhoneNumberEntityId, PhoneNumber },
            { EmailEntityId, Email },
            { DateTimeEntityId, DateTime },
            { DateRangeEntityId, DateRange },
            { DateRangeInYearEntityId, DateRangeInYear },
            { DateRangeInQuarterEntityId, DateRangeInQuarter },
            { DateRangeInMonthEntityId, DateRangeInMonth },
            { DateRangeInDayEntityId, DateRangeInDay },
            { DateEntityId, Date },
            { DateInYearEntityId, DateInYear },
            { DateInQuarterEntityId, DateInQuarter },
            { DateInMonthEntityId, DateInMonth },
            { DateInDayEntityId, DateInDay },
            { PersonNameEntityId,  PersonName },
            { LocationEntityId, Location },
            { ContinentEntityId, Continent },
            { CountryEntityId, Country },
            { StateOrProvinceEntityId, StateOrProvince },
            { CityEntityId, City },
            { PostalCodeEntityId, PostalCode },
            { ProductNameEntityId, ProductName },
            { OrganizationEntityId, Organization },
            { CompareOperatorId, CompareOperator },
            { AggregateOperatorId, AggregateOperator },
            { LogicalOperatorId, LogicalOperator },
            { LocationOperatorId, LocationOperator },
            { QuestionTypeId, QuestionType },
            { TopNTypeId, TopN },
            { EnumEntityId, Enum },
            { BooleanEntityId, Boolean },
        };

        public static bool IsLiteralEntityId(string entityId)
        {
            return string.IsNullOrWhiteSpace(entityId) ? false : AllTypePairs.ContainsKey(entityId);
        }

        /// <summary>
        /// Get literal entity id from entity name.
        /// </summary>
        public static string GetLiteralEntityIdFromName(string entityName)
        {
            foreach (var pair in AllTypePairs)
            {
                if (string.Compare(entityName, pair.Value, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return pair.Key;
                }
            }

            return string.Empty;
        }
    }
}
