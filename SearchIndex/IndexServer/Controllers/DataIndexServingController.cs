using IndexServer.Models;
using IndexServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Document = IndexModels.Document;

namespace IndexServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataIndexServingController : ControllerBase
    {
        private static readonly ISet<string> TokensToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "of",
            "is",
        };

        private static readonly IList<string> SearchableFields = GetSearchableFields();

        private readonly ISearchIndexClientProvider _searchIndexClientProvider;

        public DataIndexServingController(ISearchIndexClientProvider searchIndexClientProvider)
        {
            _searchIndexClientProvider = searchIndexClientProvider;
        }

        [HttpGet("Test")]
        public async Task<ActionResult<Document>> TestAsync([FromQuery] string query)
        {
            query = QueryRewrite(query);

            var searchParameters = new SearchParameters()
            {
                SearchFields = SearchableFields,
                Select = SearchableFields,
                HighlightFields = SearchableFields,
            };

            var searchIndexClient = _searchIndexClientProvider.CreateSearchIndexClient();

            var result = await searchIndexClient.Documents.SearchAsync<Document>(query, searchParameters);

            return Ok(result);
        }

        [HttpPost("Search")]
        public async Task<ActionResult<IEnumerable<MatchedTerm>>> SearchAsync([FromBody]DataIndexSearchRequest request)
        {
            await Task.Yield();

            return Ok(Enumerable.Empty<MatchedTerm>());
        }

        private static IList<string> GetSearchableFields()
        {
            return typeof(Document).GetProperties()
                                  .Where(p => p.GetCustomAttribute<IsSearchableAttribute>() != null)
                                  .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName)
                                  .ToList();
        }

        private static string QueryRewrite(string query)
        {
            // Super naive QR
            string[] tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return string.Concat(tokens.Where(token => !TokensToRemove.Contains(token)));
        }
    }
}
