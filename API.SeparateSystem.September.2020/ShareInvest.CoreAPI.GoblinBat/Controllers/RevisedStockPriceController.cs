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
        [HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetContext(string code)
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
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext([FromBody] RevisedStockPrice param)
        {
            if (await context.RevisedStockPrices.AnyAsync(o => o.Code.Equals(param.Code) && o.Date.Equals(param.Date)))
                context.Entry(param).State = EntityState.Modified;

            else
                context.RevisedStockPrices.Add(param);

            await context.BulkSaveChangesAsync();

            return Ok();
        }
        public RevisedStockPriceController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}