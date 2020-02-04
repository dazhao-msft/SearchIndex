using Microsoft.Azure.Search;
using Microsoft.Extensions.Configuration;

namespace IndexServer.Services
{
    public class SearchIndexClientProvider : ISearchIndexClientProvider
    {
        private readonly string _serviceName;
        private readonly string _indexName;
        private readonly string _queryApiKey;

        public SearchIndexClientProvider(IConfiguration configuration)
        {
            _serviceName = configuration["SearchServiceName"];
            _indexName = configuration["SearchIndexName"];
            _queryApiKey = configuration["SearchServiceQueryApiKey"];
        }

        public ISearchIndexClient CreateSearchIndexClient()
        {
            return new SearchIndexClient(_serviceName, _indexName, new SearchCredentials(_queryApiKey));
        }
    }
}
