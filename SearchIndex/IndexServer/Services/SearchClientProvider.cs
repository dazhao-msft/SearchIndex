using Microsoft.Azure.Search;
using Microsoft.Extensions.Configuration;

namespace IndexServer.Services
{
    public class SearchClientProvider : ISearchClientProvider
    {
        private readonly string _serviceName;
        private readonly string _indexName;
        private readonly string _adminApiKey;
        private readonly string _queryApiKey;

        public SearchClientProvider(IConfiguration configuration)
        {
            _serviceName = configuration["SearchServiceName"];
            _indexName = configuration["SearchIndexName"];
            _adminApiKey = configuration["SearchServiceAdminApiKey"];
            _queryApiKey = configuration["SearchServiceQueryApiKey"];
        }

        public ISearchServiceClient CreateSearchServiceClient()
        {
            return new SearchServiceClient(_serviceName, new SearchCredentials(_adminApiKey));
        }

        public ISearchIndexClient CreateSearchIndexClient()
        {
            return new SearchIndexClient(_serviceName, _indexName, new SearchCredentials(_queryApiKey));
        }
    }
}
