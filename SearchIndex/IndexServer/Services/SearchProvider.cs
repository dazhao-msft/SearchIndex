using IndexServer.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Document = IndexModels.Document;

namespace IndexServer.Services
{
    public class SearchProvider : ISearchProvider
    {
        private readonly ISearchIndexClientProvider _searchIndexClientProvider;
        private readonly IEnumerable<ISearchResultHandler> _searchResultHandlers;

        public SearchProvider(
            ISearchIndexClientProvider searchIndexClientProvider,
            IEnumerable<ISearchResultHandler> searchResultHandlers)
        {
            _searchIndexClientProvider = searchIndexClientProvider;
            _searchResultHandlers = searchResultHandlers;
        }

        public async Task<IReadOnlyCollection<MatchedTerm>> SearchAsync(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return Array.Empty<MatchedTerm>();
            }

            searchText = searchText.TrimEnd(new[] { ' ', ',', '.', '?', '!' });

            var searchParameters = new SearchParameters()
            {
                SearchMode = SearchMode.Any,
                SearchFields = Document.SearchableFields,
                HighlightFields = Document.SearchableFields,
                HighlightPreTag = string.Empty,
                HighlightPostTag = string.Empty,
            };

            var searchIndexClient = _searchIndexClientProvider.CreateSearchIndexClient();
            var searchResults = (await searchIndexClient.Documents.SearchAsync(searchText, searchParameters)).Results;

            var matchedTerms = new List<MatchedTerm>();

            var context = new SearchResultHandlerContext(searchText, searchParameters, searchResults, matchedTerms);

            foreach (var handler in _searchResultHandlers)
            {
                await handler.ProcessAsync(context);
            }

            return matchedTerms;
        }
    }
}
