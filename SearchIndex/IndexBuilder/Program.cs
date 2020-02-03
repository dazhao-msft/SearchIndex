using Microsoft.Azure.Search;
using Microsoft.Extensions.Configuration;
using Index = Microsoft.Azure.Search.Models.Index;

namespace IndexBuilder
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddUserSecrets(typeof(Program).Assembly);
            var configuration = builder.Build();

            var serviceClient = CreateSearchServiceClient(configuration);
            var indexName = GetIndexName(configuration);

            CreateOrUpdateIndexDefinition(serviceClient, indexName);
        }

        private static string GetIndexName(IConfiguration configuration)
        {
            return configuration["SearchIndexName"];
        }

        private static SearchServiceClient CreateSearchServiceClient(IConfiguration configuration)
        {
            string searchServiceName = configuration["SearchServiceName"];
            string adminApiKey = configuration["SearchServiceAdminApiKey"];

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
            return serviceClient;
        }

        private static Index CreateOrUpdateIndexDefinition(ISearchServiceClient serviceClient, string indexName)
        {
            if (serviceClient.Indexes.Exists(indexName))
            {
                serviceClient.Indexes.Delete(indexName);
            }

            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Document>(),
            };

            return serviceClient.Indexes.Create(definition);
        }
    }
}
