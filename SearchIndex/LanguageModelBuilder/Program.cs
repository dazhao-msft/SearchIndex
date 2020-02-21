using CsvHelper;
using IndexModels;
using Microsoft.BizQA.NLU.QueryRewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageModelBuilder
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.Configure<QueryRewriteOptions>(configuration.GetSection(nameof(QueryRewriteOptions)));

            var serviceProvider = services.BuildServiceProvider(true);

            var queryRewriter = QueryRewriteActivator.GetQueryRewriter(serviceProvider.GetRequiredService<IOptions<QueryRewriteOptions>>().Value);

            var model = await BuildLanguageModelAsync(queryRewriter, configuration["DataRootFolder"]);

            string query = "contact of ford";

            foreach (var token in queryRewriter.Rewrite(query).LastStepRewriteTokens.Where(p => !p.IsStopword))
            {
                var bindings = model.GetTermBindings(token.Text);
            }
        }

        private static async Task<LanguageModel> BuildLanguageModelAsync(IQueryRewriter queryRewriter, string dataRootFolder)
        {
            var model = new LanguageModel();

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
                await BuildLanguageModelCoreAsync(model, queryRewriter, Path.Combine(dataRootFolder, entityMetadata.EntityName + ".csv"), entityMetadata.EntityName, entityMetadata.EntityIdName);
            }

            return model;
        }

        private static async Task BuildLanguageModelCoreAsync(LanguageModel model, IQueryRewriter queryRewriter, string csvFile, string entityName, string entityIdAttributeName)
        {
            var propertiesToIndex = Document.GetPropertiesToIndex(entityName);

            int count = 0;

            using (var streamReader = new StreamReader(csvFile))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    await foreach (IDictionary<string, object> entity in csvReader.GetRecordsAsync<dynamic>())
                    {
                        count++;

                        if (!Guid.TryParse(entity[entityIdAttributeName]?.ToString(), out var entityId))
                        {
                            continue;
                        }

                        foreach (var propertyToIndex in propertiesToIndex)
                        {
                            string value = entity[propertyToIndex.CdsAttributeName]?.ToString();

                            if (string.IsNullOrEmpty(value))
                            {
                                continue;
                            }

                            var result = queryRewriter.Rewrite(value);

                            foreach (var token in result.LastStepRewriteTokens)
                            {
                                model.Add(token.Text, entityName, propertyToIndex.CdsAttributeName, entityId);
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"{count} records have been processed in {csvFile}.");
        }
    }
}
