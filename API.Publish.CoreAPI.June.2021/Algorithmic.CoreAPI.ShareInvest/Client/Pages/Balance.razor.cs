using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

using ShareInvest.Components;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class BalanceBase : LoadingFragment, IAsyncDisposable
	{
		public async ValueTask DisposeAsync()
		{
			await Hub.DisposeAsync();
			await Hermes.DisposeAsync();
		}
		protected internal Dictionary<Tuple<string, string>, Catalog.Models.Balance> Balance
		{
			get; private set;
		}
		protected override async Task OnInitializedAsync()
		{
			Balance = new Dictionary<Tuple<string, string>, Catalog.Models.Balance>();

			foreach (var response in await Http.GetFromJsonAsync<Catalog.Models.Balance[]>(Crypto.Security.GetRoute("Balance", await OnReceiveLogUserInformation())))
				Balance[new Tuple<string, string>(response.Kiwoom, response.Code)] = response;

			Hub = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/balance"), o => o.AccessTokenProvider = async () =>
			{
				(await TokenProvider.RequestAccessToken()).TryGetToken(out var accessToken);

				return accessToken.Value;

			}).Build();
			Hermes = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/hermes")).Build();
			Hermes.On<Catalog.Models.Message>("ReceiveCurrentMessage", (current) => StateHasChanged(current));
			Hub.On<Catalog.Models.Balance>("ReceiveBalanceMessage", (balance) => StateHasChanged(balance));
			await Hermes.StartAsync();
			await Hub.StartAsync();
		}
		void StateHasChanged<T>(T param) where T : struct
		{
			var now = DateTime.Now;
			var render = false;

			switch (param)
			{
				case Catalog.Models.Balance balance:
					if (balance.Quantity.Length < 2 && balance.Quantity[0] is '0')
					{
						if (Balance.Remove(new Tuple<string, string>(balance.Kiwoom, balance.Code)))
							StateHasChanged();

						return;
					}
					Balance[new Tuple<string, string>(balance.Kiwoom, balance.Code)] = balance;
					render = true;
					break;

				case Catalog.Models.Message message when Balance.Any(o => o.Key.Item2.Equals(message.Key)):
					foreach (var kv in Balance)
						if (kv.Key.Item2.Equals(message.Key) && int.TryParse(kv.Value.Quantity.Replace(",", string.Empty), out int quantity))
						{
							if (quantity < 1)
							{
								if (Balance.Remove(kv.Key))
									StateHasChanged();

								return;
							}
							if (int.TryParse(message.Convey[0] is '-' ? message.Convey[1..] : message.Convey, out int price) && int.TryParse(kv.Value.Purchase.Replace(",", string.Empty), out int purchase))
							{
								Balance[kv.Key] = new Catalog.Models.Balance
								{
									Kiwoom = kv.Value.Kiwoom,
									Account = kv.Value.Account,
									Code = message.Key,
									Name = kv.Value.Name,
									Quantity = kv.Value.Quantity,
									Purchase = kv.Value.Purchase,
									Current = price.ToString("N0"),
									Revenue = ((price - purchase) * quantity).ToString("C0"),
									Rate = (price / (double)purchase - 1).ToString("P2")
								};
								render = true;
							}
						}
					break;
			}
			if (render && now.CompareTo(Time) > 0)
			{
				Time = now.AddSeconds(3);
				StateHasChanged();
			}
		}
		DateTime Time
		{
			get; set;
		}
		[Inject]
		IAccessTokenProvider TokenProvider
		{
			get; set;
		}
		HubConnection Hub
		{
			get; set;
		}
		HubConnection Hermes
		{
			get; set;
		}
		[Inject]
		NavigationManager Manager
		{
			get; set;
		}
		[Inject]
		HttpClient Http
		{
			get; set;
		}
		[CascadingParameter]
		Task<AuthenticationState> State
		{
			get; set;
		}
		async Task<string> OnReceiveLogUserInformation()
		{
			var user = (await State).User;

			return user.Identity.IsAuthenticated ? user.Identity.Name : string.Empty;
		}
	}
}