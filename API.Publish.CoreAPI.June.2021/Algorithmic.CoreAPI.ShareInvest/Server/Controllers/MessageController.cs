using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using ShareInvest.Catalog.Models;
using ShareInvest.Hubs;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class MessageController : ControllerBase
	{
		[AllowAnonymous, HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Message param)
		{
			try
			{
				if (Security.User.TryGetValue(param.Key, out User user))
				{
					var log = param.Convey.Split('(');
					var message = log[0].Split(']');
					user.Logs.Enqueue(new Log
					{
						Time = DateTime.Now,
						Message = message[^1].Trim(),
						Code = message[0].Replace("[", string.Empty),
						Screen = log[^1].Remove(log[^1].Length - 1)
					});
					if (hub is not null)
						await hub.Clients.All.SendAsync(method, log[0].Trim());

					return Ok(param.Convey);
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return Ok();
		}
		[HttpGet]
		public IEnumerable<Log> GetContext(string key)
		{
			if (string.IsNullOrEmpty(key) is false)
				foreach (var str in new[] { key, HttpUtility.UrlDecode(key) })
					if (Security.User.TryGetValue(str, out User user) && user.Logs.Count > 0)
						return user.Logs.ToArray();

			return Array.Empty<Log>();
		}
		public MessageController(IHubContext<MessageHub> hub) => this.hub = hub;
		readonly IHubContext<MessageHub> hub;
		const string method = "ReceiveMessage";
	}
}