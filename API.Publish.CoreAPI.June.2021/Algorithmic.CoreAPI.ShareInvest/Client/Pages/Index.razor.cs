using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;

using ShareInvest.Catalog.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class IndexBase : ComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync()
		{
			if (Hub is not null)
				await Hub.DisposeAsync();
		}
		protected async override Task OnInitializedAsync()
		{
			try
			{
				Connection = new Dictionary<string, bool>();
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
		protected internal async Task OnClick(object account, MouseEventArgs _)
		{
			if (account is string str && Information.Single(o => (o.Check.StartsWith(str) || o.Check.EndsWith(str)) && Array.Exists(o.Account, x => x.Equals(str))) is UserInformation info)
			{
				if (Hub is null)
				{
					Hub = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/account"), o => o.AccessTokenProvider = async () =>
					{
						(await TokenProvider.RequestAccessToken()).TryGetToken(out var accessToken);

						return accessToken.Value;

					}).Build();
					Hub.On<string>("ReceiveAccountMessage", (message) =>
					{
						try
						{
							if (Count == int.MaxValue)
							{
								var account = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

								if (str.Equals(account[nameof(account)]) && int.TryParse(account["출력건수"], out int count))
								{
									if (Accounts is null)
										Accounts = new Dictionary<string, string>[Information.Length];

									Accounts[Array.FindIndex(Information, o => o.Key.Equals(info.Key))] = account;
									Count = count;
								}
							}
							else
							{
								var balance = JsonConvert.DeserializeObject<Catalog.OpenAPI.OPW00004>(message);

								if (str.Equals(balance.Account))
								{


									Count--;
								}
							}
							if (Count == 0 && Connection.ContainsKey(str))
							{
								Connection[str] = true;
								StateHasChanged();
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					});
					await Hub.StartAsync();
				}
				if (Connection.ContainsKey(str) is false || Connection[str])
				{
					Count = int.MaxValue;
					Connection[str] = false;
					await Hub.SendAsync("SendMessage", info.Key, str);
				}
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
		protected internal Dictionary<string, bool> Connection
		{
			get; private set;
		}
		protected internal Dictionary<string, string>[] Accounts
		{
			get; private set;
		}
		[Inject]
		IAccessTokenProvider TokenProvider
		{
			get; set;
		}
		[Inject]
		HttpClient Http
		{
			get; set;
		}
		[Inject]
		NavigationManager Manager
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
		int Count
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