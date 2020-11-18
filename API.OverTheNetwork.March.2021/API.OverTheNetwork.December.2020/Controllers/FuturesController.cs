using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class FuturesController : ControllerBase
    {
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContextAsync([FromBody] Queue<Futures> param)
        {
            if (await Security.Client.PostContextAsync(param) > 0xC8)
            {
                var peek = param.Peek();
                Base.SendMessage(peek.Code, peek.Retention, GetType());
            }
            return Ok();
        }
    }
}