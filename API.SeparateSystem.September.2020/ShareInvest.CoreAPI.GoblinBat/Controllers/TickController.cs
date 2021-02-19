using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

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
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpGet(Security.confirm), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> GetContextAsync(string code, string date, string start, string close)
		{
			try
			{
				if (await context.Ticks.AnyAsync(o => o.Code.Equals(code) && o.Date.Equals(date) && o.Open.Equals(start) && o.Close.Equals(close)))
					return Ok();

				else
					return NoContent();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
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
						Contents = context.Contents.Find(response.Code, response.Date).CompressedContents
					});
				return NoContent();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.Tick tick)
		{
			try
			{
				if (string.IsNullOrEmpty(tick.Close) is false && tick.Close.Length == 9 && tick.Close.EndsWith(ends) && tick.Close[1] is '5' or '6' && tick.Close[2] is '3' && string.IsNullOrEmpty(tick.Open) is false && tick.Open.Length == 9 && tick.Open.EndsWith(ends) && tick.Open[1] is '9' or '0')
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
					if (await context.Ticks.AnyAsync(o => o.Code.Equals(tick.Code) && o.Date.Equals(tick.Date)))
						foreach (var res in from o in context.Ticks where o.Code.Equals(tick.Code) && o.Date.Equals(tick.Date) select o)
							if ((tick.Open.Equals(res.Open) && tick.Close.Equals(res.Close)) is false && uint.TryParse(tick.Open, out uint to) && uint.TryParse(res.Open, out uint ro) && to < ro && res.Close[2] is not '3' && res.Close[1] is not '5' or '6' && res.Open[1] is not '9' or '0')
								context.SingleDelete(res, o =>
								{
									o.BatchSize = 0x5000;
									o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
									o.AutoMapOutputDirection = false;
								});
					await context.SingleInsertAsync(entry, o =>
					{
						o.InsertIfNotExists = true;
						o.BatchSize = 0x5000;
						o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
						o.AutoMapOutputDirection = false;
					});
					return Ok();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{JsonConvert.SerializeObject(new Catalog.Models.Tick { Code = tick.Code, Date = tick.Date, Open = tick.Open, Close = tick.Close, Price = tick.Price, Contents = tick.Contents.Length.ToString("N0") })}\n{ex.InnerException.Message}\n{nameof(this.PostContextAsync)}");
			}
			return BadRequest();
		}
		public TickController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
		const string ends = "000";
	}
}