using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class IdentifyController : ControllerBase
	{
		[HttpGet, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContextAsync()
		{
			try
			{
				return Ok(await context.Securities.AsNoTracking().Select(o => o.Code).ToArrayAsync());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpGet(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContextAsync(string key)
		{
			try
			{
				if (await context.Securities.AsNoTracking().AnyAsync(o => o.Security.Equals(key)))
					return Ok(context.Securities.AsNoTracking().Where(o => o.Security.Equals(key)).Select(o => new
					{
						o.Strategics,
						o.Contents,
						o.Date,
						o.Code
					}));
				else
					return NoContent();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpGet(Security.routeGetDayChart), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContextAsync(string key, string code)
		{
			try
			{
				if (await context.Securities.AsNoTracking().AnyAsync(o => o.Security.Equals(key) && o.Code.Equals(code)))
					return Ok(context.Securities.AsNoTracking().First(o => o.Code.Equals(code) && o.Security.Equals(key)));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContextAsync([FromBody] Models.Identify security)
		{
			try
			{
				if (context.Securities.Any(o => o.Code.Equals(security.Code) && o.Security.Equals(security.Security)))
					context.Entry(security).State = EntityState.Modified;

				else
					context.Securities.Add(security);

				await context.BulkSaveChangesAsync();

				return Ok();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.PostContextAsync)}");
			}
			return BadRequest();
		}
		public IdentifyController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}