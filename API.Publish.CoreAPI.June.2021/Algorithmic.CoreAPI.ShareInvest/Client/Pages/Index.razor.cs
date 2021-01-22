using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using Newtonsoft.Json;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class IndexBase : ComponentBase
	{
		protected async override Task OnInitializedAsync()
		{
			try
			{
				Information = await Http.GetFromJsonAsync<UserInformation[]>(Crypto.Security.GetRoute("Account", await OnReceiveLogUserInformation()));

				if (Information is null || Information.Length == 0)
					Caution = "정상적인 이메일로 가입하지 않았거나 입력하신 정보와 연결된 계좌가 없습니다.";
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
		protected internal async Task Send(object sender, object param, MouseEventArgs _)
		{
			try
			{
				switch (sender)
				{
					case string kiwoom when kiwoom.Equals(Kiwoom) && string.IsNullOrWhiteSpace(kiwoom) is false && kiwoom.Length < 9 && param is string confirm && string.IsNullOrEmpty(confirm) is false && confirm.Length > 2 && confirm.Length < 9:
						var append = new Confirm
						{
							Email = await OnReceiveLogUserInformation(),
							Key = Crypto.Security.Encrypt(kiwoom),
							First = confirm[0],
							Last = confirm[^1]
						};
						var response = await Http.PostAsJsonAsync(Crypto.Security.GetRoute(append.GetType()), append);

						if (HttpStatusCode.OK.Equals(response.StatusCode))
							Information = JsonConvert.DeserializeObject<UserInformation[]>(await response.Content.ReadAsStringAsync());

						break;

					case string stock when stock.Equals(Stock):

						break;
				}
				if (Information is null || Information.Length == 0)
					Caution = "You have not signed up with a legitimate email or do not have an account linked to the information entered.";

				else
					Caution = string.Empty;
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
			}
		}
		protected internal void OnReceiveTheSelectedButton(ChangeEventArgs e)
		{
			if (e.Value is string account)
			{
				if (account[^2..].CompareTo("31") == 0)
					Futures = account;

				else
					Stock = account;

				IsClicked = false;
			}
		}
		protected internal static string ConvertFormat(string account) => string.Format("{0}­ ─ ­{1}", account.Substring(0, 4), account.Substring(4, 4));
		protected internal bool IsClicked
		{
			get; private set;
		}
		protected internal string Kiwoom
		{
			get; set;
		}
		protected internal string Name
		{
			get; set;
		}
		protected internal string Stock
		{
			get; private set;
		}
		protected internal string Futures
		{
			get; private set;
		}
		protected internal string Caution
		{
			get; private set;
		}
		protected internal UserInformation[] Information
		{
			get; set;
		}
		[Inject]
		HttpClient Http
		{
			get; set;
		}
		[CascadingParameter]
		Task<AuthenticationState> State
		{
			get; set;
		}
		async Task<string> OnReceiveLogUserInformation()
		{
			var user = (await State).User;

			return user.Identity.IsAuthenticated ? user.Identity.Name : null;
		}
	}
}