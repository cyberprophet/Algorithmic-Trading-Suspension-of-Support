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
    [ApiController, Route(route), Produces(produces)]
    public class ChartsController : ControllerBase
    {
        [HttpDelete("{code}"), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteContext(string code, [FromBody] Security param)
        {
            if (new Security().IsAdministrator(param.Administrator))
            {
                switch (code.Length)
                {
                    case int length when param.Length == 8 && (length == 6 || length == 8):
                        await context.BulkDeleteAsync(context.Days.Where(o => o.Code.Equals(code)));
                        break;

                    case int length when length == 8 && code.StartsWith(futures) && param.Length == 0xF:
                        await context.BulkDeleteAsync(context.Futures.Where(o => o.Code.Equals(code)));
                        break;

                    case int length when length == 8 && (code.StartsWith(call) || code.StartsWith(put)) && param.Length == 0xF:
                        await context.BulkDeleteAsync(context.Options.Where(o => o.Code.Equals(code)));
                        break;

                    case int length when length == 6 && param.Length == 0xF:
                        await context.BulkDeleteAsync(context.Stocks.Where(o => o.Code.Equals(code)));
                        break;

                    case int length when param.Length == 0x63 && (length == 6 || length == 8):
                        await context.BulkDeleteAsync(context.Codes.Where(o => o.Code.Equals(code)));
                        break;

                    default:
                        return NotFound();
                }
                return Ok(code);
            }
            return BadRequest();
        }
        [HttpGet("{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetContext()
        {
            if (Registry.Codes != null && Registry.Codes.Count > 0)
                return Ok(Registry.Codes.Dequeue());

            return StatusCode(0xCD);
        }
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContexts()
        {
            if (Registry.Codes != null && Registry.Codes.Count > 0)
                return Ok(Registry.CodesDictionary.Values);

            var context = await this.context.Codes.AsNoTracking().ToArrayAsync();

            if (context != null)
            {
                Registry.Codes.Clear();
                Registry.CodesDictionary.Clear();

                foreach (var ct in context.OrderByDescending(o => o.MaturityMarketCap))
                {
                    Registry.Codes.Enqueue(ct.Code);
                    Registry.CodesDictionary[ct.Code] = new Tuple<string, string>(ct.Name, ct.MaturityMarketCap);
                }
                return Ok(Registry.CodesDictionary);
            }
            return NotFound();
        }
        [HttpPost("days/{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext(string code, [FromBody] IEnumerable<Days> chart)
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x6BD;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            Registry.Retentions[code] = chart.FirstOrDefault(o => string.IsNullOrEmpty(o.Retention) == false).Retention;
            code = Registry.Codes.Count > 0 ? Registry.Codes.Dequeue() : string.Empty;

            if (string.IsNullOrEmpty(code))
                return StatusCode(0xCD);

            return Ok(new Tuple<string, string>(code, Registry.Retentions[code]));
        }
        [HttpPost("futures/{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext(string code, [FromBody] IEnumerable<Futures> chart)
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x43AD;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            Registry.Retentions[code] = chart.FirstOrDefault(o => string.IsNullOrEmpty(o.Retention) == false).Retention;
            code = Registry.Codes.Count > 0 ? Registry.Codes.Dequeue() : string.Empty;

            if (string.IsNullOrEmpty(code))
                return StatusCode(0xCD);

            return Ok(new Tuple<string, string>(code, Registry.Retentions[code]));
        }
        [HttpPost("options/{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext(string code, [FromBody] IEnumerable<Options> chart)
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x25F3;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            Registry.Retentions[code] = chart.FirstOrDefault(o => string.IsNullOrEmpty(o.Retention) == false).Retention;
            code = Registry.Codes.Count > 0 ? Registry.Codes.Dequeue() : string.Empty;

            if (string.IsNullOrEmpty(code))
                return StatusCode(0xCD);

            return Ok(new Tuple<string, string>(code, Registry.Retentions[code]));
        }
        [HttpPost("stocks/{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext(string code, [FromBody] IEnumerable<Stocks> chart)
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x1C1B;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            Registry.Retentions[code] = chart.FirstOrDefault(o => string.IsNullOrEmpty(o.Retention) == false).Retention;
            code = Registry.Codes.Count > 0 ? Registry.Codes.Dequeue() : string.Empty;

            if (string.IsNullOrEmpty(code))
                return StatusCode(0xCD);

            return Ok(new Tuple<string, string>(code, Registry.Retentions[code]));
        }
        public ChartsController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
        const string route = "coreapi/[controller]";
        const string produces = "application/json";
        const string futures = "1";
        const string call = "2";
        const string put = "3";
    }
}