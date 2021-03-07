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
	public class IncorporatedStocksController : ControllerBase
	{
		[HttpPost(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContext(string key, [FromBody] IEnumerable<IncorporatedStocks> param)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				foreach (var stock in param)
					if (string.IsNullOrEmpty(stock.Code) == false)
					{
						if (await context.Incorporate.AnyAsync(o => o.Code.Equals(stock.Code)))
							context.Entry(stock).State = EntityState.Modified;

						else
							context.Incorporate.Add(stock);

						await context.BulkSaveChangesAsync();
					}
				return Ok();
			}
			return BadRequest();
		}
		[HttpGet(Security.routeMarket), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContext(string key)
		{
			try
			{
				var date = await context.Incorporate.MaxAsync(o => o.Date);

				return Ok(await context.Incorporate.Where(o => o.Market.Equals(key) && o.Date.Equals(date)).ToListAsync());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContext)}");
			}
			return BadRequest();
		}
		[HttpGet(Security.market), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContext(int key)
		{
			try
			{
				var date = DateTime.Now.ToString(Base.DateFormat);
				int count;

				switch (key)
				{
					case 'P':
						var market = ((char)key).ToString();
						count = (await context.Incorporate.Where(o => o.Market.Equals(market) && o.Date.Equals(date)).ToListAsync()).Count;

						if (count >= 0xC8)
							return BadRequest();

						else
							return Ok((count > 0xBE && count < 0xC8 ? 2 : 1) + count / 0xA);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContext)}");
			}
			return Ok(1);
		}
		public IncorporatedStocksController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}