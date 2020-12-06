using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

using ShareInvest.Hubs;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class BalanceController : ControllerBase
	{
		[HttpPost]
		public async Task PostContextAsync([FromBody] Catalog.Models.Balance balance)
		{
			if (hub is not null)
				await hub.Clients.All.SendAsync(message, balance);

			else
				Base.SendMessage(message, balance.Name, GetType());
		}
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PutContextAsync([FromBody] Catalog.OpenAPI.Balance balance)
		{
			try
			{
				if (Progress.Collection.TryGetValue(balance.Code[0] is 'A' ? balance.Code[1..] : balance.Code, out Analysis analysis))
				{
					if (analysis.Balance is null)
						analysis.Balance = new Balance(new string[]
						{
							balance.Code,
							balance.Name,
							balance.Quantity,
							balance.Purchase,
							balance.Current,
							string.Empty,
							balance.Rate
						});
					if (hub is not null)
						await hub.Clients.All.SendAsync(message, await analysis.OnReceiveBalance(balance));
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return Ok();
		}
		public BalanceController(IHubContext<BalanceHub> hub) => this.hub = hub;
		readonly IHubContext<BalanceHub> hub;
		const string message = "ReceiveBalanceMessage";
	}
}