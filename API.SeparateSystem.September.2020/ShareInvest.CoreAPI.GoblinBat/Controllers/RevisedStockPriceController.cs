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
    public class RevisedStockPriceController : ControllerBase
    {
        [HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetContext(string key, string code)
        {
            if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
            {
                var hash = new Queue<ConfirmRevisedStockPrice>();

                if (await context.RevisedStockPrices.AnyAsync(o => o.Code.Equals(code)))
                {
                    foreach (var sp in context.RevisedStockPrices.Where(o => o.Code.Equals(code)).OrderBy(o => o.Date).AsNoTracking())
                        if (double.TryParse(sp.Rate, out double rate))
                            hash.Enqueue(new ConfirmRevisedStockPrice
                            {
                                Date = sp.Date,
                                Price = sp.Price,
                                Revise = sp.Revise,
                                Rate = rate
                            });
                    return Ok(hash);
                }
                return NoContent();
            }
            return BadRequest();
        }
        [HttpPost(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostContext(string key, [FromBody] RevisedStockPrice param)
        {
            if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
            {
                if (await context.RevisedStockPrices.AnyAsync(o => o.Code.Equals(param.Code) && o.Date.Equals(param.Date)))
                    context.Entry(param).State = EntityState.Modified;

                else
                    context.RevisedStockPrices.Add(param);

                await context.BulkSaveChangesAsync();

                return Ok();
            }
            return BadRequest();
        }
        public RevisedStockPriceController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}