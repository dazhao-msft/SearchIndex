using Microsoft.Azure.Search;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace IndexServer.Services
{
    public class SearchClientProvider : ISearchClientProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SearchClientProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public ISearchServiceClient CreateSearchServiceClient()
        {
            var credentials = new SearchCredentials(_configuration["SearchServiceAdminApiKey"]);
            var httpClient = _httpClientFactory.CreateClient();

            return new SearchServiceClient(credentials, httpClient, false)
            {
                SearchServiceName = _configuration["SearchServiceName"],
            };
        }

        public ISearchIndexClient CreateSearchIndexClient()
        {
            var credentials = new SearchCredentials(_configuration["SearchServiceQueryApiKey"]);
            var httpClient = _httpClientFactory.CreateClient();

            return new SearchIndexClient(credentials, httpClient, false)
            {
                SearchServiceName = _configuration["SearchServiceName"],
                IndexName = _configuration["SearchIndexName"],
            };
        }
    }
}
