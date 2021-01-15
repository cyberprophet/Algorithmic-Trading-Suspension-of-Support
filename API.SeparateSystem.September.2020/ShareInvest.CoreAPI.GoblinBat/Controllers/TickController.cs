using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class TickController : ControllerBase
	{
		[HttpGet(Security.stocks), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContextAsync(string code, string date)
		{
			try
			{
				if (await context.Ticks.AnyAsync(o => o.Code.Equals(code) && o.Date.Equals(date)))
					return Ok(context.Ticks.First(o => o.Code.Equals(code) && o.Date.Equals(date)));
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		[HttpGet(Security.collect), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContextAsync(string code)
		{
			try
			{
				if (await context.Ticks.AnyAsync(o => o.Code.Equals(code)))
					return Ok(context.Ticks.Where(o => o.Code.Equals(code)).Select(o => new { o.Date, o.Price }).ToList());
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContextAsync([FromBody] Tick tick)
		{
			try
			{
				if (await context.Ticks.AnyAsync(o => o.Code.Equals(tick.Code) && o.Date.Equals(tick.Date)))
				{
					var first = context.Ticks.First(o => o.Code.Equals(tick.Code) && o.Date.Equals(tick.Date));

					if (string.IsNullOrEmpty(tick.Open) is false && (first.Close.CompareTo(tick.Close) < 0 || first.Open.CompareTo(tick.Open) > 0))
						context.Entry(tick).State = EntityState.Modified;

					else
						return Ok();
				}
				else
					context.Ticks.Add(tick);

				await context.BulkSaveChangesAsync();

				return Ok();
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		public TickController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}