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

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class BalanceBase : ComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync()
		{
			await Hub.DisposeAsync();
			await Hermes.DisposeAsync();
		}
		[Inject]
		protected internal NavigationManager Manager
		{
			get; set;
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
			switch (param)
			{
				case Catalog.Models.Balance balance:
					if (balance.Quantity.Length < 2 && balance.Quantity[0] is '0')
					{
						if (Balance.Remove(new Tuple<string, string>(balance.Kiwoom, balance.Code)))
							StateHasChanged();
					}
					else
					{
						Balance[new Tuple<string, string>(balance.Kiwoom, balance.Code)] = balance;
						StateHasChanged();
					}
					break;

				case Catalog.Models.Message message when Balance.Any(o => o.Key.Item2.Equals(message.Key)) && (message.Convey[0] is '-' or '+' ? message.Convey[1..] : message.Convey) is string convey:
					var now = DateTime.Now;
					var render = false;

					foreach (var kv in Balance)
						if (kv.Key.Item2.Equals(message.Key) && convey.Equals(kv.Value.Current.Replace(",", string.Empty)) is false && int.TryParse(kv.Value.Quantity, out int quantity) && quantity > 0 && int.TryParse(convey, out int price) && int.TryParse(kv.Value.Purchase.Replace(",", string.Empty), out int purchase))
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
								Revenue = ((Math.Abs(price) - purchase) * quantity).ToString("C0"),
								Rate = (price / (double)purchase - 1).ToString("P2")
							};
							render = true;
						}
					if (render && Second != now.Second)
					{
						Second = now.Second;
						StateHasChanged();
					}
					break;
			}
		}
		int Second
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