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
    public class DaysController : ControllerBase
    {
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContexts() => Ok(await context.Days.LongCountAsync());
        [HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContext(string code)
        {
            var date = context.Days.Where(o => o.Code.Equals(code)).AsNoTracking();

            return Ok(new Retention
            {
                Code = code,
                LastDate = await date.MaxAsync(o => o.Date),
                FirstDate = await date.MinAsync(o => o.Date)
            });
        }
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostContext([FromBody] Queue<Days> chart)
        {
            await context.BulkInsertAsync<Queue<Days>>(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x43AD;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            return Ok();
        }
        public DaysController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}