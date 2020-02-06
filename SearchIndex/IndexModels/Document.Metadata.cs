using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IndexModels
{
    public partial class Document
    {
        public const string MetadataEntityName = "systemmetadata";
        public const string MetadataEntityIdName = "systemmetadataid";
        public const string MetadataEntityEntityFieldName = MetadataEntityName + FieldNameDelimiter + "entity";
        public const string MetadataEntityAttributeFieldName = MetadataEntityName + FieldNameDelimiter + "attribute";

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty(MetadataEntityEntityFieldName)]
        public string MetadataEntity { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty(MetadataEntityAttributeFieldName)]
        public string MetadataAttribute { get; set; }

        #region Helper

        public static IReadOnlyCollection<Document> ReadMetadataAsDocuments()
        {
            var documents = new List<Document>();

            foreach (var searchableField in SearchableFields)
            {
                string[] entityAndAttribute = searchableField.Split(FieldNameDelimiter);

                if (entityAndAttribute.Length != 2)
                {
                    throw new InvalidOperationException($"searchable field name is incorrect: {searchableField}");
                }

                documents.Add(new Document()
                {
                    EntityId = Guid.NewGuid().ToString(),
                    EntityName = MetadataEntityName,
                    MetadataEntity = entityAndAttribute[0],
                    MetadataAttribute = entityAndAttribute[1],
                });
            }

            return documents;
        }

        #endregion Helper
    }
}
