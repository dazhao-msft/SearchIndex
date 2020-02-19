using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IndexServer.Controllers
{
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthCheckController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("health/live")]
        public ActionResult Liveness() => Ok(new { SearchServiceName = _configuration["SearchServiceName"], SearchIndexName = _configuration["SearchIndexName"] });

        [HttpGet("health/ready")]
        public ActionResult Readiness() => Ok(new { SearchServiceName = _configuration["SearchServiceName"], SearchIndexName = _configuration["SearchIndexName"] });
    }
}