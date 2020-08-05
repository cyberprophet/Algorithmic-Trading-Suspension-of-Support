using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using ShareInvest.CoreAPI;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class StocksController : ControllerBase
    {
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContexts() => Ok(await context.Stocks.LongCountAsync());
        [HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContext(string code) => Ok(new Retention
        {
            Code = code,
            LastDate = await context.Stocks.Where(o => o.Code.Equals(code)).AsNoTracking().MaxAsync(o => o.Date)
        });
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext([FromBody] Queue<Stocks> chart)
        {
            await context.BulkInsertAsync<Queue<Stocks>>(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x43AD;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            return Ok();
        }
        public StocksController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}