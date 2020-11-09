using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class CodesController : ControllerBase
    {
        [HttpPut, ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutContextAsync([FromBody] Codes codes)
        {
            
            return NoContent();
        }
    }
}