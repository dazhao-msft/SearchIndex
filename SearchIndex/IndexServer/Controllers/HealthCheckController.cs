using Microsoft.AspNetCore.Mvc;

namespace IndexServer.Controllers
{
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet("health/live")]
        public ActionResult GetLiveness() => Ok();

        [HttpGet("health/ready")]
        public ActionResult GetReadiness() => Ok();
    }
}