using IndexServer.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureSearchDocument = Microsoft.Azure.Search.Models.Document;
using Document = IndexModels.Document;

namespace IndexServer.Services
{
    public class SearchProvider : ISearchProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ISearchClientProvider _searchClientProvider;
        private readonly IEnumerable<ISearchResultHandler> _searchResultHandlers;
        private readonly ILogger<SearchProvider> _logger;

        public SearchProvider(
            IConfiguration configuration,
            ISearchClientProvider searchClientProvider,
            IEnumerable<ISearchResultHandler> searchResultHandlers,
            ILogger<SearchProvider> logger)
        {
            _configuration = configuration;
            _searchClientProvider = searchClientProvider;
            _searchResultHandlers = searchResultHandlers;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<MatchedTerm>> SearchAsync(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return Array.Empty<MatchedTerm>();
            }

            var searchParameters = new SearchParameters()
            {
                SearchMode = SearchMode.Any,
                SearchFields = Document.SearchableFields,
                HighlightFields = Document.SearchableFields,
                HighlightPreTag = "<em>",
                HighlightPostTag = "</em>"
            };

            IList<TokenInfo> tokens = null;
            using (var benchmarkScope = new BenchmarkScope(_logger, "analyzing text"))
            {
                var searchServiceClient = _searchClientProvider.CreateSearchServiceClient();
                tokens = (await searchServiceClient.Indexes.AnalyzeAsync(_configuration["SearchIndexName"], new AnalyzeRequest() { Text = searchText, Analyzer = Document.DocumentAnalyzerName })).Tokens;
            }

            IList<SearchResult<AzureSearchDocument>> searchResults = null;
            using (var benchmarkScope = new BenchmarkScope(_logger, "searching text"))
            {
                var searchIndexClient = _searchClientProvider.CreateSearchIndexClient();
                searchResults = (await searchIndexClient.Documents.SearchAsync(searchText, searchParameters)).Results;
            }

            var matchedTerms = new List<MatchedTerm>();

            var searchResultHandlerContext = new SearchResultHandlerContext(searchText, tokens, searchParameters, searchResults, matchedTerms);

            foreach (var searchResultHandler in _searchResultHandlers)
            {
                await searchResultHandler.ProcessAsync(searchResultHandlerContext);
            }

            return matchedTerms;
        }
    }
}
