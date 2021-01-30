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
					case string kiwoom when kiwoom.Equals(Kiwoom) && string.IsNullOrWhiteSpace(kiwoom) is false && kiwoom.Length < 9 && param is string confirm && string.IsNullOrEmpty(confirm) is false && confirm.Length > 0 && confirm.Length < 9:
						var name = Crypto.Security.ConvertName(confirm).Trim();
						var append = new Confirm
						{
							Email = await OnReceiveLogUserInformation(),
							Key = Crypto.Security.Encrypt(kiwoom),
							First = name[0],
							Last = name[^1]
						};
						var response = await Http.PostAsJsonAsync(Crypto.Security.GetRoute(append.GetType()), append);

						if (HttpStatusCode.OK.Equals(response.StatusCode))
							Information = JsonConvert.DeserializeObject<UserInformation[]>(await response.Content.ReadAsStringAsync());

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
		protected internal async Task Send(MouseEventArgs _)
		{
			if (Information is not null && Information.Length > 0)
			{
				foreach (var info in Information)
				{
					var response = await Http.PutAsJsonAsync(Crypto.Security.GetRoute("Account"), info);

					if (HttpStatusCode.OK.Equals(response.StatusCode))
						IsClicked = false;

					else
						Caution = "Account linking Error.";
				}
				Information = null;
				await OnInitializedAsync();
			}
		}
		protected internal void OnReceiveTheSelectedButton(ChangeEventArgs e)
		{
			if (e.Value is string str)
			{
				var split = str.Split(';');
				var account = split[0];

				if (int.TryParse(split[^1], out int index))
				{
					if (account[^2..].CompareTo("31") == 0)
						Information[index] = new UserInformation
						{
							Key = Information[index].Key,
							Account = Information[index].Account,
							Name = Information[index].Name,
							Remaining = Information[index].Remaining,
							Check = string.Concat(Information[index].Check.Split(';')[0], ';', account)
						};
					else
						Information[index] = new UserInformation
						{
							Key = Information[index].Key,
							Account = Information[index].Account,
							Name = Information[index].Name,
							Remaining = Information[index].Remaining,
							Check = string.Concat(account, ';', Information[index].Check.Split(';')[^1])
						};
				}
				IsClicked = true;
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