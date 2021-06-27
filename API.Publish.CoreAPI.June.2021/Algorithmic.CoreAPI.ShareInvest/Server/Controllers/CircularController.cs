using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class CircularController : ControllerBase
	{
		[HttpGet]
		public async Task<string> GetContextAsync(string id, string account)
		{
			var response = string.Empty;
			var repeat = account.Length;

			if (Security.User.TryGetValue(id, out Catalog.Models.User user))
			{
				await user.Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(account)), WebSocketMessageType.Text, true, token);

				while (string.IsNullOrEmpty(user.Message) && repeat > 0)
				{
					await Task.Delay(0x100);
					repeat--;

					if (string.IsNullOrEmpty(user.Message) is false)
						response = user.Message;
				}
				user.Message = string.Empty;
			}
			return response;
		}
		public CircularController() => token = new CancellationToken();
		readonly CancellationToken token;
	}
}