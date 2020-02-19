using Microsoft.Azure.Search;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IndexBuilder
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                              .AddUserSecrets(typeof(Program).Assembly)
                                                              .Build();

                string adminApiKey = configuration["SearchServiceAdminApiKey"];
                string searchServiceName = configuration["SearchServiceName"];
                string searchIndexName = configuration["SearchIndexName"];
                string dataRootFolder = configuration["DataRootFolder"];

                var serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));

                await DocumentIndexer.ForceCreateIndexAsync(serviceClient, searchIndexName);

                await DocumentIndexer.UploadDocumentsAsync(serviceClient, searchIndexName, dataRootFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.ToString()}");
            }
            finally
            {
                sw.Stop();

                Console.WriteLine($"Job completed in {sw.Elapsed.TotalSeconds} seconds.");
            }
        }
    }
}
