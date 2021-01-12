using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ShareInvest.Pages
{
	public partial class BalanceBase : ComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync() => await Hub.DisposeAsync();
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
			Hub.On<Catalog.Models.Balance>("ReceiveBalanceMessage", (balance) =>
			{
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
			});
			await Hub.StartAsync();
		}
		HubConnection Hub
		{
			get; set;
		}
	}
}