using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.CoreAPI;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class StrategicsController : ControllerBase
    {
        [HttpPut(Security.security), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutContext(string security, [FromBody] StocksStrategics param)
        {
            try
            {
                if (await context.Catalog.AnyAsync(o => o.Strategics.Equals(param.Strategics)) == false)
                {
                    var temp = param.Strategics.Split('.');

                    if (int.TryParse(temp[6], out int unit) && int.TryParse(temp[5], out int purchase) && int.TryParse(temp[4], out int profit) && int.TryParse(temp[3], out int trend) && int.TryParse(temp[2], out int iLong) && int.TryParse(temp[1], out int iShort))
                    {
                        context.Catalog.Add(new CatalogStrategics
                        {
                            Strategics = param.Strategics,
                            Short = iShort,
                            Long = iLong,
                            Trend = trend,
                            RealizeProfit = profit * 1e-4,
                            AdditionalPurchase = purchase * 1e-4,
                            QuoteUnit = unit,
                            LongShort = temp[7],
                            TrendType = temp[8],
                            Setting = temp[9]
                        });
                        await context.BulkSaveChangesAsync();
                    }
                }
                if (await context.StocksStrategics.AnyAsync(o => o.Code.Equals(param.Code) && o.Strategics.Equals(param.Strategics)))
                    context.Entry(param).State = EntityState.Modified;

                else
                    context.StocksStrategics.Add(param);

                await context.BulkSaveChangesAsync();

                return Ok((await context.Privacies.FirstAsync(o => o.Security.Equals(security))).Coin);
            }
            catch (Exception ex)
            {
                SendExceptionMessage(ex.Message);
            }
            return BadRequest();
        }
        [Conditional("DEBUG")]
        void SendExceptionMessage(string message) => Console.WriteLine(message);
        public StrategicsController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}