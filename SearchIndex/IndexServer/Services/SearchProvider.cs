using IndexServer.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IReadOnlyCollection<TokenInfo>> AnalyzeAsync(string searchText, string analyzerName)
        {
            var analyzeRequest = new AnalyzeRequest()
            {
                Text = searchText,
                Analyzer = analyzerName,
            };

            IList<TokenInfo> tokens = null;
            using (var benchmarkScope = new BenchmarkScope(_logger, "analyzing text"))
            {
                var searchServiceClient = _searchClientProvider.CreateSearchServiceClient();
                tokens = (await searchServiceClient.Indexes.AnalyzeAsync(_configuration["SearchIndexName"], analyzeRequest)).Tokens;
            }

            return new List<TokenInfo>(tokens);
        }

        public async Task<IReadOnlyCollection<MatchedTerm>> SearchAsync(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return Array.Empty<MatchedTerm>();
            }

            var analyzeRequest = new AnalyzeRequest()
            {
                Text = searchText,
                Analyzer = Document.DefaultAnalyzerName,
            };

            IList<TokenInfo> searchTokens = null;
            using (var benchmarkScope = new BenchmarkScope(_logger, "analyzing text"))
            {
                var searchServiceClient = _searchClientProvider.CreateSearchServiceClient();
                searchTokens = (await searchServiceClient.Indexes.AnalyzeAsync(_configuration["SearchIndexName"], analyzeRequest)).Tokens;
            }

            var searchParameters = new SearchParameters()
            {
                SearchMode = SearchMode.Any,
                SearchFields = Document.SearchableFields,
                ScoringProfile = Document.PrimaryFieldFavoredScoringProfile,
                HighlightFields = Document.SearchableFields,
                HighlightPreTag = "<em>",
                HighlightPostTag = "</em>",
                Top = 500,
            };

            var searchResults = new List<SearchResult<AzureSearchDocument>>();

            using (var benchmarkScope = new BenchmarkScope(_logger, "searching text"))
            {
                var searchIndexClient = _searchClientProvider.CreateSearchIndexClient();

                var currentResult = await searchIndexClient.Documents.SearchAsync(searchText, searchParameters);
                searchResults.AddRange(currentResult.Results);

                while (currentResult.ContinuationToken != null)
                {
                    currentResult = await searchIndexClient.Documents.ContinueSearchAsync(currentResult.ContinuationToken);
                    searchResults.AddRange(currentResult.Results);
                }
            }

            var matchedTerms = new HashSet<MatchedTerm>();

            var searchResultHandlerContext = new SearchResultHandlerContext(searchText, searchTokens.ToList(), searchParameters, searchResults, matchedTerms);

            foreach (var searchResultHandler in _searchResultHandlers)
            {
                await searchResultHandler.ProcessAsync(searchResultHandlerContext);
            }

            //
            // Merge term bindings
            //

            var consolidatedMatchedTerms = new List<MatchedTerm>();

            foreach (var matchedTerm in matchedTerms)
            {
                int index = consolidatedMatchedTerms.FindIndex(p => StringComparer.OrdinalIgnoreCase.Equals(p.Text, matchedTerm.Text) && p.StartIndex == matchedTerm.StartIndex && p.Length == matchedTerm.Length);

                if (index < 0)
                {
                    consolidatedMatchedTerms.Add(new MatchedTerm()
                    {
                        Text = matchedTerm.Text,
                        StartIndex = matchedTerm.StartIndex,
                        Length = matchedTerm.Length,
                        TermBindings = new HashSet<TermBinding>(),
                    });

                    index = consolidatedMatchedTerms.Count - 1;
                }

                consolidatedMatchedTerms[index].TermBindings.UnionWith(matchedTerm.TermBindings);
            }

            return consolidatedMatchedTerms;
        }
    }
}
