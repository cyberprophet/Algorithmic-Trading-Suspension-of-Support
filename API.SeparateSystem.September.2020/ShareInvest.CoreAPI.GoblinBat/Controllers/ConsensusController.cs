using System;
using System.Collections.Generic;
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
    public class ConsensusController : ControllerBase
    {
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext([FromBody] IEnumerable<Consensus> param)
        {
            foreach (var consensus in param)
            {
                if (await context.Consensus.AnyAsync(o => o.Code.Equals(consensus.Code) && o.Date.Equals(consensus.Date) && o.Quarter.Equals(consensus.Quarter)))
                    context.Entry(consensus).State = EntityState.Modified;

                else
                    context.Consensus.Add(consensus);

                await context.BulkSaveChangesAsync();
            }
            return Ok();
        }
        [HttpPut, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutContext([FromBody] EstimatedPrice param)
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
            if (await context.Estimate.AnyAsync(o => o.Code.Equals(param.Code) && o.Strategics.Equals(param.Strategics)))
                context.Entry(param).State = EntityState.Modified;

            else
                context.Estimate.Add(param);

            await context.BulkSaveChangesAsync();

            return Ok();
        }
        [HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContext(string code) => Ok(await context.Consensus.Where(o => o.Code.Equals(code)).AsNoTracking().ToListAsync());
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContext()
        {
            var now = DateTime.Now;

            return Ok(await context.Estimate.Where(o => o.Date.Equals(now.Hour < 0x11 ? now.AddDays(-1).ToString(Security.format) : now.ToString(Security.format))).AsNoTracking().Select(o => new
            {
                o.Code,
                o.FirstQuarter,
                o.SecondQuarter,
                o.ThirdQuarter,
                o.Quarter,
                o.TheNextYear,
                o.TheYearAfterNext
            }).ToListAsync());
        }
        public ConsensusController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}