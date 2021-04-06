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
						case 'I' when condition.Item2.Split(';') is string[] insert && int.TryParse(insert[0], out int append):
							if (append > 9)
							{
								if (Security.Conditions[append].Add(insert[^1]) && Clients is not null)
									await SendMessage(new Message
									{
										Key = condition.Item1.ToString(),
										Convey = insert[^1]
									});
								return;
							}
							if (Security.Conditions[9 - append].Add(insert[^1]))
								Base.SendMessage(sender.GetType(), condition.Item1, insert[0], insert[^1]);

							break;

						case 'D' when condition.Item2.Split(';') is string[] delete && int.TryParse(delete[0], out int remove):
							if (remove > 9)
							{
								if (Security.Conditions[remove].Remove(delete[^1]) && Clients is not null)
									await SendMessage(new Message
									{
										Key = condition.Item1.ToString(),
										Convey = delete[^1]
									});
								return;
							}
							if (Security.Conditions[9 - remove].Remove(delete[^1]))
								Base.SendMessage(sender.GetType(), condition.Item1, delete[0], delete[^1]);

							break;

						case > (char)0x2F and < (char)0x3A:
							var index = 9 - condition.Item1 + 0x30;
							Security.Conditions[index] = new HashSet<string>();

							foreach (var code in condition.Item2.Split(';'))
								if (string.IsNullOrEmpty(code) is false && Security.Conditions[index].Add(code))
									Base.SendMessage(GetType(), index, code);

							break;

						case > (char)0x40 and < (char)0x5B:
							var real = 0xA - condition.Item1 + 0x41;
							Security.Conditions[real] = new HashSet<string>();

							foreach (var code in condition.Item2.Split(';'))
								if (string.IsNullOrEmpty(code) is false && Security.Conditions[real].Add(code))
									Base.SendMessage(GetType(), real, code);

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