using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

using System;
using System.Threading.Tasks;

namespace ShareInvest.Shared
{
	public partial class MainLayoutBase : LayoutComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync() => await Hub.DisposeAsync();
		protected override async Task OnInitializedAsync()
		{
			if (await OnReceiveLogUserInformation())
			{
				Hub = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/message"), o => o.AccessTokenProvider = async () =>
				{
					(await TokenProvider.RequestAccessToken()).TryGetToken(out var accessToken);

					return accessToken.Value;

				}).Build();
				Hub.On<string>("ReceiveMessage", (message) =>
				{
					Message = message[^1] is '.' ? message : string.Concat(message, '.');
					StateHasChanged();
				});
				await Hub.StartAsync();
			}
			await Runtime.InvokeVoidAsync(string.Concat(interop, "show"), "none");
		}
		protected internal string Message
		{
			get; private set;
		}
		HubConnection Hub
		{
			get; set;
		}
		[Inject]
		NavigationManager Manager
		{
			get; set;
		}
		[Inject]
		IJSRuntime Runtime
		{
			get; set;
		}
		[Inject]
		IAccessTokenProvider TokenProvider
		{
			get; set;
		}
		[CascadingParameter]
		Task<AuthenticationState> State
		{
			get; set;
		}
		async Task<bool> OnReceiveLogUserInformation() => (await State).User.Identity.IsAuthenticated;
		const string interop = "JsFunctions.";
	}
}