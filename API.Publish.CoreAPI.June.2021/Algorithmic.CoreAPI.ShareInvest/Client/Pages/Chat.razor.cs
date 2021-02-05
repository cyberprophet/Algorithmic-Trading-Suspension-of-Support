using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class ChatBase : ComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync() => await Hub.DisposeAsync();
		protected override async Task OnInitializedAsync()
		{
			User = (await OnReceiveLogUserInformation()).Split('@')[0];
			Messages = new List<Tuple<uint, string, string>>();
			Hub = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/chat")).Build();
			Hub.On<string, string>("ReceiveChatMessage", (user, message) =>
			{
				Messages.Add(new Tuple<uint, string, string>(Count++, user, message));

				if (User.Equals(user))
					StateHasChanged();
			});
			await Hub.StartAsync();
		}
		protected internal async void SendMessage(KeyboardEventArgs e)
		{
			if (Enum.TryParse(e.Key, out ConsoleKey key) && ConsoleKey.Enter.Equals(key) && string.IsNullOrWhiteSpace(Message) is false)
			{
				await Hub.SendAsync("SendMessage", User, Message);
				Message = string.Empty;
			}
			if (Messages.Count > 0 && Message.Length < 2 && (e.Key.Length == 1 || ConsoleKey.Enter.Equals(key)))
				(Runtime as IJSInProcessRuntime).InvokeVoid(string.Concat(interop, "scroll"));
		}
		protected internal bool IsConnected => Hub.State == HubConnectionState.Connected;
		[Inject]
		protected internal NavigationManager Manager
		{
			get; set;
		}
		protected internal List<Tuple<uint, string, string>> Messages
		{
			get; private set;
		}
		protected internal string User
		{
			get; private set;
		}
		protected internal string Message
		{
			get; set;
		}
		uint Count
		{
			get; set;
		}
		HubConnection Hub
		{
			get; set;
		}
		[CascadingParameter]
		Task<AuthenticationState> State
		{
			get; set;
		}
		[Inject]
		IJSRuntime Runtime
		{
			get; set;
		}
		async Task<string> OnReceiveLogUserInformation()
		{
			var user = (await State).User;

			return user.Identity.IsAuthenticated ? user.Identity.Name : string.Empty;
		}
		const string interop = "JsFunctions.";
	}
}