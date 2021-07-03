using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using Newtonsoft.Json.Linq;

using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ShareInvest.Hubs
{
	[Authorize]
	public class AccountHub : Hub
	{
		public async Task SendMessage(string user, string account)
		{
			if (Security.User.TryGetValue(user, out Catalog.Models.User model) && model.Socket is WebSocket && WebSocketState.Open.Equals(model.Socket.State))
			{
				model.Send += OnReceiveMessage;
				await model.Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(account)), WebSocketMessageType.Text, true, model.Token);
			}
		}
		public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
		public override async Task OnDisconnectedAsync(Exception exception) => await base.OnDisconnectedAsync(exception);
		public AccountHub(IHubContext<AccountHub> hub) => this.hub = hub;
		async void OnReceiveMessage(object sender, string message)
		{
			if (sender is Catalog.Models.User user && user.Id.Length > 0)
			{
				if (user.Id.Length == 1)
					await hub.Clients.User(user.Id[0]).SendAsync("ReceiveAccountMessage", message);

				else
					await hub.Clients.Users(user.Id).SendAsync("ReceiveAccountMessage", message);

				if (user.Count == 0)
					user.Count = (int)JToken.Parse(message)["출력건수"];

				else
					user.Count--;

				if (user.Count == 0)
					user.Send -= OnReceiveMessage;
			}
		}
		readonly IHubContext<AccountHub> hub;
	}
}