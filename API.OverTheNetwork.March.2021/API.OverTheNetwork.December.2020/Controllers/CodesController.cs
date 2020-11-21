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
        [HttpPut, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutContextAsync([FromBody] Codes param)
        {
            if (Security.Collection.ContainsKey(param.Code))
                return Ok(param.Name);

            else
            {
                if (Security.SecuritiesCompany == 0x4F && (param.Code.Length == 6 || param.Code.Length == 8 && param.Code[0] > '1')
                    && await Security.Client.PutContextAsync(param) is string code)
                    Base.SendMessage(code, Security.Collection.Count, GetType());

                if (param.MaturityMarketCap.Contains(transaction_suspension) == false)
                {
                    switch (Security.SecuritiesCompany)
                    {
                        case 'O' when param.Code.Length == 6:
                            Security.Collection[param.Code] = new Statistical.OpenAPI.Stocks
                            {
                                Code = param.Code,
                                Current = 0,
                                Offer = 0,
                                Bid = 0,
                                Collection = new Queue<Collect>(0x800)
                            };
                            break;

                        case 'O' when param.Code.Length == 8 && param.Code[0] == '1':
                            Security.Collection[param.Code] = new Statistical.OpenAPI.Futures
                            {
                                Code = param.Code,
                                Current = param.Code[1] == '0' ? 0D : 0,
                                Offer = param.Code[1] == '0' ? 0D : 0,
                                Bid = param.Code[1] == '0' ? 0D : 0,
                                Collection = new Queue<Collect>(0x800)
                            };
                            break;

                        case 'O' when param.Code.Length == 8 && param.Code[0] > '1':
                            Security.Collection[param.Code] = new Statistical.OpenAPI.Options
                            {
                                Code = param.Code,
                                Current = 0D,
                                Offer = 0D,
                                Bid = 0D,
                                Collection = new Queue<Collect>(0x800)
                            };
                            break;
                    }
                    if (Security.Strategics.TryGetValue(param.Code, out Interface.IStrategics strategics)
                        && Security.Collection.TryGetValue(param.Code, out Statistical.Analysis analysis))
                        analysis.Strategics = strategics;

                    return Ok(param.Name);
                }
            }
            return Ok();
        }
        const string transaction_suspension = "거래정지";
    }
}