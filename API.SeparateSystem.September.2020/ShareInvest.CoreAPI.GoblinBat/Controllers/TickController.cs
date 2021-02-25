using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using ShareInvest.Filter;
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
				if (await context.Ticks.AsNoTracking().AnyAsync(o => o.Code.Equals(code)))
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
							Contents = string.Empty,
							Path = string.Empty
						});
					if (ticks.Count > 0)
						return Ok(ticks);
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpGet(Security.stocks), ServiceFilter(typeof(ClientIpCheckActionFilter)), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), SupportedOSPlatform("windows")]
		public async Task<IActionResult> GetContextAsync(string code, string date)
		{
			try
			{
				if (await context.Ticks.FindAsync(code, date) is Tick response)
					foreach (var content in from o in context.Contents.AsNoTracking() where o.Code.Equals(response.Code) && o.Date.Equals(response.Date) select o.CompressedContents)
						return Ok(new Catalog.Models.Tick
						{
							Code = response.Code,
							Date = response.Date,
							Open = response.Open,
							Close = response.Close,
							Price = response.Price,
							Path = string.Empty,
							Contents = Repository.RetrieveSavedMaterial(content) as string
						});
				return NoContent();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\nCode:{code} Date:{date}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpPost, ServiceFilter(typeof(ClientIpCheckActionFilter)), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status204NoContent), SupportedOSPlatform("windows")]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.Tick tick)
		{
			try
			{
				var price = string.Empty;
				Repository.KeepOrganizedInStorage(tick);

				if (Base.PriceEmpty.Equals(tick.Price))
					switch (tick.Code.Length)
					{
						case 6:
							price = context.Stocks.AsNoTracking().First(r => r.Code.Equals(tick.Code) && r.Date.Equals((from o in context.Stocks.AsNoTracking() where o.Code.Equals(tick.Code) && o.Date.Substring(0, 8).Equals(string.Concat(tick.Date.Substring(2, 6), tick.Close.Substring(0, 2))) select o.Date).Max())).Price;
							break;

						case 8 when tick.Code[0] is '1' && tick.Code[1] is '0':
							price = context.Futures.AsNoTracking().First(r => r.Code.Equals(tick.Code) && r.Date.Equals((from o in context.Futures.AsNoTracking() where o.Code.Equals(tick.Code) && o.Date.Substring(0, 8).Equals(string.Concat(tick.Date.Substring(2, 6), tick.Close.Substring(0, 2))) select o.Date).Max())).Price;
							break;

						case 8 when tick.Code[0] is '2' or '3':
							price = context.Options.AsNoTracking().First(r => r.Code.Equals(tick.Code) && r.Date.Equals((from o in context.Options.AsNoTracking() where o.Code.Equals(tick.Code) && o.Date.Substring(0, 8).Equals(string.Concat(tick.Date.Substring(2, 6), tick.Close.Substring(0, 2))) select o.Date).Max())).Price;
							break;
					}
				if (await context.Ticks.AsNoTracking().AnyAsync(o => o.Code.Equals(tick.Code) && o.Date.Equals(tick.Date)))
					foreach (var i in from o in context.Ticks.AsNoTracking() where o.Code.Equals(tick.Code) && o.Date.Equals(tick.Date) select new { o.Close, o.Open })
					{
						if ((i.Open.Equals(tick.Open) && i.Close.Equals(tick.Close)) is false && (int.TryParse(i.Close, out int ic) && int.TryParse(tick.Close, out int tc) && ic < tc || int.TryParse(i.Open, out int io) && int.TryParse(tick.Open, out int to) && io > to))
						{
							var modify = context.Ticks.First(o => o.Code.Equals(tick.Code) && o.Date.Equals(tick.Date));
							modify.Open = tick.Open;
							modify.Close = tick.Close;
							modify.Price = string.IsNullOrEmpty(price) ? tick.Price : price;
							context.Contents.First(o => o.Code.Equals(tick.Code) && o.Date.Equals(tick.Date)).CompressedContents = tick.Path;
						}
						else
							return NoContent();
					}
				else
					context.Ticks.Add(new Tick
					{
						Code = tick.Code,
						Date = tick.Date,
						Open = tick.Open,
						Close = tick.Close,
						Price = string.IsNullOrEmpty(price) ? tick.Price : price,
						Contents = new Contents
						{
							Code = tick.Code,
							Date = tick.Date,
							CompressedContents = tick.Path
						}
					});
				return Ok(context.SaveChanges());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{JsonConvert.SerializeObject(new Catalog.Models.Tick { Code = tick.Code, Date = tick.Date, Open = tick.Open, Close = tick.Close, Price = tick.Price, Contents = tick.Contents.Length.ToString("N0"), Path = tick.Path })}\n{ex.InnerException.Message}\n{nameof(this.PostContextAsync)}");
			}
			return BadRequest();
		}
		public TickController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}