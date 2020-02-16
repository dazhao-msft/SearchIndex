﻿using Microsoft.Azure.Search;
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

            string adminApiKey = configuration["SearchServiceAdminApiKey"];
            string searchServiceName = configuration["SearchServiceName"];
            string searchIndexName = configuration["SearchIndexName"];

            var serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));

            await DocumentIndexer.ForceCreateIndexAsync(serviceClient, searchIndexName);

            await DocumentIndexer.UploadDocumentsAsync(serviceClient, searchIndexName);
        }
    }
}
