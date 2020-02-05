using IndexServer.Models;
using IndexServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Document = IndexModels.Document;

namespace IndexServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataIndexServingController : ControllerBase
    {
        private const string HighlightPreTag = "<em>";
        private const string HighlightPostTag = "</em>";

        private readonly ISearchIndexClientProvider _searchIndexClientProvider;
        private readonly ILogger<DataIndexServingController> _logger;

        public DataIndexServingController(ISearchIndexClientProvider searchIndexClientProvider, ILogger<DataIndexServingController> logger)
        {
            _searchIndexClientProvider = searchIndexClientProvider;
            _logger = logger;
        }

        [HttpGet("Test")]
        public async Task<ActionResult<IReadOnlyCollection<MatchedTerm>>> TestAsync([FromQuery] string query)
        {
            return Ok(await SearchCoreAsync(query));
        }

        [HttpPost("Search")]
        public async Task<ActionResult<IEnumerable<MatchedTerm>>> SearchAsync([FromBody]DataIndexSearchRequest request)
        {
            return Ok(await SearchCoreAsync(request.Query));
        }

        private async Task<IReadOnlyCollection<MatchedTerm>> SearchCoreAsync(string query)
        {
            _logger.LogInformation($"Trace ID: {HttpContext.TraceIdentifier} | Query: {query}");

            if (string.IsNullOrEmpty(query))
            {
                return Array.Empty<MatchedTerm>();
            }

            var searchIndexClient = _searchIndexClientProvider.CreateSearchIndexClient();

            var searchParameters = new SearchParameters()
            {
                SearchMode = SearchMode.Any,
                SearchFields = Document.SearchableFields,
                HighlightFields = Document.SearchableFields,
                HighlightPreTag = HighlightPreTag,
                HighlightPostTag = HighlightPostTag,
            };

            var matchedTerms = new List<MatchedTerm>();

            var searchResults = (await searchIndexClient.Documents.SearchAsync(query, searchParameters)).Results;

            if (searchResults.Count > 0)
            {
                foreach (var searchResult in searchResults)
                {
                    string cdsEntityName = searchResult.Document[Document.EntityNameFieldName].ToString();

                    foreach (var highlight in searchResult.Highlights)
                    {
                        if (Document.TryResolveCdsAttributeName(highlight.Key, cdsEntityName, out string cdsAttributeName))
                        {
                            string fieldValue = searchResult.Document[highlight.Key].ToString();

                            foreach (string fragment in highlight.Value)
                            {
                                int startIndex = fragment.IndexOf(HighlightPreTag, StringComparison.Ordinal) + HighlightPreTag.Length;
                                int length = fragment.LastIndexOf(HighlightPostTag, StringComparison.Ordinal) - startIndex;
                                string matchedText = fragment.Substring(startIndex, length).Replace(HighlightPreTag, null).Replace(HighlightPostTag, null);

                                var matchedTerm = new MatchedTerm
                                {
                                    Text = matchedText,
                                    StartIndex = query.IndexOf(matchedText, StringComparison.OrdinalIgnoreCase),
                                    Length = matchedText.Length,
                                    TermBindings = new HashSet<TermBinding>(),
                                };

                                matchedTerm.TermBindings.Add(new TermBinding()
                                {
                                    BindingType = BindingType.InstanceValue,
                                    SearchScope = new SearchScope()
                                    {
                                        Table = cdsEntityName,
                                        Column = cdsAttributeName,
                                    },
                                    Value = fieldValue,
                                    IsExactlyMatch = StringComparer.OrdinalIgnoreCase.Equals(fieldValue, matchedText),
                                    IsSynonymMatch = false,
                                });

                                matchedTerms.Add(matchedTerm);
                            }
                        }
                    }
                }
            }

            _logger.LogInformation($"Trace ID: {HttpContext.TraceIdentifier} | Count of matched terms: {matchedTerms.Count}");

            return matchedTerms;
        }
    }
}
