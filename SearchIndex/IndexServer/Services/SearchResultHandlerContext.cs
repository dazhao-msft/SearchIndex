using IndexServer.Models;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace IndexServer.Services
{
    public class SearchResultHandlerContext
    {
        public SearchResultHandlerContext(
            string searchText,
            SearchParameters searchParameters,
            IList<SearchResult<Document>> searchResults,
            IList<MatchedTerm> matchedTerms)
        {
            SearchText = searchText;
            SearchParameters = searchParameters;
            SearchResults = searchResults;
            MatchedTerms = matchedTerms;
        }

        public string SearchText { get; }

        public SearchParameters SearchParameters { get; }

        public IList<SearchResult<Document>> SearchResults { get; }

        public IList<MatchedTerm> MatchedTerms { get; }
    }
}
