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
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.Balance balance)
		{
			if (hub is not null)
				try
				{
					await hub.Clients.All.SendAsync(message, balance);
				}
				catch (Exception ex)
				{
					Base.SendMessage(GetType(), ex.StackTrace);
				}
			return Ok(balance.Name);
		}
		public BalanceController(IHubContext<BalanceHub> hub) => this.hub = hub;
		readonly IHubContext<BalanceHub> hub;
		const string message = "ReceiveBalanceMessage";
	}
}