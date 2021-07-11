using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using ShareInvest.Components;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class LogBase : LoadingFragment
	{
		protected internal DateTime Temp
		{
			get; set;
		}
		protected internal Catalog.Models.Log[] Logs
		{
			get; private set;
		}
		protected override async Task OnInitializedAsync()
		{
			try
			{
				Temp = DateTime.UnixEpoch;
				Logs = await Http.GetFromJsonAsync<Catalog.Models.Log[]>(Crypto.Security.GetRoute("Message", await OnReceiveLogUserInformation()));
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
		async Task<string> OnReceiveLogUserInformation()
		{
			var user = (await State).User;

			return user.Identity.IsAuthenticated ? user.Identity.Name : string.Empty;
		}
		[CascadingParameter]
		Task<AuthenticationState> State
		{
			get; set;
		}
		[Inject]
		HttpClient Http
		{
			get; set;
		}
	}
}