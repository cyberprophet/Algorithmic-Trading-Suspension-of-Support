using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Filter;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class StocksController : ControllerBase
	{
		[HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContext(string key, string code)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				var date = context.Stocks.Where(o => o.Code.Equals(code)).AsNoTracking();

				return Ok(new Retention
				{
					Code = code,
					LastDate = await date.MaxAsync(o => o.Date),
					FirstDate = await date.MinAsync(o => o.Date)
				});
			}
			return BadRequest();
		}
		[HttpGet(Security.stock), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContext(string code, string year, string month, string day)
		{
			try
			{
				var file = Security.GetPath(code, year, month, day);

				if (file.Exists)
					using (var sr = new StreamReader(file.FullName))
						return Ok(Security.Decompress(await sr.ReadToEndAsync()));
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		[HttpPost(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContext(string key, [FromBody] Queue<Stocks> chart)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				await context.BulkInsertAsync<Queue<Stocks>>(chart, o =>
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
		[HttpPost(Security.stocks), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContext(string code, string date, [FromBody] Dictionary<string, string> param)
		{
			try
			{
				var sb = new StringBuilder(code.Length * date.Length);

				foreach (var kv in param.OrderBy(o => o.Key))
					sb.Append(kv.Key).Append(';').Append(kv.Value).Append('|');

				byte[] sourceArray = Encoding.UTF8.GetBytes(sb.Remove(sb.Length - 1, 1).ToString());
				var memoryStream = new MemoryStream();
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
					gZipStream.Write(sourceArray, 0, sourceArray.Length);

				byte[] temporaryArray = new byte[memoryStream.Length], targetArray = new byte[temporaryArray.Length + 4];
				memoryStream.Position = 0;
				memoryStream.Read(temporaryArray, 0, temporaryArray.Length);
				Buffer.BlockCopy(temporaryArray, 0, targetArray, 4, temporaryArray.Length);
				Buffer.BlockCopy(BitConverter.GetBytes(sourceArray.Length), 0, targetArray, 0, 4);

				using (var sw = new StreamWriter(Security.GetPath(code, date), false))
					await sw.WriteAsync(Convert.ToBase64String(targetArray));

				return Ok();
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		[HttpPost, ServiceFilter(typeof(ClientIpCheckActionFilter)), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Stocks stocks)
		{
			try
			{
				if (string.IsNullOrEmpty(stocks.Price) is false && string.IsNullOrEmpty(stocks.Retention) is false)
					switch (stocks.Code.Length)
					{
						case 6:
							var error = new Stack<Stocks>();

							foreach (var query in from o in context.Stocks where o.Code.Equals(stocks.Code) && o.Date.StartsWith(stocks.Date) && (o.Price.CompareTo(stocks.Price) > 0 || o.Price.CompareTo(stocks.Retention) < 0) select o)
								if (int.TryParse(query.Price, out int price) && int.TryParse(stocks.Price, out int high) && int.TryParse(stocks.Retention, out int low) && (price > high || price < low))
									error.Push(query);

							if (error.Count > 0)
							{
								context.Stocks.RemoveRange(error);

								return Ok(context.SaveChanges().ToString("N0"));
							}
							break;

						case 8 when stocks.Code[0] is '1':
							var err = new Stack<Futures>();

							foreach (var query in from o in context.Futures where o.Code.Equals(stocks.Code) && o.Date.StartsWith(stocks.Date) && (o.Price.CompareTo(stocks.Price) > 0 || o.Price.CompareTo(stocks.Retention) < 0) select o)
								if (double.TryParse(query.Price, out double price) && double.TryParse(stocks.Price, out double high) && double.TryParse(stocks.Retention, out double low) && (price > high || price < low))
									err.Push(query);

							if (err.Count > 0)
							{
								context.Futures.RemoveRange(err);

								return Ok(context.SaveChanges().ToString("N0"));
							}
							break;

						case 8 when stocks.Code[0] is '2' or '3':
							var modify = new Stack<Options>();

							foreach (var query in from o in context.Options where o.Code.Equals(stocks.Code) && o.Date.StartsWith(stocks.Date) && (o.Price.CompareTo(stocks.Price) > 0 || o.Price.CompareTo(stocks.Retention) < 0) select o)
								if (double.TryParse(query.Price, out double price) && double.TryParse(stocks.Price, out double high) && double.TryParse(stocks.Retention, out double low) && (price > high || price < low))
									modify.Push(query);

							if (modify.Count > 0)
							{
								context.Options.RemoveRange(modify);

								return Ok(context.SaveChanges().ToString("N0"));
							}
							break;
					}
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
				Base.SendMessage(GetType(), stocks.Code, stocks.Date);
			}
			return Ok();
		}
		public StocksController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}