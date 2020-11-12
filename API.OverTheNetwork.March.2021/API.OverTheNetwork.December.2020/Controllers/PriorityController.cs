using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;
using ShareInvest.Statistical;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class PriorityController : ControllerBase
    {
        [HttpPut, ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutContextAsync([FromBody] Priority param)
        {
            if (Security.Collection.TryGetValue(param.Code, out Analysis analysis))
            {
                analysis.Current = param.Current > 0 ? param.Current : analysis.Current;
                analysis.Offer = param.Offer;
                analysis.Bid = param.Bid;

                return NoContent();
            }
            await new Task(() => Base.SendMessage(GetType(), param.Code));

            return Ok(param.Code);
        }
    }
}