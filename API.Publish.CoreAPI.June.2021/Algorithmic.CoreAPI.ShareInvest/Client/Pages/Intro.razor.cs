using System;
using System.Collections.Generic;
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
		protected internal DateTime[] Initialize
		{
			get; private set;
		}
		protected internal string User
		{
			get; private set;
		}
		protected internal Dictionary<int, bool> IsClick
		{
			get; set;
		}
		protected override async Task OnInitializedAsync()
		{
			if (await OnReceiveLogUserInformation() is string response)
			{
				IsClick = new Dictionary<int, bool>();

				if (string.IsNullOrEmpty(response))
					User = response;

				else
				{
					var sb = new StringBuilder();

					foreach (int str in response.ToCharArray())
						sb.Append(str.ToString("D3")).Append('/');

					User = sb.Remove(sb.Length - 1, 1).ToString();
					Collection = await Http.GetFromJsonAsync<Catalog.Dart.Theme[]>(Crypto.Security.GetRoute(nameof(Catalog.Dart.Theme), nameof(Intro)));
				}
			}
		}
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				var past = 0;
				var now = DateTime.Now;
				Initialize = new DateTime[0xC];

				for (int pi = 0; pi < Initialize.Length - 1; pi++)
				{
					IsClick[pi] = false;

					if (pi == 0)
					{
						Initialize[pi] = now;

						continue;
					}
					while (Base.DisplayThePage(now.AddDays(past - pi)))
						past--;

					Initialize[pi] = now.AddDays(past - pi);
				}
				IsClick[Initialize.Length - 1] = true;
				Initialize[^1] = DateTime.UnixEpoch;
			}
		}
		protected internal void OnClick(int index, MouseEventArgs _)
		{
			for (int i = 0; i < Initialize.Length; i++)
				IsClick[i] = false;

			IsClick[index] = IsClick[index] is false;
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