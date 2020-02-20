//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// slot match schema
    /// </summary>
    [JsonConverter(typeof(SlotMatchConverter))]
    public class SlotMatch
    {
        /// <summary>
        /// slot match id
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        /// <summary>
        /// slot match type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public virtual SlotMatchType Type => SlotMatchType.None;
    }

    public class SlotMatchConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SlotMatch);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("write mode is not supported");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var slotMatch = default(SlotMatch);
            if (Enum.TryParse<SlotMatchType>(jsonObject["type"].Value<string>(), out var slotMatchType))
            {
                switch (slotMatchType)
                {
                    case SlotMatchType.Keyword:
                        slotMatch = new KeywordSlotMatch();
                        break;
                    case SlotMatchType.Regex:
                        slotMatch = new RegexSlotMatch();
                        break;
                    case SlotMatchType.Function:
                        slotMatch = new FunctionSlotMatch();
                        break;
                    case SlotMatchType.Example:
                        slotMatch = new ExampleSlotMatch();
                        break;
                }
            }

            serializer.Populate(jsonObject.CreateReader(), slotMatch);

            return slotMatch;
        }
    }
}
