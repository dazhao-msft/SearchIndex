using Microsoft.Azure.Search;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Index = Microsoft.Azure.Search.Models.Index;

namespace IndexBuilder
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                          .AddUserSecrets(typeof(Program).Assembly)
                                                          .Build();

            var serviceClient = CreateSearchServiceClient(configuration);

            var indexName = GetIndexName(configuration);

            await ForceCreateNewIndexDefinitionAsync(serviceClient, indexName);
        }

        private static SearchServiceClient CreateSearchServiceClient(IConfiguration configuration)
        {
            string searchServiceName = configuration["SearchServiceName"];
            string adminApiKey = configuration["SearchServiceAdminApiKey"];

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
            return serviceClient;
        }

        private static string GetIndexName(IConfiguration configuration)
        {
            return configuration["SearchIndexName"];
        }

        private static async Task<Index> ForceCreateNewIndexDefinitionAsync(ISearchServiceClient serviceClient, string indexName)
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
    }
}
