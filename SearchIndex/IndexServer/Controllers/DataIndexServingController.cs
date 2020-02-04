using IndexServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndexServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataIndexServingController : ControllerBase
    {
        [HttpPost("Search")]
        public async Task<ActionResult<IEnumerable<MatchedTerm>>> SearchAsync([FromBody]DataIndexSearchRequest request)
        {
            await Task.Yield();

            return Ok(Enumerable.Empty<MatchedTerm>());
        }
    }
}
