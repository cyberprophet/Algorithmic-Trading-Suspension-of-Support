using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;

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
						analysis.Balance = new Balance
						{
							Market = balance.Code.Length == 8,
							Name = balance.Name,
							Purchase = 0,
							Quantity = 0,
							Revenue = 0,
							Rate = 0D
						};
					if (hub is not null)
						await hub.Clients.All.SendAsync(message, analysis.OnReceiveBalance(balance));
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace, balance);
				Base.SendMessage(JsonConvert.SerializeObject(balance), balance.GetType());
			}
			return Ok();
		}
		public BalanceController(IHubContext<BalanceHub> hub) => this.hub = hub;
		readonly IHubContext<BalanceHub> hub;
		const string message = "ReceiveBalanceMessage";
	}
}