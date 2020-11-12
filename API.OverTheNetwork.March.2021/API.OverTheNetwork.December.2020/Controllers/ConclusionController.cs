using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;
using ShareInvest.Statistical;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class ConclusionController : ControllerBase
    {
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostContextAsync([FromBody] Conclusion param)
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
                    Time = param.Time,
                    Datum = string.Concat(param.Price, ';', param.Volume)
                });
                if (order is not null)
                    return order;
            }
            return NoContent();
        }
    }
}