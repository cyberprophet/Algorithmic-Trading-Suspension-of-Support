using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class PortfolioBase : ComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync()
		{
			await Balance.DisposeAsync();
			await Hermes.DisposeAsync();
		}
		protected override async Task OnInitializedAsync()
		{
			var index = 0;
			Key = new Dictionary<string, int>();
			Dictionary = new Dictionary<string, Queue<Catalog.Models.Consensus>>();

			try
			{
				foreach (var con in await Http.GetFromJsonAsync<Catalog.Models.Consensus[]>("Consensus"))
				{
					if (Dictionary.TryGetValue(con.Code, out Queue<Catalog.Models.Consensus> queue))
					{
						queue.Enqueue(con);
						Dictionary[con.Code] = queue;
					}
					else
						Dictionary[con.Code] = new Queue<Catalog.Models.Consensus>(new Catalog.Models.Consensus[] { con });
				}
				Enumerable = new Dictionary<int, Catalog.Models.Portfolio>();

				foreach (var consensus in Dictionary.OrderBy(o => o.Key))
				{
					Key[consensus.Key] = index;
					Key[consensus.Value.Peek().Date] = index;
					Enumerable[index++] = new Catalog.Models.Portfolio
					{
						Consensus = consensus.Value,
						RenderingBalance = true,
						SelectStrategics = 'D'
					};
				}
				index = 0;
				Quarter = new string[6];

				foreach (var near in Base.FindTheNearestQuarter(DateTime.Now))
					Quarter[index++] = ConvertFormat(near);
			}
			catch (AccessTokenNotAvailableException exception)
			{
				exception.Redirect();
			}
			catch (Exception ex)
			{
				Base.SendMessage(ex.StackTrace, GetType());
			}
		}
		protected override async Task OnAfterRenderAsync(bool render)
		{
			if (render)
			{
				Balance = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/balance")).Build();
				Hermes = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/hermes")).Build();
				Balance.On<Catalog.Models.Balance>("ReceiveBalanceMessage", (balance) => StateHasChanged(balance));
				Hermes.On<Catalog.Models.Message>("ReceiveCurrentMessage", (current) => StateHasChanged(current));
				await Balance.StartAsync();
				await Hermes.StartAsync();
			}
		}
		protected internal void OnClick(int sender, MouseEventArgs _)
		{
			if (Enumerable.TryGetValue(sender, out Catalog.Models.Portfolio portfolio))
				portfolio.IsClickedAmend = portfolio.IsClickedAmend is false;
		}
		protected internal void OnClick(int sender, char initial, MouseEventArgs _)
		{
			if (Enumerable.TryGetValue(sender, out Catalog.Models.Portfolio portfolio))
			{
				switch (initial)
				{
					case 'A':
					case 'D':
						if (portfolio.Temp is char.MinValue)
							portfolio.Temp = portfolio.SelectStrategics;

						break;

					case 'B':
						portfolio.RenderingBalance = portfolio.RenderingBalance is false;
						return;

					case 'C':
						portfolio.RenderingConsensus = portfolio.RenderingConsensus is false;
						return;

					case 'S':
						if (portfolio.RenderingStrategics)
						{
							if (portfolio.Temp > char.MinValue)
								portfolio.SelectStrategics = portfolio.Temp;

							portfolio.IsClickedAmend = portfolio.RenderingStrategics is false;
						}
						portfolio.RenderingStrategics = portfolio.RenderingStrategics is false;
						return;

					default:
						return;
				}
				portfolio.SelectStrategics = initial;
			}
		}
		protected internal async void OnReceiveKeyPress(ChangeEventArgs e)
		{
			if (e.Value is string str && string.IsNullOrEmpty(str) is false)
				foreach (var name in new[] { str, str.ToUpper() })
					if (Key.TryGetValue(name, out int index))
					{
						await WaitForTheScrollToMovement(index, 0x95);

						break;
					}
		}
		protected internal string[] Quarter
		{
			get; private set;
		}
		protected internal int Index
		{
			get; set;
		}
		protected internal Dictionary<int, Catalog.Models.Portfolio> Enumerable
		{
			get; private set;
		}
		[Inject]
		protected internal NavigationManager Manager
		{
			get; set;
		}
		[Inject]
		protected internal IJSRuntime Runtime
		{
			get; set;
		}
		protected internal static string ConvertFormat(char initial, string param)
		{
			var unit = string.Empty;

			switch (initial)
			{
				case 'D':
					unit = "일";
					break;

				case 'M':
					unit = "㎳";
					break;

				case 'S':
					unit = "주";
					break;

				case 'T':
					unit = "호가";
					break;

				case '%':
					break;
			}
			return string.Concat(param, string.IsNullOrEmpty(unit) ? initial : unit);
		}
		protected internal static string ConvertFormat(string param) => string.Format("’{0}. {1}.", param.Substring(0, 2), param.Substring(2, 2));
		protected internal static (string, ConsoleColor) ConvertFormat(double param)
				=> (param < 0 ? param.ToString("P2")[1..] : param.ToString("P2"), param > 0 ? ConsoleColor.Red : ConsoleColor.Blue);
		[Inject]
		HttpClient Http
		{
			get; set;
		}
		async Task WaitForTheScrollToMovement(int index, int pixel)
		{
			await Runtime.InvokeVoidAsync(string.Concat(interop, "move"), (index - Index) * pixel);
			await Task.Delay(0x700);

			if (Index >= index && index > Index - 0x14 || Index < 0x18 && index < 0x18)
				await Runtime.InvokeVoidAsync(string.Concat(interop, "selector"), index);

			else
				await WaitForTheScrollToMovement(index, pixel);
		}
		void StateHasChanged<T>(T param) where T : struct
		{
			switch (param)
			{
				case Catalog.Models.Balance balance when Enumerable.TryGetValue(Key[balance.Code], out Catalog.Models.Portfolio port):
					port.Balance = balance;
					break;

				case Catalog.Models.Message message when Enumerable.TryGetValue(Key[message.Key], out Catalog.Models.Portfolio port) && int.TryParse(message.Convey, out int price):
					if (string.IsNullOrEmpty(port.Balance.Quantity) || port.Balance.Quantity[0] is '0')
						port.Balance = new Catalog.Models.Balance
						{
							Code = message.Key,
							Current = price.ToString("N0"),
							Quantity = "0"
						};
					else if (int.TryParse(port.Balance.Quantity, out int quantity) && quantity > 0 && int.TryParse(port.Balance.Purchase, out int purchase))
						port.Balance = new Catalog.Models.Balance
						{
							Code = message.Key,
							Name = port.Balance.Name,
							Quantity = port.Balance.Quantity,
							Purchase = port.Balance.Purchase,
							Current = price.ToString("N0"),
							Revenue = ((Math.Abs(price) - purchase) * quantity).ToString("C0"),
							Rate = (price / (double)purchase - 1).ToString("P2"),
							Separation = port.Balance.Separation,
							Trend = port.Balance.Trend
						};
					break;
			}
			if (Count++ > 0x300)
			{
				Count = uint.MinValue;
				StateHasChanged();
			}
		}
		HubConnection Balance
		{
			get; set;
		}
		HubConnection Hermes
		{
			get; set;
		}
		Dictionary<string, Queue<Catalog.Models.Consensus>> Dictionary
		{
			get; set;
		}
		Dictionary<string, int> Key
		{
			get; set;
		}
		uint Count
		{
			get; set;
		}
		const string interop = "JsFunctions.";
	}
}