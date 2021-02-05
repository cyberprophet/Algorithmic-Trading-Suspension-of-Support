using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace ShareInvest.Hubs
{
	public class ChatHub : Hub
	{
		public async Task SendMessage(string user, string message) => await Clients.All.SendAsync("ReceiveChatMessage", user, message);
		public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
		public override async Task OnDisconnectedAsync(Exception exception) => await base.OnDisconnectedAsync(exception);
	}
}