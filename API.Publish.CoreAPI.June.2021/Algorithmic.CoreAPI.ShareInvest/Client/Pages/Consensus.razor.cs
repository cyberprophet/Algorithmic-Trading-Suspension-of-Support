using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

using ShareInvest.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class ConsensusBase : LoadingFragment
	{
		protected override async Task OnInitializedAsync()
		{
			var index = 0;
			Key = new Dictionary<string, int>();
			Dictionary = new Dictionary<string, Queue<Catalog.Models.Consensus>>();

			try
			{
				foreach (var con in await Http.GetFromJsonAsync<Catalog.Models.Consensus[]>(Crypto.Security.GetRoute("Consensus")))
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
		protected internal void OnClick(int sender, char initial, MouseEventArgs _)
		{
			if (Enumerable.TryGetValue(sender, out Catalog.Models.Portfolio portfolio))
			{
				switch (initial)
				{
					case 'A':
						if (portfolio.Temp is char.MinValue)
							portfolio.Temp = portfolio.SelectStrategics;

						break;

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
		protected internal static string ConvertFormat(string param) => $"’{param.Substring(0, 2)}. {param.Substring(2, 2)}.";
		protected internal static (string, ConsoleColor) ConvertFormat(double param) => (param < 0 ? param.ToString("P2")[1..] : param.ToString("P2"), param > 0 ? ConsoleColor.Red : ConsoleColor.Blue);
		[Inject]
		HttpClient Http
		{
			get; set;
		}
		[Inject]
		IJSRuntime Runtime
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
		Dictionary<string, Queue<Catalog.Models.Consensus>> Dictionary
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