using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace ShareInvest.Hubs
{
	public class MessageHub : Hub
	{
		public async Task SendMessage(string message) => await Clients.All.SendAsync("ReceiveMessage", message);
	}
}