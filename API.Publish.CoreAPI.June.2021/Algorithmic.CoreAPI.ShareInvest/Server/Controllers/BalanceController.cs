using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Hubs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class BalanceController : ControllerBase
	{
		[AllowAnonymous, HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.Balance balance)
		{
			try
			{
				if (Security.User.TryGetValue(balance.Kiwoom, out Catalog.Models.User user))
				{
					user.Balance[balance.Code] = balance;

					if (hub is not null && user.Id.Length > 0)
					{
						if (user.Id.Length == 1)
							await hub.Clients.User(user.Id[0]).SendAsync(message, balance);

						else
							await hub.Clients.Users(user.Id).SendAsync(message, balance);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.PostContextAsync)}");
			}
			return Ok(balance.Name);
		}
		[HttpGet]
		public IEnumerable<Catalog.Models.Balance> GetContextAsync(string key)
		{
			if (context.User.AsNoTracking().Any(o => o.Email.Equals(key)))
			{
				var stack = new Stack<Catalog.Models.Balance>();

				foreach (var user in from o in context.User.AsNoTracking() where o.Email.Equals(key) select o)
					if (Security.User.TryGetValue(user.Kiwoom, out Catalog.Models.User model))
						foreach (var kv in model.Balance)
							stack.Push(kv.Value);

				if (stack.Count > 0)
					return stack.ToArray();
			}
			return Array.Empty<Catalog.Models.Balance>();
		}
		public BalanceController(CoreApiDbContext context, IHubContext<BalanceHub> hub)
		{
			this.hub = hub;
			this.context = context;
		}
		readonly CoreApiDbContext context;
		readonly IHubContext<BalanceHub> hub;
		const string message = "ReceiveBalanceMessage";
	}
}