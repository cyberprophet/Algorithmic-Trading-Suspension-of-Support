using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;
using ShareInvest.Client;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class FuturesController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Queue<Futures> param)
		{
			if (await Progress.Client.PostContextAsync(param) > 0xC8)
			{
				var peek = param.Peek();
				Base.SendMessage(peek.Code, peek.Retention, GetType());
			}
			var now = DateTime.Now;

			if (await Progress.Client.PostContextAsync(await new ConstituentStocks(Progress.Key.Security).GetConstituentStocks(now.Second % 2 == 0 ? 2 : 1, now)) > 0xC8)
				Base.SendMessage(now.ToLongTimeString(), typeof(ConstituentStocks));

			return Ok();
		}
	}
}