using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ShareInvest.Shared
{
	public partial class MainLayoutBase : LayoutComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync() => await Hub.DisposeAsync();
		protected override async Task OnInitializedAsync()
		{
			Hub = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/message")).Build();
			Hub.On<string>("ReceiveMessage", (message) =>
			{
				Message = message[^1] is '.' ? message : string.Concat(message, '.');
				StateHasChanged();
			});
			await Hub.StartAsync();
		}
		[Inject]
		protected internal NavigationManager Manager
		{
			get; set;
		}
		protected internal string Message
		{
			get; private set;
		}
		HubConnection Hub
		{
			get; set;
		}
	}
}