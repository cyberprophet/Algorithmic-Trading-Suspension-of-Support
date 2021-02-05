using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ShareInvest.Hubs
{
	[Authorize]
	public class MessageHub : Hub
	{
		public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
		public override async Task OnDisconnectedAsync(Exception exception) => await base.OnDisconnectedAsync(exception);
		public async Task SendMessage(string user, string message) => await Clients.User(user).SendAsync("ReceiveMessage", message);
	}
}