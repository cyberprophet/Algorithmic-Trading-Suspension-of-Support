using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.CoreAPI;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class StocksController : ControllerBase
    {
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetContexts()
        {
            if (Registry.Retentions != null && Registry.Retentions.Count > 0)
            {
                var registry = Registry.Retentions.First(o => o.Key.Length == 6);
                var param = string.Concat(registry.Key, ";", registry.Value);

                if (Registry.Retentions.Remove(registry.Key))
                    return Ok(param);
            }
            return NotFound();
        }
        public StocksController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}