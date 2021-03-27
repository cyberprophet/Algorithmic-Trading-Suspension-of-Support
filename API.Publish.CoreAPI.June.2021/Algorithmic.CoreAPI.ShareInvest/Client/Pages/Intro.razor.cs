using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

namespace ShareInvest.Pages
{
	public partial class IntroBase : ComponentBase
	{
		protected internal Catalog.Dart.Theme[] Collection
		{
			get; private set;
		}
		protected internal Catalog.Dart.Theme Pick => Collection.OrderBy(o => Guid.NewGuid()).First();
		protected internal string User
		{
			get; private set;
		}
		protected internal bool Click
		{
			get; set;
		}
		protected override async Task OnInitializedAsync()
		{
			if (await OnReceiveLogUserInformation() is string response)
			{
				Click = string.IsNullOrEmpty(response) is false;

				if (Click)
				{
					var sb = new StringBuilder();

					foreach (int str in response.ToCharArray())
						sb.Append(str.ToString("D3")).Append('/');

					User = sb.Remove(sb.Length - 1, 1).ToString();
					Collection = await Http.GetFromJsonAsync<Catalog.Dart.Theme[]>(Crypto.Security.GetRoute(nameof(Catalog.Dart.Theme), nameof(Intro)));
				}
				else
					User = response;
			}
		}
		protected internal void OnClick(MouseEventArgs _) => Click = Click is false;
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