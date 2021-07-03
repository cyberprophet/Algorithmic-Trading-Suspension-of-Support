using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
		protected internal object OnClick(int index, string param)
		{
			var closing = Accounts is null || Accounts[index] is null;

			if (closing is false)
				switch (param.Length)
				{
					case 5 when "총매입금액".Equals(param) is false && double.TryParse(Accounts[index][param], out double rate):
						if (rate < 0)
							return new Tuple<ConsoleColor, string>(ConsoleColor.Blue, Math.Abs(rate).ToString("P2"));

						return rate.ToString("P2");

					case 4 when int.TryParse(Accounts[index][param], out int hold):
						if (hold > 0)
							return hold.ToString("N0");

						return string.Empty;

					case 0:
						foreach (var kv in Accounts[index])
							if (Array.Exists(prefix, o => kv.Key.StartsWith(o)) && Array.Exists(suffix, o => kv.Key.EndsWith(o)))
								foreach (var confirm in kv.Value.ToCharArray())
									if (confirm is not '0')
										return closing;

						return true;

					default:
						if (long.TryParse(Accounts[index][param], out long principal))
						{
							if (principal < 0)
								return new Tuple<ConsoleColor, string>(ConsoleColor.Blue, Math.Abs(principal).ToString("C0"));

							return principal.ToString("C0");
						}
						return true;
				}
			return closing;
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
						var key = JToken.Parse(message)[nameof(account)].ToString();
						var index = Array.FindIndex(Information, o => o.Key.Equals(Information.Single(o => (o.Check.StartsWith(key) || o.Check.EndsWith(key)) && Array.Exists(o.Account, x => x.Equals(key))).Key));

						if (Count == int.MaxValue)
						{
							var account = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

							if (int.TryParse(account["출력건수"], out int count))
							{
								if (Accounts is null)
								{
									Accounts = new Dictionary<string, string>[Information.Length];
									Balances = new Catalog.OpenAPI.OPW00004[Information.Length][];
								}
								if (index > -1)
								{
									Accounts[index] = account;
									Balances[index] = new Catalog.OpenAPI.OPW00004[count];
									Count = count;
								}
							}
						}
						else
						{
							if (index > -1)
								Balances[index][--Count] = JsonConvert.DeserializeObject<Catalog.OpenAPI.OPW00004>(message);
						}
						if (Count == 0 && Connection.ContainsKey(key))
						{
							Connection[key] = true;
							StateHasChanged();
						}
					});
					await Hub.StartAsync();
				}
				if (Connection.ContainsKey(str) is false || Connection[str])
				{
					Count = int.MaxValue;
					Connection[str] = false;

					if ("31".Equals(str[^2..]) is false)
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
		protected internal string[] Prefix => prefix;
		protected internal string[] Suffix => suffix;
		protected internal string[] Holds => holds;
		protected internal string[] Deposit => deposit;
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
		protected internal Catalog.OpenAPI.OPW00004[][] Balances
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
		readonly string[] prefix = new[] { "당일", "당월", "누적" };
		readonly string[] suffix = new[] { "투자원금", "투자손익", "손익율" };
		readonly string[] holds = new[] { "출력건수", "유가잔고평가액", "예탁자산평가액", "추정예탁자산" };
		readonly string[] deposit = new[] { "예수금", "D+2추정예수금", "총매입금액", "매도담보대출금" };
	}
}