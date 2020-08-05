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
    public class FuturesController : ControllerBase
    {
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContexts() => Ok(await context.Futures.LongCountAsync());
        [HttpGet(Security.routeRetention), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContext(string code) => Ok(new Retention
        {
            Code = code,
            LastDate = await context.Futures.Where(o => o.Code.Equals(code)).AsNoTracking().MaxAsync(o => o.Date)
        });
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext([FromBody] Queue<Futures> chart)
        {
            await context.BulkInsertAsync<Queue<Futures>>(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x43AD;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            return Ok();
        }
        public FuturesController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}