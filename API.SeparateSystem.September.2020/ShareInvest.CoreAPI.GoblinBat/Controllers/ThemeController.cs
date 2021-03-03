using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class ThemeController : ControllerBase
	{
		[HttpGet, ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContextAsync()
		{
			try
			{
				return Ok(await (from o in context.Theme.AsNoTracking() select new { o.Name, o.Index, o.Rate, o.Average }).ToArrayAsync());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpPost, ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Models.Theme theme)
		{
			try
			{
				if (await context.Theme.AnyAsync(o => o.Index.Equals(theme.Index)))
				{
					if (context.Theme.Any(o => o.Date.Equals(theme.Date) && o.Index.Equals(theme.Index)))
						return Ok();

					context.Entry(theme).State = EntityState.Modified;
				}
				else
					context.Theme.Add(theme);

				return Ok(context.SaveChanges());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.PostContextAsync)}");
			}
			return BadRequest();
		}
		public ThemeController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}