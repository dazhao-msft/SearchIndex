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
        public static async Task ForceCreateIndexAsync(ISearchServiceClient serviceClient, string indexName)
        {
            if (await serviceClient.Indexes.ExistsAsync(indexName))
            {
                Console.WriteLine($"Index '{indexName}' exists. Deleting the existing index...");

                await serviceClient.Indexes.DeleteAsync(indexName);

                Console.WriteLine($"Index '{indexName}' was deleted.");
            }

            Console.WriteLine($"Creating index '{indexName}'...");

            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Document>(),
            };

            await serviceClient.Indexes.CreateAsync(definition);

            Console.WriteLine($"Index '{indexName}' was created.");
        }

        public static async Task UploadDocumentsAsync(ISearchServiceClient serviceClient, string indexName)
        {
            //
            // Prepare documents.
            //

            Console.WriteLine("Preparing documents...");

            var documents = new List<Document>();

            documents.AddRange(Document.ReadMetadataAsDocuments());
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\accounts.json", Document.AccountEntityName, Document.AccountEntityIdName));
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\contacts.json", Document.ContactEntityName, Document.ContactEntityIdName));
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\leads.json", Document.LeadEntityName, Document.LeadEntityIdName));
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\opportunities.json", Document.OpportunityEntityName, Document.OpportunityEntityIdName));
            documents.AddRange(await ReadEntitiesAsDocumentsAsync(@"Data\systemusers.json", Document.UserEntityName, Document.UserEntityIdName));

            //
            // Uploads documents.
            //

            Console.WriteLine("Uploading documents...");

            var batch = IndexBatch.Upload(documents);

            try
            {
                var indexClient = serviceClient.Indexes.GetClient(indexName);

                await indexClient.Documents.IndexAsync(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.WriteLine("Failed to index some of the documents: {0}", string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }

            Console.WriteLine("Uploading documents completed.");
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
                    string value = entity[propertyToIndex.CdsAttributeName].ToString();

                    //
                    // Appends a list of synonyms to the value if applicable.
                    //

                    if (propertyToIndex.CdsAttributeName == "address1_city" && SynonymMap.CitySynonymMap.TryGetSynonyms(value.Trim(), out string synonyms))
                    {
                        value = $"{value},{synonyms}";
                    }

                    if (propertyToIndex.CdsAttributeName == "address1_stateorprovince" && SynonymMap.StateOrProvinceSynonymMap.TryGetSynonyms(value.Trim(), out synonyms))
                    {
                        value = $"{value},{synonyms}";
                    }

                    if (propertyToIndex.CdsAttributeName == "address1_country" && SynonymMap.CountrySynonymMap.TryGetSynonyms(value.Trim(), out synonyms))
                    {
                        value = $"{value},{synonyms}";
                    }

                    propertyToIndex.PropertyInfo.SetValue(document, value);
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
