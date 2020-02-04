using IndexServer.Models;
using IndexServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Document = IndexModels.Document;

namespace IndexServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataIndexServingController : ControllerBase
    {
        private readonly ISearchIndexClientProvider _searchIndexClientProvider;

        public DataIndexServingController(ISearchIndexClientProvider searchIndexClientProvider)
        {
            _searchIndexClientProvider = searchIndexClientProvider;
        }

        [HttpGet("Test")]
        public async Task<ActionResult<IReadOnlyCollection<MatchedTerm>>> TestAsync([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Ok();
            }

            var searchIndexClient = _searchIndexClientProvider.CreateSearchIndexClient();

            var searchParameters = new SearchParameters()
            {
                SearchFields = Document.SearchableFields,
                HighlightFields = Document.SearchableFields,
            };

            var matchedTerms = new List<MatchedTerm>();

            foreach (string token in Tokenize(query))
            {
                var searchResults = (await searchIndexClient.Documents.SearchAsync(token, searchParameters)).Results;

                if (searchResults.Count > 0)
                {
                    var matchedTerm = new MatchedTerm();

                    matchedTerm.Text = token;
                    matchedTerm.StartIndex = query.IndexOf(token);
                    matchedTerm.Length = token.Length;
                    matchedTerm.TermBindings = new HashSet<TermBinding>();

                    foreach (var searchResult in searchResults)
                    {
                        string cdsEntityName = searchResult.Document[Document.EntityNameFieldName].ToString();

                        foreach (var highlight in searchResult.Highlights)
                        {
                            if (Document.TryResolveCdsAttributeName(highlight.Key, cdsEntityName, out string cdsAttributeName))
                            {
                                matchedTerm.TermBindings.Add(new TermBinding()
                                {
                                    BindingType = BindingType.InstanceValue,
                                    SearchScope = new SearchScope()
                                    {
                                        Table = cdsEntityName,
                                        Column = cdsAttributeName,
                                    },
                                    Value = searchResult.Document[highlight.Key].ToString(),
                                });
                            }
                        }
                    }

                    matchedTerms.Add(matchedTerm);
                }
            }

            return matchedTerms;
        }

        [HttpPost("Search")]
        public async Task<ActionResult<IEnumerable<MatchedTerm>>> SearchAsync([FromBody]DataIndexSearchRequest request)
        {
            await Task.Yield();

            return Ok(Enumerable.Empty<MatchedTerm>());
        }

        #region Tokenizer

        private static readonly ISet<string> TokensToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "of",
            "is",
        };

        private static IReadOnlyCollection<string> Tokenize(string query)
        {
            string[] tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return tokens.Where(token => !TokensToRemove.Contains(token)).Distinct().ToList();
        }

        #endregion Tokenizer
    }
}
