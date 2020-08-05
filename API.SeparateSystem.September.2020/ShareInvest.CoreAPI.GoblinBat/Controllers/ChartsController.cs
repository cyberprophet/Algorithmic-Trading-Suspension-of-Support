using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ChartsController : ControllerBase
    {
        [Conditional("DEBUG")]
        void SendExceptionMessage(string message) => Console.WriteLine(message);
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
        [HttpGet(Security.routeGetDayChart), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContext(string code)
        {
            try
            {
                return Ok(await context.Days.Where(o => o.Code.Equals(code)).AsNoTracking().Select(o => new { o.Date, o.Price }).ToListAsync());
            }
            catch (Exception ex)
            {
                SendExceptionMessage(ex.Message);
            }
            return NotFound(code);
        }
        [HttpGet(Security.routeGetChart), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContext(string code, string start, string end)
        {
            try
            {
                var charts = new Stack<Charts>();

                switch (code.Length)
                {
                    case 6:
                        var stocks = context.Stocks.Where(o => o.Code.Equals(code)).AsNoTracking();

                        if (start.Length < 6 || end.Length < 6)
                            return Ok(await stocks.MinAsync(o => o.Date));

                        else
                            foreach (var chart in stocks.Where(o => o.Date.CompareTo(string.Concat(start, remain)) > 0 && o.Date.CompareTo(string.Concat(end, remain)) < 0).AsNoTracking().Select(o => new { o.Date, o.Price, o.Volume }).OrderBy(o => o.Date))
                                charts.Push(new Charts
                                {
                                    Date = chart.Date,
                                    Price = chart.Price,
                                    Volume = chart.Volume
                                });
                        break;

                    case int length when length == 8 && code.EndsWith("000") && (code.StartsWith("101") || code.StartsWith("106")):
                        var futures = context.Futures.Where(o => o.Code.Equals(code)).AsNoTracking();

                        if (start.Length < 6 || end.Length < 6)
                            return Ok(await futures.MinAsync(o => o.Date));

                        else
                            foreach (var chart in futures.Where(o => o.Date.CompareTo(string.Concat(start, remain)) > 0 && o.Date.CompareTo(string.Concat(end, remain)) < 0).AsNoTracking().Select(o => new { o.Date, o.Price, o.Volume }).OrderBy(o => o.Date))
                                charts.Push(new Charts
                                {
                                    Date = chart.Date,
                                    Price = chart.Price,
                                    Volume = chart.Volume
                                });
                        break;

                    case int length when length == 8 && (code.StartsWith("2") || code.StartsWith("3")):
                        var options = context.Options.Where(o => o.Code.Equals(code)).AsNoTracking();

                        if (start.Length < 6 || end.Length < 6)
                            return Ok(await options.MinAsync(o => o.Date));

                        else
                            foreach (var chart in options.Where(o => o.Date.CompareTo(string.Concat(start, remain)) > 0 && o.Date.CompareTo(string.Concat(end, remain)) < 0).AsNoTracking().Select(o => new { o.Date, o.Price, o.Volume }).OrderBy(o => o.Date))
                                charts.Push(new Charts
                                {
                                    Date = chart.Date,
                                    Price = chart.Price,
                                    Volume = chart.Volume
                                });
                        break;

                    case int length when length == 8 && code.EndsWith("000") && code.Substring(1, 1).Equals("0") == false:
                        var name = (await context.Codes.FirstAsync(o => o.Code.Equals(code))).Name;
                        var convert = (await context.Codes.FirstAsync(o => o.Name.StartsWith(name) && o.Code.Length == 6)).Code;
                        var sFutures = context.Stocks.Where(o => o.Code.Equals(convert)).AsNoTracking();

                        if (start.Length < 6 || end.Length < 6)
                            return Ok(await sFutures.MinAsync(o => o.Date));

                        else
                            foreach (var chart in sFutures.Where(o => o.Date.CompareTo(string.Concat(start, remain)) > 0 && o.Date.CompareTo(string.Concat(end, remain)) < 0).AsNoTracking().Select(o => new { o.Date, o.Price, o.Volume }).OrderBy(o => o.Date))
                                charts.Push(new Charts
                                {
                                    Date = chart.Date,
                                    Price = chart.Price,
                                    Volume = chart.Volume
                                });
                        break;
                }
                if (charts.Count > 0)
                    return Ok(charts);
            }
            catch (Exception ex)
            {
                SendExceptionMessage(ex.Message);
            }
            return NotFound(code);
        }
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetContexts() => NotFound();
        [HttpPost("days/{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext<T>(string code, [FromBody] IEnumerable<Days> chart) where T : struct
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x6BD;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            if (string.IsNullOrEmpty(code))
                return StatusCode(0xCD);

            return Ok(code);
        }
        [HttpPost("futures/{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext<T>(string code, [FromBody] IEnumerable<Futures> chart) where T : struct
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x43AD;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            if (string.IsNullOrEmpty(code))
                return StatusCode(0xCD);

            return Ok(code);
        }
        [HttpPost("options/{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext<T>(string code, [FromBody] IEnumerable<Options> chart) where T : struct
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x25F3;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            if (string.IsNullOrEmpty(code))
                return StatusCode(0xCD);

            return Ok(code);
        }
        [HttpPost("stocks/{code}"), ProducesResponseType(StatusCodes.Status205ResetContent), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext<T>(string code, [FromBody] IEnumerable<Stocks> chart) where T : struct
        {
            await context.BulkInsertAsync(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x1C1B;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            if (string.IsNullOrEmpty(code))
                return StatusCode(0xCD);

            return Ok(code);
        }
        public ChartsController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
        const string futures = "1";
        const string call = "2";
        const string put = "3";
        const string remain = "000000000";
    }
}