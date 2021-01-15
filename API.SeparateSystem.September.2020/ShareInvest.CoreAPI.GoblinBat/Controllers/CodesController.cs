using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class CodesController : ControllerBase
	{
		[HttpGet(Security.routeCodes), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContext(string key, string code)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				var context = await this.context.Codes.FindAsync(code);

				if (context != null)
					return Ok(context);

				return NotFound(code);
			}
			return BadRequest();
		}
		[HttpGet(Security.routeLength), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContexts(string key, int length)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				if (length == 6 || length == 8)
					return Ok(await context.Codes.Where(o => o.Code.Length == length).Select(o => new { o.Code, o.Name, o.MarginRate, o.MaturityMarketCap, o.Price }).AsNoTracking().ToListAsync());

				else
					return NotFound(length);
			}
			return BadRequest();
		}
		[HttpGet(Security.routeKey), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContexts(string key)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				var date = string.Empty;
				var dic = new Dictionary<string, string>();

				foreach (var arg in context.Codes.AsNoTracking().Select(o => new { o.Code }))
				{
					switch (arg.Code.Length)
					{
						case 6:
							date = await context.Stocks.Where(o => o.Code.Equals(arg.Code)).AsNoTracking().MaxAsync(o => o.Date);
							break;

						case int length when length == 8 && (arg.Code.StartsWith("106") || arg.Code.StartsWith("101")) && arg.Code.EndsWith("000"):
							date = await context.Futures.Where(o => o.Code.Equals(arg.Code)).AsNoTracking().MaxAsync(o => o.Date);
							break;

						case int length when length == 8 && (arg.Code.StartsWith("2") || arg.Code.StartsWith("3")):
							date = await context.Options.Where(o => o.Code.Equals(arg.Code)).AsNoTracking().MaxAsync(o => o.Date);
							break;
					}
					if (string.IsNullOrEmpty(date) == false && string.Compare(date.Substring(0, 6), DateTime.Now.ToString("yyMMdd")) < 0)
						dic[arg.Code] = date;
				}
				return Ok(dic);
			}
			return BadRequest();
		}
		[HttpPost(Security.routeKey), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContext(string key, [FromBody] IEnumerable<Codes> chart)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
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
			return BadRequest();
		}
		[HttpPut(Security.routeGetDayChart), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PutContext(string key, string code, [FromBody] Codes param)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				if (string.IsNullOrEmpty(code) == false && string.IsNullOrEmpty(param.MaturityMarketCap) == false)
				{
					if (await context.Codes.AnyAsync(o => o.Code.Equals(code)))
						context.Entry(param).State = EntityState.Modified;

					else
						context.Codes.Add(param);

					string retentions = string.Empty;
					await context.BulkSaveChangesAsync();
				}
				return Ok(code);
			}
			return BadRequest();
		}
		public CodesController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}