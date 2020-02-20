//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// slot contraint schema
    /// </summary>
    [JsonConverter(typeof(SlotTransformConverter))]
    public class SlotTransform
    {
        /// <summary>
        /// slot transform id
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        /// <summary>
        /// slot transform type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public virtual SlotTransformType Type => SlotTransformType.None;
    }

    public class SlotTransformConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SlotConstraint);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("write mode is not supported");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var slotTransform = default(SlotTransform);
            if (Enum.TryParse<SlotTransformType>(jsonObject["type"].Value<string>(), out var slotConstraintType))
            {
                switch (slotConstraintType)
                {
                    case SlotTransformType.KeyPhraseValue:
                        slotTransform = new KeyPhraseValueSlotTransform();
                        break;
                    case SlotTransformType.DateRangeValue:
                        slotTransform = new DateRangeValueSlotTransform();
                        break;
                }
            }

            serializer.Populate(jsonObject.CreateReader(), slotTransform);

            return slotTransform;
        }
    }
}
