using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class DownloadBase : ComponentBase
	{
		protected internal async Task TryToDownloadAsync() => await Runtime.InvokeVoidAsync(string.Concat(interop, "download"), Path.Combine(Http.BaseAddress.AbsoluteUri, Crypto.Security.GetRoute("Files", await OnReceiveLogUserInformation())));
		async Task<string> OnReceiveLogUserInformation()
		{
			var user = (await State).User;

			return user.Identity.IsAuthenticated ? user.Identity.Name : null;
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
		const string interop = "JsFunctions.";
	}
}