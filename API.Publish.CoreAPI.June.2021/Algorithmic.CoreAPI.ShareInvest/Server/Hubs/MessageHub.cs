using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace ShareInvest.Hubs
{
	public class MessageHub : Hub
	{
		public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
		public override async Task OnDisconnectedAsync(Exception exception) => await base.OnDisconnectedAsync(exception);
		public async Task SendMessage(string message) => await Clients.All.SendAsync("ReceiveMessage", message);
	}
}