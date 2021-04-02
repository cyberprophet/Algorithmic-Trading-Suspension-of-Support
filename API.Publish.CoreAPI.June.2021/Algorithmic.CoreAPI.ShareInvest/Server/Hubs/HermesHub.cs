using System;
using System.Collections.Generic;
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

				case Tuple<char, string> condition:
					switch (condition.Item1)
					{
						case 'I' when Security.Conditions[^1].Add(condition.Item2):

							break;

						case 'D' when Security.Conditions[^1].Remove(condition.Item2):

							break;

						case > (char)47 and < (char)58:
							var index = 9 - condition.Item1 + 0x30;
							Security.Conditions[index] = new HashSet<string>();

							foreach (var code in condition.Item2.Split(';'))
								if (string.IsNullOrEmpty(code) is false && Security.Conditions[index].Add(code))
									Base.SendMessage(GetType(), index, code);

							break;
					}
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
						{
							Process.Start(Security.StartInfo);
							Security.Conditions[^1].Clear();
						}
					}
					return;
			}
		}
		public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
		public override async Task OnDisconnectedAsync(Exception exception) => await base.OnDisconnectedAsync(exception);
	}
}