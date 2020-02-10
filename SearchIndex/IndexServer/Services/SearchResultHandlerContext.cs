using IndexServer.Models;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace IndexServer.Services
{
    public class SearchResultHandlerContext
    {
        public SearchResultHandlerContext(
            string searchText,
            IReadOnlyList<TokenInfo> searchTokens,
            SearchParameters searchParameters,
            IReadOnlyList<SearchResult<Document>> searchResults,
            ICollection<MatchedTerm> matchedTerms)
        {
            SearchText = searchText;
            SearchTokens = searchTokens;
            SearchParameters = searchParameters;
            SearchResults = searchResults;
            MatchedTerms = matchedTerms;
        }

        public string SearchText { get; }

        public IReadOnlyList<TokenInfo> SearchTokens { get; }

        public SearchParameters SearchParameters { get; }

        public IReadOnlyList<SearchResult<Document>> SearchResults { get; }

        public ICollection<MatchedTerm> MatchedTerms { get; }
    }
}
