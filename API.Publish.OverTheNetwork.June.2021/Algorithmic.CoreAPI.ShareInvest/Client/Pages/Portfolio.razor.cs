using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Pages
{
	public partial class PortfolioBase : ComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync() => await Hub.DisposeAsync();
		protected override async Task OnInitializedAsync()
		{
			Key = new Dictionary<string, int>();
			Enumerable = new Dictionary<int, Catalog.Models.Portfolio>();
			Dictionary = new Dictionary<string, Queue<Consensus>>();
			Quarter = new string[6];
			var index = 0;

			foreach (var con in await Http.GetFromJsonAsync<Consensus[]>(Crypto.Security.GetRoute("Consensus")))
			{
				if (Dictionary.TryGetValue(con.Code, out Queue<Consensus> queue))
				{
					queue.Enqueue(con);
					Dictionary[con.Code] = queue;
				}
				else
					Dictionary[con.Code] = new Queue<Consensus>(new Consensus[] { con });
			}
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

			foreach (var near in Base.FindTheNearestQuarter(DateTime.Now))
				Quarter[index++] = ConvertFormat(near);
		}
		protected override async Task OnAfterRenderAsync(bool render)
		{
			if (render)
			{
				Hub = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/balance")).Build();
				Hub.On<Catalog.Models.Balance>("ReceiveBalanceMessage", (balance) =>
				{
					if (Enumerable.TryGetValue(Key[balance.Code], out Catalog.Models.Portfolio port))
					{
						port.Balance = balance;
						StateHasChanged();
					}
				});
				await Hub.StartAsync();
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
		protected internal HttpClient Http
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
		async Task WaitForTheScrollToMovement(int index, int pixel)
		{
			await Runtime.InvokeVoidAsync(string.Concat(interop, "move"), (index - Index) * pixel);
			await Task.Delay(0x700);

			if (Index >= index && index > Index - 0x14 || Index < 0x18 && index < 0x18)
				await Runtime.InvokeVoidAsync(string.Concat(interop, "selector"), index);

			else
				await WaitForTheScrollToMovement(index, pixel);
		}
		HubConnection Hub
		{
			get; set;
		}
		Dictionary<string, Queue<Consensus>> Dictionary
		{
			get; set;
		}
		Dictionary<string, int> Key
		{
			get; set;
		}
		const string interop = "JsFunctions.";
	}
}