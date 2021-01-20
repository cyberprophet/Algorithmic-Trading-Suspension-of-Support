using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Pages
{
	[AllowAnonymous]
	public partial class LoginBase : ComponentBase
	{
		protected internal string Email
		{
			get; set;
		}
		protected internal string Password
		{
			get; set;
		}
		protected internal async Task Send(MouseEventArgs _)
		{
			var token = new Token
			{
				Email = Email,
				Password = Password
			};
			var response = await Http.PostAsJsonAsync<object>(Crypto.Security.GetRoute(token.GetType()), token);
			Base.SendMessage(GetType(), response.Content.ToString());
		}
		[Inject]
		HttpClient Http
		{
			get; set;
		}
	}
}