using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest.Hubs
{
	public class HermesHub : Hub
	{
		public async Task SendMessage(Message message) => await Clients.All.SendAsync("ReceiveMessage", message);
		public async void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e)
		{
			switch (e.Convey)
			{
				case Message message:
					await SendMessage(message);
					return;

				case string:
					Base.SendMessage(GetType(), e.Convey as string);
					return;

				case short:
					Security.Host.Dispose();
					return;
			}
		}
	}
}