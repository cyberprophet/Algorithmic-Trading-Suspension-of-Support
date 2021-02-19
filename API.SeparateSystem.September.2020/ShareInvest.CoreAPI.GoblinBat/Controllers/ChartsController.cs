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
	public class ChartsController : ControllerBase
	{
		async Task<string> ConvertName(string code)
		{
			var name = (await context.Codes.FirstAsync(o => o.Code.Equals(code))).Name;
			var convert = (await context.Codes.FirstOrDefaultAsync(o => o.Name.Equals(name) && o.Code.Length == 6))?.Code;

			if (string.IsNullOrEmpty(convert))
				convert = (await context.Codes.FirstAsync(o => o.Name.StartsWith(name) && o.Code.Length == 6)).Code;

			return convert;
		}
		[HttpDelete(Security.routeGetDayChart), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> DeleteContext(string key, string code, [FromBody] Security param)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))) && Security.IsAdministrator(param.Administrator))
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
		[HttpGet(Security.routeGetDayChart), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetContext(string key, string code)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				try
				{
					string convert;

					if (code.Length == 8 && code[1].Equals('0') == false)
						convert = await ConvertName(code);

					else
						convert = code;

					return Ok(await context.Days.Where(o => o.Code.Equals(convert)).AsNoTracking().Select(o => new { o.Date, o.Price }).ToListAsync());
				}
				catch (Exception ex)
				{
					Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContext)}");
				}
				return NotFound(code);
			}
			return BadRequest();
		}
		[HttpGet(Security.route_day_chart), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContextAsync(string code, string start, string end)
		{
			try
			{
				if (await context.Stocks.AnyAsync(o => o.Code.Equals(code)))
				{
					var response = new Stack<Charts>();
					var stocks = from o in context.Stocks.AsNoTracking() where o.Code.Equals(code) select new { o.Date, o.Price, o.Volume };

					foreach (var co in from o in stocks.AsNoTracking() where o.Date.EndsWith(end_date) && start_date.Equals(o.Date.Substring(start.Length, end.Length / 2)) && o.Date.CompareTo(string.Concat(start, remain)) > 0 && o.Date.CompareTo(string.Concat(end, remain)) < 0 select o)
						if (co.Date[9] < '5' && co.Date.StartsWith(sat) is false)
							response.Push(new Charts
							{
								Date = co.Date.Substring(0, end.Length + start.Length),
								Price = co.Price,
								Volume = co.Volume
							});
					if (sat.CompareTo(start) >= 0)
						foreach (var co in from o in stocks.AsNoTracking() where o.Date.StartsWith(sat_date) && o.Date.EndsWith(end_date) select o)
							if (co.Date[9] < '5')
								response.Push(new Charts
								{
									Date = co.Date.Substring(0, end.Length + start.Length),
									Price = co.Price,
									Volume = co.Volume
								});
					if (response.Count > 0)
						return Ok(response);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpGet(Security.routeGetChart), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetContext(string key, string code, string start, string end)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
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

						case int length when length == 8 && code.EndsWith("000") && (code.StartsWith("101") || code.StartsWith("105") || code.StartsWith("106")):
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

						case int length when length == 8 && code.EndsWith("000") && code[1].Equals('0') == false:
							var convert = await ConvertName(code);
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
					Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContext)}");
				}
				return NotFound(code);
			}
			return BadRequest();
		}
		[HttpGet, ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetContexts() => NotFound();
		[HttpPost(Security.routeDays), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContext<T>(string key, string code, [FromBody] IEnumerable<Days> chart) where T : struct
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				await context.BulkInsertAsync(chart, o =>
				{
					o.InsertIfNotExists = true;
					o.BatchSize = 0x6BD;
					o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
					o.AutoMapOutputDirection = false;
				});
				if (string.IsNullOrEmpty(code))
					return NotFound();

				return Ok(code);
			}
			return BadRequest();
		}
		[HttpPost(Security.routeFutures), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContext<T>(string key, string code, [FromBody] IEnumerable<Futures> chart) where T : struct
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				await context.BulkInsertAsync(chart, o =>
				{
					o.InsertIfNotExists = true;
					o.BatchSize = 0x43AD;
					o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
					o.AutoMapOutputDirection = false;
				});
				if (string.IsNullOrEmpty(code))
					return NotFound();

				return Ok(code);
			}
			return BadRequest();
		}
		[HttpPost(Security.routeOptions), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContext<T>(string key, string code, [FromBody] IEnumerable<Options> chart) where T : struct
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				await context.BulkInsertAsync(chart, o =>
				{
					o.InsertIfNotExists = true;
					o.BatchSize = 0x25F3;
					o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
					o.AutoMapOutputDirection = false;
				});
				if (string.IsNullOrEmpty(code))
					return NotFound();

				return Ok(code);
			}
			return BadRequest();
		}
		[HttpPost(Security.routeStock), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContext<T>(string key, string code, [FromBody] IEnumerable<Stocks> chart) where T : struct
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
				if (string.IsNullOrEmpty(code))
					return NotFound();

				return Ok(code);
			}
			return BadRequest();
		}
		public ChartsController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
		const string futures = "1";
		const string call = "2";
		const string put = "3";
		const string start_date = "153";
		const string sat_date = "201203163";
		const string end_date = "000";
		const string sat = "201203";
		const string remain = "000000000";
	}
}