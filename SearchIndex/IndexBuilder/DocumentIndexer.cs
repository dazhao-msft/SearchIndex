using CsvHelper;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
                ScoringProfiles = new List<ScoringProfile>() { Document.CreatePrimaryFieldFavoredScoringProfile() },
            };

            await serviceClient.Indexes.CreateAsync(definition);

            Console.WriteLine($"Index '{indexName}' was created.");
        }

        public static async Task UploadDocumentsAsync(ISearchServiceClient serviceClient, string indexName)
        {
            //
            // Uploads metadata
            //

            await UploadDocumentsCoreAsync(serviceClient, indexName, Document.ReadMetadataAsDocuments());

            //
            // Uploads entities
            //

            const int BufferSize = 5000;

            var buffer = new List<Document>();

            await foreach (var entity in ReadEntitiesAsDocumentsAsync())
            {
                buffer.Add(entity);

                if (buffer.Count == BufferSize)
                {
                    await UploadDocumentsCoreAsync(serviceClient, indexName, buffer);

                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
            {
                await UploadDocumentsCoreAsync(serviceClient, indexName, buffer);

                buffer.Clear();
            }
        }

        private static async Task UploadDocumentsCoreAsync(ISearchServiceClient serviceClient, string indexName, IReadOnlyCollection<Document> documents)
        {
            //
            // Uploads documents.
            //

            Console.WriteLine($"Uploading {documents.Count} documents...");

            var sw = Stopwatch.StartNew();

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

            sw.Stop();

            Console.WriteLine($"Uploading {documents.Count} documents completed. Elapsed: {sw.ElapsedMilliseconds} ms.");
        }

        private static async IAsyncEnumerable<Document> ReadEntitiesAsDocumentsAsync()
        {
            const string DataRootFolder = @"C:\Data\sample";

            var entityMetadataArray = new[]
            {
                new { EntityName = Document.AccountEntityName, EntityIdName = Document.AccountEntityIdName },
                new { EntityName = Document.BusinessUnitEntityName, EntityIdName = Document.BusinessUnitEntityIdName },
                new { EntityName = Document.ContactEntityName, EntityIdName = Document.ContactEntityIdName },
                new { EntityName = Document.LeadEntityName, EntityIdName = Document.LeadEntityIdName },
                new { EntityName = Document.OpportunityEntityName, EntityIdName = Document.OpportunityEntityIdName },
                new { EntityName = Document.OrganizationEntityName, EntityIdName = Document.OrganizationEntityIdName },
                new { EntityName = Document.TerritoryEntityName, EntityIdName = Document.TerritoryEntityIdName },
                new { EntityName = Document.UserEntityName, EntityIdName = Document.UserEntityIdName },
            };

            foreach (var entityMetadata in entityMetadataArray)
            {
                await foreach (var entity in ReadEntitiesAsDocumentsAsync(Path.Combine(DataRootFolder, entityMetadata.EntityName + ".csv"), entityMetadata.EntityName, entityMetadata.EntityIdName))
                {
                    yield return entity;
                }
            }
        }

        private static async IAsyncEnumerable<Document> ReadEntitiesAsDocumentsAsync(string csvFile, string entityName, string entityIdAttributeName)
        {
            var propertiesToIndex = Document.GetPropertiesToIndex(entityName);

            using (var streamReader = new StreamReader(csvFile))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    await foreach (IDictionary<string, object> entity in csvReader.GetRecordsAsync<dynamic>())
                    {
                        var document = new Document();

                        //
                        // Fill in the common fields.
                        //

                        document.EntityId = entity[entityIdAttributeName].ToString();
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
                                value = $"{value}{Document.SynonymDelimiter}{synonyms}";
                            }

                            if (propertyToIndex.CdsAttributeName == "address1_stateorprovince" && SynonymMap.StateOrProvinceSynonymMap.TryGetSynonyms(value.Trim(), out synonyms))
                            {
                                value = $"{value}{Document.SynonymDelimiter}{synonyms}";
                            }

                            if (propertyToIndex.CdsAttributeName == "address1_country" && SynonymMap.CountrySynonymMap.TryGetSynonyms(value.Trim(), out synonyms))
                            {
                                value = $"{value}{Document.SynonymDelimiter}{synonyms}";
                            }

                            if (entityName == "account" && propertyToIndex.CdsAttributeName == "name" && SynonymMap.OrganizationSynonymMap.TryGetSynonyms(value.Trim(), out synonyms))
                            {
                                value = $"{value}{Document.SynonymDelimiter}{synonyms}";
                            }

                            propertyToIndex.PropertyInfo.SetValue(document, value);
                        }

                        yield return document;
                    }
                }
            }
        }
    }
}
