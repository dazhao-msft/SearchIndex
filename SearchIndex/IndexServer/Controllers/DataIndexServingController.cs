using IndexModels;
using IndexServer.Models;
using IndexServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndexServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataIndexServingController : ControllerBase
    {
        private readonly ISearchProvider _searchProvider;
        private readonly ILogger<DataIndexServingController> _logger;

        public DataIndexServingController(ISearchProvider searchProvider, ILogger<DataIndexServingController> logger)
        {
            _searchProvider = searchProvider;
            _logger = logger;
        }

        [HttpGet("Analyze")]
        public async Task<ActionResult<IReadOnlyCollection<MatchedTerm>>> AnalyzeAsync([FromQuery] string query, [FromQuery] string analyzer)
        {
            return Ok(await _searchProvider.AnalyzeAsync(query, string.IsNullOrEmpty(analyzer) ? Document.DefaultAnalyzerName : analyzer));
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IReadOnlyCollection<MatchedTerm>>> SearchAsync([FromQuery] string query)
        {
            return Ok(await SearchCoreAsync(query));
        }

        [HttpPost("Search")]
        public async Task<ActionResult<IReadOnlyCollection<MatchedTerm>>> SearchAsync([FromBody] DataIndexSearchRequest request)
        {
            return Ok(await SearchCoreAsync(request.Query));
        }

        private async Task<IReadOnlyCollection<MatchedTerm>> SearchCoreAsync(string query)
        {
            using (var benchmarkScope = new BenchmarkScope(_logger, $"Trace ID: {HttpContext.TraceIdentifier} | searching text"))
            {
                _logger.LogInformation($"Trace ID: {HttpContext.TraceIdentifier} | Query: {query}");

                var matchedTerms = await _searchProvider.SearchAsync(query);

                _logger.LogInformation($"Trace ID: {HttpContext.TraceIdentifier} | Count of matched terms: {matchedTerms.Count}");

                return matchedTerms;
            }
        }
    }
}
