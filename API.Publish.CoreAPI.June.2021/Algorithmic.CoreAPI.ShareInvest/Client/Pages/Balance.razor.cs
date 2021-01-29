using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
		protected internal Dictionary<string, Catalog.Models.Balance> Balance
		{
			get; private set;
		}
		protected override async Task OnInitializedAsync()
		{
			Balance = new Dictionary<string, Catalog.Models.Balance>();
			Hub = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/balance")).Build();
			Hermes = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/hermes")).Build();
			Hub.On<Catalog.Models.Balance>("ReceiveBalanceMessage", (balance) => StateHasChanged(balance));
			Hermes.On<Catalog.Models.Message>("ReceiveCurrentMessage", (current) => StateHasChanged(current));
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
						if (Balance.Remove(balance.Code))
							StateHasChanged();
					}
					else
					{
						Balance[balance.Code] = balance;
						StateHasChanged();
					}
					break;

				case Catalog.Models.Message message when Balance.TryGetValue(message.Key, out Catalog.Models.Balance bal) && int.TryParse(bal.Quantity, out int quantity) && quantity > 0 && int.TryParse(bal.Purchase, out int purchase) && int.TryParse(message.Convey, out int price) && int.TryParse(bal.Current, out int current) && price != current:
					Balance[bal.Code] = new Catalog.Models.Balance
					{
						Account = bal.Account,
						Code = message.Key,
						Name = bal.Name,
						Quantity = bal.Quantity,
						Purchase = bal.Purchase,
						Current = price.ToString("N0"),
						Revenue = ((Math.Abs(price) - purchase) * quantity).ToString("C0"),
						Rate = (price / (double)purchase - 1).ToString("P2"),
						Separation = bal.Separation,
						Trend = bal.Trend
					};
					StateHasChanged();
					break;
			}
		}
		HubConnection Hub
		{
			get; set;
		}
		HubConnection Hermes
		{
			get; set;
		}
	}
}