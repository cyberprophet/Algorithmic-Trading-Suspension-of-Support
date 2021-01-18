using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class LogBase : ComponentBase
	{
		[Inject]
		HttpClient Http
		{
			get; set;
		}
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
				Logs = await Http.GetFromJsonAsync<Catalog.Models.Log[]>(Crypto.Security.GetRoute("Message", Security.Identify));
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
	}
}