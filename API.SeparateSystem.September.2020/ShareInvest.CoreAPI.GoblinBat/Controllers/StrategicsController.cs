using System;
using System.Diagnostics;
using System.Linq;
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

                    if (Enum.TryParse(temp[0], out Strategics initial))
                    {
                        switch (initial)
                        {
                            case Strategics.ST:
                                if (int.TryParse(temp[8], out int stNet) && int.TryParse(temp[6], out int stOp) && int.TryParse(temp[4], out int stSales) && int.TryParse(temp[2], out int stTrend) && int.TryParse(temp[1], out int calendar))
                                    context.Catalog.Add(new CatalogStrategics
                                    {
                                        Strategics = param.Strategics,
                                        Short = calendar,
                                        Long = stSales,
                                        Trend = stTrend,
                                        RealizeProfit = stOp * 1e-2,
                                        AdditionalPurchase = stNet * 1e-2,
                                        LongShort = temp[3],
                                        TrendType = temp[5],
                                        Setting = temp[7]
                                    });
                                break;

                            case Strategics.TS:
                                if (int.TryParse(temp[6], out int unit) && int.TryParse(temp[5], out int purchase) && int.TryParse(temp[4], out int profit) && int.TryParse(temp[3], out int trend) && int.TryParse(temp[2], out int iLong) && int.TryParse(temp[1], out int iShort))
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
                                break;

                            default:
                                return BadRequest();
                        }
                        await context.BulkSaveChangesAsync();
                    }
                    else
                        return BadRequest();
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
        [HttpPost(Security.strategics), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostContext(string strategics, [FromBody] ConfirmStrategics confirm)
        {
            if (Enum.TryParse(strategics, out Strategics initial))
                switch (initial)
                {
                    case Strategics.TS:
                    case Strategics.ST:
                        return Ok(await context.StocksStrategics.AnyAsync(o => o.Strategics.Equals(confirm.Strategics) && o.Code.Equals(confirm.Code) && o.Date.Equals(confirm.Date)));

                    default:
                        return BadRequest();
                }
            return BadRequest();
        }
        [HttpGet(Security.strategics), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetContext(string strategics)
        {
            try
            {
                if (Enum.TryParse(strategics, out Strategics initial))
                    switch (initial)
                    {
                        case Strategics.TS:
                            return Ok(await context.Catalog.Where(o => o.Strategics.StartsWith(strategics)).AsNoTracking().Select(o => new
                            {
                                o.Short,
                                o.Long,
                                o.Trend,
                                o.RealizeProfit,
                                o.AdditionalPurchase,
                                o.QuoteUnit,
                                o.LongShort,
                                o.TrendType,
                                o.Setting
                            }).ToListAsync());

                        case Strategics.ST:
                            return Ok(await context.Catalog.Where(o => o.Strategics.StartsWith(strategics)).AsNoTracking().Select(o => new
                            {
                                o.Short,
                                o.Trend,
                                o.LongShort,
                                o.Long,
                                o.TrendType,
                                o.RealizeProfit,
                                o.Setting,
                                o.AdditionalPurchase
                            }).ToListAsync());

                        default:
                            break;
                    }
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