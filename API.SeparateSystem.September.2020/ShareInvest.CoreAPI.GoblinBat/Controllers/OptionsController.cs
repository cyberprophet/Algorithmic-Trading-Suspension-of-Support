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
	public class OptionsController : ControllerBase
	{
		[HttpGet, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContexts() => Ok(await context.Options.LongCountAsync());
		[HttpGet(Security.routeRetention), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContext(string key, string code)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				return Ok(new Retention
				{
					Code = code,
					LastDate = await context.Options.Where(o => o.Code.Equals(code)).AsNoTracking().MaxAsync(o => o.Date)
				});
			}
			return BadRequest();
		}
		[HttpPost(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContext(string key, [FromBody] Queue<Options> chart)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				await context.BulkInsertAsync<Queue<Options>>(chart, o =>
				{
					o.InsertIfNotExists = true;
					o.BatchSize = 0x43AD;
					o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
					o.AutoMapOutputDirection = false;
				});
				return Ok();
			}
			return BadRequest();
		}
		public OptionsController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}