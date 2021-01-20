using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest.Hubs
{
	public class HermesHub : Hub
	{
		public async Task SendMessage(Message message) => await Clients.All.SendAsync("ReceiveCurrentMessage", message);
		public async void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e)
		{
			switch (e.Convey)
			{
				case Message message when Clients is not null:
					await SendMessage(message);
					return;

				case string:
					Base.SendMessage(GetType(), e.Convey as string);
					return;

				case short:
					var now = DateTime.Now;

					if (DayOfWeek.Sunday.Equals(now.DayOfWeek) && now.Hour < 4)
					{
						Dispose();
						Security.User.Clear();
						Security.Host.Dispose();
					}
					else
					{
						await Task.Delay(0x7CE7);
						GC.Collect();

						if (Base.IsDebug is false)
							Process.Start(Security.StartInfo);
					}
					return;
			}
		}
		public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
		public override async Task OnDisconnectedAsync(Exception exception) => await base.OnDisconnectedAsync(exception);
	}
}