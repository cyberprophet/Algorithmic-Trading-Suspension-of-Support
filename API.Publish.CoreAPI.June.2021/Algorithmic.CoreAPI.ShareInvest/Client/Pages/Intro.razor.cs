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
using Microsoft.AspNetCore.SignalR.Client;

namespace ShareInvest.Pages
{
	public partial class IntroBase : ComponentBase, IAsyncDisposable
	{
		public async ValueTask DisposeAsync()
		{
			if (Hermes is HubConnection)
				await Hermes.DisposeAsync();
		}
		protected internal Catalog.Dart.Theme[] Collection
		{
			get; private set;
		}
		protected internal Catalog.Dart.Theme Pick => Collection.OrderBy(o => Guid.NewGuid()).First();
		protected internal DateTime[] Initialize
		{
			get; private set;
		}
		protected internal List<Catalog.Models.Intro>[] Conditions
		{
			get; private set;
		}
		protected internal Stack<Catalog.Models.News> Title
		{
			get; private set;
		}
		protected internal string User
		{
			get; private set;
		}
		protected internal bool Close
		{
			get; private set;
		}
		protected internal string[] Inclination
		{
			get; private set;
		}
		protected internal Catalog.Models.Codes[] Codes
		{
			get; private set;
		}
		protected internal Dictionary<string, Catalog.Models.News[]> Link
		{
			get; private set;
		}
		protected internal Dictionary<string, Tuple<int, double>> Sort
		{
			get; private set;
		}
		protected internal Dictionary<string, int> Stocks
		{
			get; private set;
		}
		protected internal Dictionary<string, bool> IsSort
		{
			get; private set;
		}
		protected internal Dictionary<int, bool> IsClick
		{
			get; set;
		}
		protected internal async void OnClick(int index, MouseEventArgs _)
		{
			OnClick(false);
			IsClick[index] = IsClick[index] is false;

			if (User.Length < 1)
				Manager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(Manager.Uri)}");

			else if (index < Initialize.Length - 1)
			{
				if (Conditions is null)
				{
					Conditions = new List<Catalog.Models.Intro>[Initialize.Length - 1];
					Link = new Dictionary<string, Catalog.Models.News[]>();
				}
				if (Conditions[index] is null || Conditions[index].Count == 0 || index == 9)
				{
					Conditions[index] = new List<Catalog.Models.Intro>();

					foreach (var code in await Http.GetFromJsonAsync<List<string>>(Crypto.Security.GetRoute(nameof(Conditions), index.ToString("D2"))))
					{
						Conditions[index].Add(await Http.GetFromJsonAsync<Catalog.Models.Intro>(Crypto.Security.GetRoute(nameof(Conditions), code)));

						if (Link.ContainsKey(code) is false)
							Link[code] = await Http.GetFromJsonAsync<Catalog.Models.News[]>(Crypto.Security.GetRoute(nameof(Catalog.Models.Classification), code));
					}
					StateHasChanged();
				}
			}
		}
		protected internal void OnClick(string name, int index, MouseEventArgs _)
		{
			if (Conditions[index] is List<Catalog.Models.Intro> model)
			{
				IsSort[name] = IsSort.ContainsKey(name) && IsSort[name] is false;
				List<Catalog.Models.Intro> list;

				switch (name)
				{
					case nameof(Catalog.Models.Intro.Code):
						list = (IsSort[name] ? model.OrderBy(o => o.Code) : model.OrderByDescending(o => o.Code)).ToList();
						break;

					case nameof(Catalog.Models.Intro.Name):
						list = (IsSort[name] ? model.OrderBy(o => o.Name) : model.OrderByDescending(o => o.Name)).ToList();
						break;

					case nameof(Catalog.Models.Intro.Price):
						list = new List<Catalog.Models.Intro>();

						if (IsSort[name])
						{
							foreach (var code in from o in Sort orderby o.Value.Item1 select o.Key)
								if (string.IsNullOrEmpty(code) is false && model.Find(o => o.Code.Equals(code)) is Catalog.Models.Intro ps)
									list.Add(ps);
						}
						else
							foreach (var code in from o in Sort orderby o.Value.Item1 descending select o.Key)
								if (string.IsNullOrEmpty(code) is false && model.Find(o => o.Code.Equals(code)) is Catalog.Models.Intro ps)
									list.Add(ps);

						break;

					default:
						list = new List<Catalog.Models.Intro>();

						if (IsSort[name])
						{
							foreach (var code in from o in Sort orderby o.Value.Item2 select o.Key)
								if (string.IsNullOrEmpty(code) is false && model.Find(o => o.Code.Equals(code)) is Catalog.Models.Intro ps)
									list.Add(ps);
						}
						else
							foreach (var code in from o in Sort orderby o.Value.Item2 descending select o.Key)
								if (string.IsNullOrEmpty(code) is false && model.Find(o => o.Code.Equals(code)) is Catalog.Models.Intro ps)
									list.Add(ps);

						break;
				}
				Conditions[index] = list;
			}
		}
		protected internal void OnClick(int index, string name, MouseEventArgs _)
		{
			if (Conditions[index] is List<Catalog.Models.Intro> model && model.Where(o => string.IsNullOrEmpty(o.Theme) is false) is IEnumerable<Catalog.Models.Intro> list)
			{
				IsSort[name] = IsSort.ContainsKey(name) && IsSort[name] is false;
				Conditions[index] = (name switch
				{
					nameof(Catalog.Models.Intro.Code) => IsSort[name] ? list.OrderBy(o => o.Code) : list.OrderByDescending(o => o.Code),
					nameof(Catalog.Models.Intro.Name) => IsSort[name] ? list.OrderBy(o => o.Name) : list.OrderByDescending(o => o.Name),
					nameof(Interface.KRX.Line.Week) => IsSort[name] ? list.OrderBy(o => o.Inclination[0]) : list.OrderByDescending(o => o.Inclination[0]),
					nameof(Interface.KRX.Line.Month) => IsSort[name] ? list.OrderBy(o => o.Inclination[1]) : list.OrderByDescending(o => o.Inclination[1]),
					nameof(Interface.KRX.Line.Quarter) => IsSort[name] ? list.OrderBy(o => o.Inclination[2]) : list.OrderByDescending(o => o.Inclination[2]),
					nameof(Interface.KRX.Line.Semiannual) => IsSort[name] ? list.OrderBy(o => o.Inclination[3]) : list.OrderByDescending(o => o.Inclination[3]),
					nameof(Interface.KRX.Line.Annual) => IsSort[name] ? list.OrderBy(o => o.Inclination[^1]) : list.OrderByDescending(o => o.Inclination[^1]),
					_ => list.OrderBy(o => Guid.NewGuid())

				}).Union(model).ToList();
			}
		}
		protected internal async void OnChange(ChangeEventArgs e)
		{
			if (e.Value is string code && code.Length == 6)
			{
				if (Link is Dictionary<string, Catalog.Models.News[]> && Link.TryGetValue(code, out Catalog.Models.News[] link))
					Title = new Stack<Catalog.Models.News>(link);

				else if (await Http.GetFromJsonAsync<Catalog.Models.News[]>(Crypto.Security.GetRoute(nameof(Catalog.Models.Classification), code)) is Catalog.Models.News[] title && title.Length > 0)
					Title = new Stack<Catalog.Models.News>(title);

			}
			if (Title is Stack<Catalog.Models.News> && Title.Count > 0)
				StateHasChanged();
		}
		protected override async Task OnInitializedAsync()
		{
			IsSort = new Dictionary<string, bool>();
			IsClick = new Dictionary<int, bool>();
			Sort = new Dictionary<string, Tuple<int, double>>();
			Stocks = new Dictionary<string, int>();
			Storage = new HashSet<Catalog.Models.Intro>();
			User = await OnReceiveLogUserInformation();
		}
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				var past = 0;
				var now = DateTime.Now;
				var sb = new StringBuilder();
				Initialize = new DateTime[0xC];

				if (User is string user && user.Length > 1)
				{
					foreach (int str in user.ToCharArray())
						sb.Append(str.ToString("D3")).Append('/');

					Hermes = new HubConnectionBuilder().WithUrl(Manager.ToAbsoluteUri("/hub/hermes")).Build();
					Hermes.On<Catalog.Models.Message>("ReceiveCurrentMessage", async (current) => await StateHasChanged(current));
					User = sb.Remove(sb.Length - 1, 1).ToString();
					Collection = await Http.GetFromJsonAsync<Catalog.Dart.Theme[]>(Crypto.Security.GetRoute(nameof(Catalog.Dart.Theme), nameof(Catalog.Models.Intro)));
					Codes = await Http.GetFromJsonAsync<Catalog.Models.Codes[]>(Crypto.Security.GetRoute("Confirm", "Stocks"));
					Inclination = Enum.GetNames(typeof(Interface.KRX.Line));
					await Hermes.StartAsync();
				}
				for (int pi = 0; pi < Initialize.Length - 1; pi++)
				{
					if (pi == 0)
					{
						Initialize[Initialize.Length - 2 - pi] = now;
						OnClick(firstRender);

						continue;
					}
					while (Base.DisplayThePage(now.AddDays(past - pi + (now.Hour < 9 ? 0 : 1))))
						past--;

					Initialize[Initialize.Length - 2 - pi] = now.AddDays(past - pi + (now.Hour < 9 ? 0 : 1));
				}
				IsClick[Initialize.Length - 1] = true;
				Initialize[^1] = DateTime.UnixEpoch;
				StateHasChanged();
			}
		}
		async Task StateHasChanged(int index, string code) => Conditions[index].Add(await Http.GetFromJsonAsync<Catalog.Models.Intro>(Crypto.Security.GetRoute(nameof(Conditions), code)));
		async Task StateHasChanged(Catalog.Models.Message message)
		{
			var now = DateTime.Now;
			var render = false;

			if (message.Key.Length == 6 && Conditions[Initialize.Length - 2].Any(o => o.Code.Equals(message.Key)) && int.TryParse(message.Convey[0] is '-' ? message.Convey[1..] : message.Convey, out int price))
			{
				Stocks[message.Key] = price;
				render = true;
			}
			else if (message.Key.Length == 1)
				switch (message.Key)
				{
					case "I":
						if (Storage.Any(o => o.Code.Equals(message.Convey)))
							Conditions[Initialize.Length - 2].Add(Storage.First(o => o.Code.Equals(message.Convey)));

						else
							await StateHasChanged(Initialize.Length - 2, message.Convey);

						render = true;
						break;

					case "D" when Conditions[Initialize.Length - 2].Find(o => o.Code.Equals(message.Convey)) is Catalog.Models.Intro remove:
						render = Conditions[Initialize.Length - 2].Remove(remove) && Storage.Add(remove) && Sort.Remove(message.Convey);
						break;
				}
			if (render && IsClick[Initialize.Length - 2] && now.CompareTo(Time) > 0)
			{
				Time = now.AddSeconds(3);
				StateHasChanged();
			}
		}
		void OnClick(bool render)
		{
			for (int i = 0; i < Initialize.Length; i++)
			{
				if (i == Initialize.Length - 2 && render is false && (Initialize[i].DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || Initialize[i].Hour is < 8 or > 0xF || Array.Exists(Base.Holidays, o => o.Equals(Initialize[i].ToString(Base.DateFormat)))))
				{
					Close = true;

					continue;
				}
				IsClick[i] = false;
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
		HubConnection Hermes
		{
			get; set;
		}
		[Inject]
		NavigationManager Manager
		{
			get; set;
		}
		HashSet<Catalog.Models.Intro> Storage
		{
			get; set;
		}
		DateTime Time
		{
			get; set;
		}
	}
}