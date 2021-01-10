using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace ShareInvest.Hubs
{
	public class BalanceHub : Hub
	{
		public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
		public override async Task OnDisconnectedAsync(Exception exception) => await base.OnDisconnectedAsync(exception);
		public async Task SendBalanceMessage(Catalog.Models.Balance balance) => await Clients.All.SendAsync("ReceiveBalanceMessage", balance);
	}
}