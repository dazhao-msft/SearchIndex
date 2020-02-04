using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\accounts.json", "account", "accountid"));
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\contacts.json", "contact", "contactid"));
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\leads.json", "lead", "leadid"));
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\opportunities.json", "opportunity", "opportunityid"));
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\systemusers.json", "systemuser", "systemuserid"));

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

        private static async Task<IReadOnlyCollection<Document>> ReadEntitiesAsDocumentsAsync(string file, string entityName, string entityIdAttributeName)
        {
            var documents = new List<Document>();

            var propertiesToIndex = Document.GetPropertiesToIndex(entityName);

            var entities = JsonConvert.DeserializeObject<List<dynamic>>(await File.ReadAllTextAsync(file));

            foreach (var entity in entities)
            {
                var document = new Document();

                //
                // Fill in the common fields.
                //

                document.EntityId = entity[entityIdAttributeName];
                document.EntityName = entityName;

                //
                // Fill in the fields to be indexed.
                //

                foreach (var propertyToIndex in propertiesToIndex)
                {
                    propertyToIndex.PropertyInfo.SetValue(document, entity[propertyToIndex.CdsAttributeName].ToString());
                }

                //
                // Add the document to list.
                //

                documents.Add(document);
            }

            return documents;
        }
    }
}
