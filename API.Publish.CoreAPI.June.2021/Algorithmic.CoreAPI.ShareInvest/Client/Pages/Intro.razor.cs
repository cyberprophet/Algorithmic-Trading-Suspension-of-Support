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
		protected internal List<Catalog.Models.Intro>[] Conditions
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
				Navigation.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(Navigation.Uri)}");

			else if (index < Initialize.Length - 1)
			{
				if (Conditions is null)
					Conditions = new List<Catalog.Models.Intro>[Initialize.Length - 1];

				if (Conditions[index] is null || Conditions[index].Count == 0)
				{
					Conditions[index] = new List<Catalog.Models.Intro>();

					foreach (var code in await Http.GetFromJsonAsync<List<string>>(Crypto.Security.GetRoute(nameof(Conditions), index.ToString("D2"))))
						Conditions[index].Add(await Http.GetFromJsonAsync<Catalog.Models.Intro>(Crypto.Security.GetRoute(nameof(Conditions), code)));

					StateHasChanged();
				}
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
		protected override async Task OnInitializedAsync()
		{
			IsSort = new Dictionary<string, bool>();
			IsClick = new Dictionary<int, bool>();
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

					User = sb.Remove(sb.Length - 1, 1).ToString();
					Collection = await Http.GetFromJsonAsync<Catalog.Dart.Theme[]>(Crypto.Security.GetRoute(nameof(Catalog.Dart.Theme), nameof(Intro)));
					Inclination = Enum.GetNames(typeof(Interface.KRX.Line));
				}
				for (int pi = 0; pi < Initialize.Length - 1; pi++)
				{
					if (pi == 0)
					{
						Initialize[Initialize.Length - 2 - pi] = now;
						OnClick(firstRender);

						continue;
					}
					while (Base.DisplayThePage(now.AddDays(past - pi)))
						past--;

					Initialize[Initialize.Length - 2 - pi] = now.AddDays(past - pi);
				}
				IsClick[Initialize.Length - 1] = true;
				Initialize[^1] = DateTime.UnixEpoch;
				StateHasChanged();
			}
		}
		void OnClick(bool render)
		{
			for (int i = 0; i < Initialize.Length; i++)
			{
				if (i == Initialize.Length - 2 && render is false)
				{
					var now = DateTime.Now;

					switch (Initialize[i].DayOfWeek)
					{
						case DayOfWeek.Saturday or DayOfWeek.Sunday:
						case DayOfWeek.Monday when now.Hour < 8:
						case DayOfWeek.Friday when now.Hour > 0xF:
							Close = true;
							continue;

						default:
							if (Array.Exists(Base.Holidays, o => o.Equals(Initialize[i].ToString(Base.DateFormat)) || o.Equals(Initialize[i].AddHours(-8).ToString(Base.DateFormat))))
							{
								Close = true;

								continue;
							}
							break;
					}
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
		[Inject]
		protected internal NavigationManager Navigation
		{
			get; set;
		}
	}
}