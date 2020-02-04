using IndexModels;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Document = IndexModels.Document;
using Index = Microsoft.Azure.Search.Models.Index;

namespace IndexBuilder
{
    public static class DocumentIndexer
    {
        public static async Task<Index> ForceCreateIndexAsync(ISearchServiceClient serviceClient, string indexName)
        {
            if (await serviceClient.Indexes.ExistsAsync(indexName))
            {
                await serviceClient.Indexes.DeleteAsync(indexName);
            }

            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Document>(),
            };

            return await serviceClient.Indexes.CreateAsync(definition);
        }

        public static async Task UploadDocumentsAsync(ISearchServiceClient serviceClient, string indexName)
        {
            //
            // Prepare documents.
            //

            var documents = new List<Document>();

            documents.AddRange(await ReadEntitiesAsDocumentAsync(@"Data\accounts.json", EntityMetadata.Default["account"]));
            documents.AddRange(await ReadEntitiesAsDocumentAsync(@"Data\contacts.json", EntityMetadata.Default["contact"]));
            documents.AddRange(await ReadEntitiesAsDocumentAsync(@"Data\leads.json", EntityMetadata.Default["lead"]));
            documents.AddRange(await ReadEntitiesAsDocumentAsync(@"Data\opportunities.json", EntityMetadata.Default["opportunity"]));
            documents.AddRange(await ReadEntitiesAsDocumentAsync(@"Data\systemusers.json", EntityMetadata.Default["systemuser"]));

            //
            // Uploads documents.
            //

            var batch = IndexBatch.Upload(documents);

            try
            {
                var indexClient = serviceClient.Indexes.GetClient(indexName);

                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }

        private static async Task<IReadOnlyCollection<Document>> ReadEntitiesAsDocumentAsync(string file, EntityMetadata entityMetadata)
        {
            string contents = await File.ReadAllTextAsync(file);

            var entityList = JsonConvert.DeserializeObject<List<dynamic>>(contents);

            var propertiesToIndex = GetPropertiesToIndex(entityMetadata.EntityName);

            var documentList = new List<Document>();

            foreach (var entity in entityList)
            {
                var document = new Document();

                //
                // Fill in the common fields.
                //

                document.EntityType = entityMetadata.EntityName;
                document.EntityId = entity[entityMetadata.EntityIdName];
                document.EntityPrimaryField = entity[entityMetadata.EntityPrimaryFieldName];
                document.EntityAsJson = entity.ToString();

                //
                // Fill in the fields to be indexed.
                //

                foreach (var propertyToIndex in propertiesToIndex)
                {
                    propertyToIndex.PropertyInfo.SetValue(document, entity[propertyToIndex.AttributeName].ToString());
                }

                //
                // Add the document to list.
                //

                documentList.Add(document);
            }

            return documentList;
        }

        private static IReadOnlyList<(PropertyInfo PropertyInfo, string AttributeName)> GetPropertiesToIndex(string entityName)
        {
            var properties = typeof(Document).GetProperties();

            var result = new List<(PropertyInfo, string)>();

            foreach (var property in properties)
            {
                if (TryGetAttributeName(property, entityName, out string attributeName))
                {
                    result.Add((property, attributeName));
                }
            }

            return result;
        }

        private static bool TryGetAttributeName(PropertyInfo property, string entityName, out string attributeName)
        {
            attributeName = null;

            string jsonPropertyName = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;

            if (string.IsNullOrEmpty(jsonPropertyName))
            {
                return false;
            }

            string entityNamePrefix = $"{entityName}__";

            if (!jsonPropertyName.StartsWith(entityNamePrefix, StringComparison.Ordinal))
            {
                return false;
            }

            attributeName = jsonPropertyName.Substring(entityNamePrefix.Length);
            return true;
        }
    }
}
