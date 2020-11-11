using System.Collections.Generic;
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
            if (Security.SecuritiesCompany == 0x4F && (codes.Code.Length == 6 || codes.Code.Length == 8 && codes.Code[0] > 1))
                await Security.Client.PutContextAsync(codes);

            if (codes.MaturityMarketCap.Contains(transaction_suspension) == false && Security.Collection.ContainsKey(codes.Code) == false)
            {
                Security.Collection[codes.Code] = new Statistical.OpenAPI.Stocks
                {
                    Code = codes.Code,
                    Collection = new Queue<Collect>()
                };
                return Ok();
            }
            return NoContent();
        }
        const string transaction_suspension = "거래정지";
    }
}