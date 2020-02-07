using IndexServer.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Document = IndexModels.Document;

namespace IndexServer.Services
{
    public class SearchProvider : ISearchProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ISearchClientProvider _searchClientProvider;
        private readonly IEnumerable<ISearchResultHandler> _searchResultHandlers;

        public SearchProvider(
            IConfiguration configuration,
            ISearchClientProvider searchClientProvider,
            IEnumerable<ISearchResultHandler> searchResultHandlers)
        {
            _configuration = configuration;
            _searchClientProvider = searchClientProvider;
            _searchResultHandlers = searchResultHandlers;
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

            var searchServiceClient = _searchClientProvider.CreateSearchServiceClient();
            var tokens = (await searchServiceClient.Indexes.AnalyzeAsync(_configuration["SearchIndexName"], new AnalyzeRequest() { Text = searchText, Analyzer = AnalyzerName.AsString.EnMicrosoft })).Tokens;

            var searchIndexClient = _searchClientProvider.CreateSearchIndexClient();
            var searchResults = (await searchIndexClient.Documents.SearchAsync(searchText, searchParameters)).Results;

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
