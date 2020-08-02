using System;
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
        [HttpGet("{code:Minlength(6)}"), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContext(string code)
        {
            var context = await this.context.Codes.FindAsync(code);

            if (context != null)
                return Ok(context);

            return NotFound(code);
        }
        [HttpGet("{length:int}"), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContexts(int length)
        {
            if (length == 6 || length == 8)
                return Ok(await context.Codes.Where(o => o.Code.Length == length).Select(o => new { o.Code, o.Name, o.MarginRate, o.MaturityMarketCap, o.Price }).AsNoTracking().ToListAsync());

            else
                return NotFound(length);
        }
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetContexts()
        {
            if (Registry.Retentions.Any(o => string.IsNullOrEmpty(o.Key)))
                Registry.RemoveRetentions();

            return Ok(Registry.Retentions);
        }
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
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
        [HttpPut("{code}"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutContext(string code, [FromBody] Codes param)
        {
            if (string.IsNullOrEmpty(code) == false && Registry.Retentions.ContainsKey(code) == false && string.IsNullOrEmpty(param.MaturityMarketCap) == false)
            {
                if (await context.Codes.AnyAsync(o => o.Code.Equals(code)))
                    context.Entry(param).State = EntityState.Modified;

                else
                    context.Codes.Add(param);

                string retentions = string.Empty;
                await context.BulkSaveChangesAsync();

                try
                {
                    switch (code.Length)
                    {
                        case 6:
                            retentions = await context.Stocks.Where(o => o.Code.Equals(code)).AsNoTracking().MaxAsync(o => o.Date);
                            break;

                        case int length when length == 8 && (code.StartsWith("101") || code.StartsWith("106")):
                            retentions = await context.Futures.Where(o => o.Code.Equals(code)).AsNoTracking().MaxAsync(o => o.Date);
                            break;

                        case int length when length == 8 && (code.StartsWith("2") || code.StartsWith("3")):
                            retentions = await context.Options.Where(o => o.Code.Equals(code)).AsNoTracking().MaxAsync(o => o.Date);
                            break;

                        case int length when length == 8 && code.StartsWith("1"):
                            return Ok(code);

                        default:
                            return BadRequest(code);
                    }
                    if (retentions != null && (retentions.Equals(string.Empty) || retentions.Length > 6 && string.Compare(retentions.Substring(0, 6), DateTime.Now.ToString("yyMMdd")) < 0))
                        Registry.Retentions[code] = retentions;
                }
                catch (Exception ex)
                {
                    code = ex.TargetSite.Name;
                }
            }
            return Ok(code);
        }
        public CodesController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}