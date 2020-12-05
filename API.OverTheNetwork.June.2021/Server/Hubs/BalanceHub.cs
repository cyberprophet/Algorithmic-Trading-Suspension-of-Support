using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace ShareInvest.Hubs
{
	public class BalanceHub : Hub
	{
		public async Task SendMessage(string code, Balance balance) => await Clients.All.SendAsync("ReceiveBalanceMessage", code, balance);
	}
}