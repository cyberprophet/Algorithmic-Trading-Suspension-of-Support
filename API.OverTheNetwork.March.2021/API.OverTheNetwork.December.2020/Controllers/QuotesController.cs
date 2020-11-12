using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;
using ShareInvest.Statistical;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class QuotesController : ControllerBase
    {
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostContextAsync([FromBody] Quotes param)
        {
            if (Security.Collection.TryGetValue(param.Code, out Analysis analysis))
            {
                IActionResult order = null;

                switch (await analysis.AnalyzeTheDataAsync(param))
                {
                    case Catalog.OpenAPI.Order o:
                        order = Ok(o);
                        break;
                }
                analysis.Collection.Enqueue(new Collect
                {
                    Time = param.Datum.Substring(0, 6),
                    Datum = param.Datum[7..]
                });
                if (order is not null)
                    return order;
            }
            return NoContent();
        }
    }
}