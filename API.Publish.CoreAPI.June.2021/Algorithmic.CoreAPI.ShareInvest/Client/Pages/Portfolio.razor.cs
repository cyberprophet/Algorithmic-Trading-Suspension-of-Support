using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using Newtonsoft.Json;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class PortfolioBase : ComponentBase
	{
		internal object RetrieveRecordedInformation(Interface.Strategics strategics, string account, string code)
		{
			if (Array.Exists(Enumerable, o => o.Code.Equals(code)))
				foreach (var find in Enumerable.Where(o => o.Trend == (int)strategics && o.Code.Equals(code)))
				{
					var date = new DateTime(find.Date);

					switch (find)
					{
						case Catalog.LongPosition lp when lp.Account.Equals(account):
							return new Tuple<string, string, string>(lp.Underweight.ToString("P5").Replace("%", string.Empty), lp.Overweight.ToString("N0"), lp.Date > 0 ? string.Concat(date.ToLongDateString(), " ", date.ToLongTimeString()) : string.Empty);

						case Catalog.Scenario scenario when scenario.Account.Equals(account):
							return new Tuple<int, int, string, string, string, string>(scenario.Short, scenario.Long, scenario.Maximum.ToString("N0"), scenario.Hope.ToString("N0"), scenario.Target.ToString("P5").Replace("%", string.Empty), scenario.Date > 0 ? string.Concat(date.ToLongDateString(), " ", date.ToLongTimeString()) : string.Empty);
					}
				}
			return null;
		}
		protected override async Task OnInitializedAsync()
		{
			Codes = await Http.GetFromJsonAsync<Codes[]>(Crypto.Security.GetRoute("Confirm", "Stocks"));
			Information = await Http.GetFromJsonAsync<UserInformation[]>(Crypto.Security.GetRoute("Account", await OnReceiveLogUserInformation()));
		}
		protected override async Task OnAfterRenderAsync(bool render)
		{
			if (render)
			{
				Complete = save;
				IsClicked = new Dictionary<string, bool>();
				ChosenCodes = new Dictionary<string, string>();
				ChosenStrategics = new Dictionary<string, string>();
			}
			else
			{
				var enumerable = await Http.GetFromJsonAsync<BringIn[]>(Crypto.Security.GetRoute(portfolio, await OnReceiveLogUserInformation()));
				Enumerable = new Interface.IStrategics[enumerable.Length];
				Bring = new BringIn[enumerable.Length];

				for (int i = 0; i < enumerable.Length; i++)
				{
					Bring[i] = new BringIn
					{
						Code = enumerable[i].Code,
						Date = enumerable[i].Date,
						Methods = enumerable[i].Methods,
						Strategics = enumerable[i].Strategics,
						Contents = await OnReceiveContextAsync(enumerable[i].Code)
					};
					if (Enum.TryParse(enumerable[i].Strategics, out Interface.Strategics strategics))
						switch (strategics)
						{
							case Interface.Strategics.Long_Position:
								Enumerable[i] = JsonConvert.DeserializeObject<Catalog.LongPosition>(enumerable[i].Contents);
								break;

							case Interface.Strategics.Scenario:
								Enumerable[i] = JsonConvert.DeserializeObject<Catalog.Scenario>(enumerable[i].Contents);
								break;
						}
				}
			}
		}
		protected internal void OnReceiveTheChoiceItem(string sender, ChangeEventArgs e) => ChosenStrategics[sender] = e.Value as string;
		protected internal void OnReceiveTheChoiceItem(ChangeEventArgs e, string sender) => ChosenCodes[sender] = e.Value as string;
		protected internal async void RequestSave(int name, string sender, MouseEventArgs _)
		{
			IsClicked[sender] = true;
			string json = null;

			switch (Enum.ToObject(typeof(Interface.Strategics), name))
			{
				case Interface.Strategics.Long_Position when ulong.TryParse((await Runtime.InvokeAsync<string>(string.Concat(interop, recall), string.Concat(sender, name))).Replace(",", string.Empty), out ulong over) && double.TryParse(await Runtime.InvokeAsync<string>(string.Concat(interop, recall), string.Concat(name, sender)), out double under) && over > 0 && double.IsNaN(under) is false:
					json = JsonConvert.SerializeObject(new Catalog.LongPosition
					{
						Account = sender,
						Code = ChosenCodes[sender],
						Underweight = under * 1e-2,
						Overweight = over,
						Trend = name,
						Date = DateTime.Now.Ticks
					});
					break;

				case Interface.Strategics.Scenario:
					var response = new string[5];
					var location = new[] { 0, 1, 3, 5, sender.Length };

					for (int index = 0; index < location.Length; index++)
						response[index] = await Runtime.InvokeAsync<string>(string.Concat(interop, recall), sender.Insert(location[index], name.ToString("D2")));

					if (double.TryParse(response[^2], out double rate) && int.TryParse(response[location.Length - 1].Replace(",", string.Empty), out int price) && long.TryParse(response[0].Replace(",", string.Empty), out long max) && int.TryParse(response[1], out int sell) && int.TryParse(response[2], out int buy) && buy > 0 && sell > 0 && price > 0 && max > 0 && double.IsNaN(rate) is false)
						json = JsonConvert.SerializeObject(new Catalog.Scenario
						{
							Account = sender,
							Code = ChosenCodes[sender],
							Date = DateTime.Now.Ticks,
							Trend = name,
							Short = sell,
							Long = buy,
							Maximum = max,
							Hope = price,
							Target = rate * 1e-2
						});
					break;
			}
			if (string.IsNullOrEmpty(json) is false && HttpStatusCode.OK.Equals((await Http.PostAsJsonAsync(Crypto.Security.GetRoute(portfolio), new BringIn
			{
				Code = ChosenCodes[sender],
				Strategics = name.ToString("D2"),
				Methods = Information.First(o => o.Account.Any(exist => exist.Equals(sender))).Key,
				Contents = json

			})).StatusCode) && IsClicked.Remove(sender))
			{
				Complete = save;
				await OnAfterRenderAsync(IsClicked.Remove(sender));
			}
			else
				Complete = error;

			StateHasChanged();
		}
		protected internal async void RequestTheDeletionOfHoldings(string sender, MouseEventArgs _)
		{
			if (HttpStatusCode.OK.Equals((await Http.DeleteAsync(Crypto.Security.GetRoute(portfolio, sender))).StatusCode))
			{
				await OnAfterRenderAsync(false);
				StateHasChanged();
			}
		}
		protected internal void RequestVerification(string sender, MouseEventArgs _)
		{
			if (IsClicked.Remove(sender))
			{

			}
		}
		protected internal string Complete
		{
			get; private set;
		}
		protected internal BringIn[] Bring
		{
			get; private set;
		}
		protected internal Dictionary<string, bool> IsClicked
		{
			get; private set;
		}
		protected internal Dictionary<string, string> ChosenCodes
		{
			get; private set;
		}
		protected internal Dictionary<string, string> ChosenStrategics
		{
			get; private set;
		}
		protected internal Codes[] Codes
		{
			get; private set;
		}
		protected internal Interface.IStrategics[] Enumerable
		{
			get; private set;
		}
		protected internal UserInformation[] Information
		{
			get; private set;
		}
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
		[CascadingParameter]
		Task<AuthenticationState> State
		{
			get; set;
		}
		async Task<string> OnReceiveContextAsync(string code) => await Http.GetFromJsonAsync<string>(Crypto.Security.GetRoute("Exposure", code));
		async Task<string> OnReceiveLogUserInformation()
		{
			var user = (await State).User;

			return user.Identity.IsAuthenticated ? user.Identity.Name : string.Empty;
		}
		const string interop = "JsFunctions.";
		const string recall = "recall";
		const string portfolio = "Portfolio";
		const string save = "Save";
		const string error = "Error";
	}
}