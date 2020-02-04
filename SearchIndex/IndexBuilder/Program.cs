using Microsoft.Azure.Search;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;


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

            await DocumentIndexer.ForceCreateIndexAsync(serviceClient, indexName);

            await DocumentIndexer.UploadDocumentsAsync(serviceClient, indexName);
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
    }
}
