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
    public class CodesController : ControllerBase
    {
        [HttpGet("{length}"), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContexts(int length)
        {
            switch (length)
            {
                case 6:
                    break;

                case 8:
                    return Ok(await context.Codes.Where(o => o.Code.Length == length).Select(o => new { o.Code, o.Name, o.MarginRate, o.MaturityMarketCap, o.Price }).AsNoTracking().ToListAsync());

                default:
                    return NotFound(length);
            }
            return BadRequest(length);
        }
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostContext([FromBody] IEnumerable<Codes> chart)
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x1C1B;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            return Ok();
        }
        [HttpPut("{code}"), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutContext(string code, [FromBody] Codes param)
        {
            if (await context.Codes.AnyAsync(o => o.Code.Equals(code)))
                context.Entry(param).State = EntityState.Modified;

            else
                context.Codes.Add(param);

            await context.BulkSaveChangesAsync();

            return Ok(code);
        }
        public CodesController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}