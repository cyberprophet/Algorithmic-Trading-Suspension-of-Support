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
		[HttpGet(Security.collect), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContextAsync(string code)
		{
			try
			{
				if (await context.Group.AsNoTracking().AnyAsync(o => o.Code.Equals(code)))
					foreach (var page in from o in context.Page.AsNoTracking() where o.Code.Equals(code) && o.Tistory > 0 select o.Tistory)
						return Ok(page);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{code}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContextAsync(string key, string code)
		{
			try
			{
				return Ok(await context.Url.AsNoTracking().AnyAsync(o => o.Index.Equals(key) && o.Record.Equals(code)));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{key}\n{ex.Message}\n{code}\n{nameof(this.GetContextAsync)}");
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
		[HttpPut, ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> PutContextAsync([FromBody] Models.Url url)
		{
			try
			{
				if (context.Url.Find(url.Index) is Models.Url model)
				{
					model.Record = url.Record;
					model.Json = url.Json;
				}
				else if (context.Theme.Find(url.Index) is Models.Theme theme)
					theme.Url = url;

				else
					return NoContent();

				return Ok(await context.SaveChangesAsync());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{url.Index}\n{ex.Message}\n{url.Record}\n{nameof(this.PutContextAsync)}");
			}
			return BadRequest();
		}
		public ThemeController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}