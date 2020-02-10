using IndexServer.Models;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace IndexServer.Services
{
    public class SearchResultHandlerContext
    {
        public SearchResultHandlerContext(
            string searchText,
            IList<TokenInfo> queryTokens,
            SearchParameters searchParameters,
            IList<SearchResult<Document>> searchResults,
            ICollection<MatchedTerm> matchedTerms)
        {
            SearchText = searchText;
            QueryTokens = queryTokens;
            SearchParameters = searchParameters;
            SearchResults = searchResults;
            MatchedTerms = matchedTerms;
        }

        public string SearchText { get; }

        public IList<TokenInfo> QueryTokens { get; }

        public SearchParameters SearchParameters { get; }

        public IList<SearchResult<Document>> SearchResults { get; }

        public ICollection<MatchedTerm> MatchedTerms { get; }
    }
}
