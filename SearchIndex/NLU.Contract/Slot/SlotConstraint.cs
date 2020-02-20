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
    [JsonConverter(typeof(SlotContraintConverter))]
    public class SlotConstraint
    {
        /// <summary>
        /// slot contraint id
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        /// <summary>
        /// slot contraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public virtual SlotConstraintType Type => SlotConstraintType.None;
    }

    public class SlotContraintConverter : JsonConverter
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

            var slotConstraint = default(SlotConstraint);
            if (Enum.TryParse<SlotConstraintType>(jsonObject["type"].Value<string>(), out var slotConstraintType))
            {
                switch (slotConstraintType)
                {
                    case SlotConstraintType.EntityType:
                        slotConstraint = new EntityTypeSlotConstraint();
                        break;
                    case SlotConstraintType.EntitySet:
                        slotConstraint = new EntitySetSlotConstraint();
                        break;
                    case SlotConstraintType.EntityBelongsTo:
                        slotConstraint = new EntityBelongsToSlotConstraint();
                        break;
                    case SlotConstraintType.EntityCount:
                        slotConstraint = new EntityCountSlotConstraint();
                        break;
                    case SlotConstraintType.Keyword:
                        slotConstraint = new KeywordSlotConstraint();
                        break;
                    case SlotConstraintType.Regex:
                        slotConstraint = new RegexSlotConstraint();
                        break;
                    case SlotConstraintType.Example:
                        slotConstraint = new ExampleSlotConstraint();
                        break;
                }
            }

            serializer.Populate(jsonObject.CreateReader(), slotConstraint);

            return slotConstraint;
        }
    }
}
