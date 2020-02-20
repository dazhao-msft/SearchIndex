//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.BizQA.NLU.Contract
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionType
    {
        // why question type
        Why,

        // how many question type
        HowMany,

        // how question type
        How,

        // when question type
        When,

        // how much question type
        HowMuch,

        // how old question type
        HowOld,

        // who question type
        Who,

        // which question type
        Which,

        // what question type
        What,

        // where question type
        Where,
    }
}
