using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace ShareInvest.Hubs
{
	public class BalanceHub : Hub
	{
		public async Task SendBalanceMessage(Catalog.Models.Balance balance) => await Clients.All.SendAsync("ReceiveBalanceMessage", balance);
	}
}