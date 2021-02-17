using System;
using System.Collections.Generic;
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
		[HttpGet(Security.collect), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> GetContextAsync(string code)
		{
			try
			{
				if (await context.Ticks.AnyAsync(o => o.Code.Equals(code)))
				{
					var ticks = new Stack<Catalog.Models.Tick>();

					foreach (var tick in from o in context.Ticks.AsNoTracking() where o.Code.Equals(code) select new { o.Date, o.Open, o.Close, o.Price })
						ticks.Push(new Catalog.Models.Tick
						{
							Code = string.Empty,
							Date = tick.Date,
							Open = tick.Open,
							Close = tick.Close,
							Price = tick.Price,
							Contents = string.Empty
						});
					if (ticks.Count > 0)
						return Ok(ticks);
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		[HttpGet(Security.stocks), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContextAsync(string code, string date)
		{
			try
			{
				if (await context.Ticks.FindAsync(code, date) is Tick response)
					return Ok(new Catalog.Models.Tick
					{
						Code = response.Code,
						Date = response.Date,
						Open = response.Open,
						Close = response.Close,
						Price = response.Price,
						Contents = response.Contents.CompressedContents
					});
				return NoContent();
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status401Unauthorized), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.Tick tick)
		{
			try
			{
				var entry = new Tick
				{
					Code = tick.Code,
					Date = tick.Date,
					Open = tick.Open,
					Close = tick.Close,
					Price = tick.Price,
					Contents = new Contents
					{
						Code = tick.Code,
						Date = tick.Date,
						CompressedContents = tick.Contents
					}
				};
				if (await context.Ticks.FindAsync(tick.Code, tick.Date) is Tick response)
				{
					if (string.IsNullOrEmpty(tick.Close) is false && tick.Close.Length == 9 && tick.Close.EndsWith(ends) && tick.Close[1] is '5' or '6' && string.IsNullOrEmpty(tick.Open) is false && tick.Open.Length == 9 && tick.Open.EndsWith(ends) && tick.Open[1] is '9' or '0' && tick.Open.CompareTo(response.Open) <= 0 && tick.Close.CompareTo(response.Close) >= 0)
						context.Entry(entry).State = EntityState.Modified;

					else
						return Unauthorized();
				}
				else
					context.Ticks.Add(entry);

				return Ok(context.SaveChanges());
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		public TickController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
		const string ends = "000";
	}
}