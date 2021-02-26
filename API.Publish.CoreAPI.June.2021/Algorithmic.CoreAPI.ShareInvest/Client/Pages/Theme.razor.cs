using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Pages
{
	[Authorize]
	public class ThemeBase : ComponentBase
	{
		protected internal string FindByName(string code) => Array.Exists(Codes, o => o.Code.Equals(code)) ? Array.Find(Codes, o => o.Code.Equals(code)).Name : string.Empty;
		protected internal Tuple<string, string> ChangeFormat(string[] param)
		{
			if (param.Length == 2)
			{
				var temp = new string[param.Length];

				if (Array.Exists(Codes, o => o.Code.Equals(param[0])))
					temp[0] = Array.Find(Codes, o => o.Code.Equals(param[0])).Name;

				if (Array.Exists(Codes, o => o.Code.Equals(param[^1])))
					temp[1] = Array.Find(Codes, o => o.Code.Equals(param[^1])).Name;

				return new Tuple<string, string>(temp[0] is string ? temp[0] : string.Empty, temp[^1] is string ? temp[^1] : string.Empty);
			}
			return new Tuple<string, string>(string.Empty, string.Empty);
		}
		protected internal async void OnClick(object sender, MouseEventArgs _)
		{
			switch (sender)
			{
				case string str:
					IsClick[str] = IsClick[str] is false;

					if (IsClick[str])
					{
						if (Array.Exists(Group, o => o.Index.Equals(str) && string.IsNullOrEmpty(o.Date)))
							foreach (var ap in await Http.GetFromJsonAsync<GroupDetail[]>(Crypto.Security.GetRoute(nameof(Catalog.Dart.Theme), str)))
								if (Array.FindIndex(Group, o => o.Code.Equals(ap.Code)) is int search)
									Group[search] = new GroupDetail
									{
										Code = ap.Code,
										Compare = ap.Compare,
										Current = ap.Current,
										Date = ap.Date,
										Inclination = ap.Inclination,
										Index = Group[search].Index,
										Percent = ap.Percent,
										Rate = ap.Rate,
										Tick = ap.Tick,
										Title = Group[search].Title
									};
						Detail = from o in Group where o.Index.Equals(str) select o;

						foreach (var kv in IsClick)
							IsClick[kv.Key] = kv.Key.Equals(str) && Detail.Any();
					}
					break;

				case int num:
					Theme = (sender switch
					{
						0 => Collection.OrderBy(o => o.Name),
						5 => Collection.OrderByDescending(o => o.Name),
						1 => Collection.OrderBy(o => o.Rate),
						6 => Collection.OrderByDescending(o => o.Rate),
						2 => Collection.OrderBy(o => o.Average),
						7 => Collection.OrderByDescending(o => o.Average),
						4 => Collection.OrderBy(o => o.Date),
						9 => Collection.OrderByDescending(o => o.Date),
						3 => Collection.OrderBy(o => o.Index),
						_ => Collection.OrderByDescending(o => Guid.NewGuid())

					}).ToArray();
					Check[num % Base.Contents.Length] = Check[num % Base.Contents.Length] is false;
					break;
			}
			StateHasChanged();
			await Virtualize.RefreshDataAsync();
		}
		protected internal async void OnClick(MouseEventArgs _)
		{
			await WaitForTheScrollToMovement(Array.FindIndex(Theme, o => o.Name.Equals(Search)));
			Search = string.Empty;
		}
		protected internal void OnClick(MouseEventArgs _, object sender)
		{
			switch (sender)
			{
				case int num:
					Detail = sender switch
					{
						0 => Detail.OrderBy(o => o.Code),
						6 => Detail.OrderByDescending(o => o.Code),
						2 => Detail.OrderBy(o => o.Current),
						8 => Detail.OrderByDescending(o => o.Current),
						3 => Detail.OrderBy(o => o.Rate),
						9 => Detail.OrderByDescending(o => o.Rate),
						4 => Detail.OrderBy(o => o.Compare),
						0xA => Detail.OrderByDescending(o => o.Compare),
						5 => Detail.OrderBy(o => o.Percent),
						0xB => Detail.OrderByDescending(o => o.Percent),
						_ => Detail.OrderBy(o => Guid.NewGuid())
					};
					IsCheck[num % Base.Stocks.Length] = IsCheck[num % Base.Stocks.Length] is false;
					break;
			}
			StateHasChanged();
		}
		protected internal void OnReceiveKeyPress(ChangeEventArgs e)
		{
			if (e.Value is string str && string.IsNullOrEmpty(str) is false)
				foreach (var name in new[] { str, str.ToUpper() })
				{
					if (Array.Find(Group, o => o.Code.Equals(name)) is GroupDetail detail)
						Search = Array.Find(Theme, o => o.Index.Equals(detail.Index)).Name;

					else if (Array.Find(Codes, o => o.Name.Equals(name)) is Codes co && Array.Find(Group, o => o.Code.Equals(co.Code)) is GroupDetail group)
						Search = Array.Find(Theme, o => o.Index.Equals(detail.Index)).Name;

					if (string.IsNullOrEmpty(Search) is false)
					{
						StateHasChanged();

						return;
					}
				}
		}
		protected internal Catalog.Dart.Theme[] Theme
		{
			get; private set;
		}
		protected internal IEnumerable<GroupDetail> Detail
		{
			get; private set;
		}
		protected internal Dictionary<string, bool> IsClick
		{
			get; private set;
		}
		protected internal GroupDetail[] Group
		{
			get; private set;
		}
		protected internal Codes[] Codes
		{
			get; private set;
		}
		protected internal Virtualize<Catalog.Dart.Theme> Virtualize
		{
			get; set;
		}
		protected internal string Search
		{
			get; private set;
		}
		protected internal string[] Inclination
		{
			get; private set;
		}
		protected internal bool[] Check
		{
			get; private set;
		}
		protected internal bool[] IsCheck
		{
			get; private set;
		}
		protected internal ValueTask<ItemsProviderResult<Catalog.Dart.Theme>> LoadGroup(ItemsProviderRequest request) => ValueTask.FromResult(new ItemsProviderResult<Catalog.Dart.Theme>(GetTheme(request.StartIndex, Math.Min(request.Count, Collection.Length - request.StartIndex)), Collection.Length));
		protected override async Task OnInitializedAsync()
		{
			Collection = await Http.GetFromJsonAsync<Catalog.Dart.Theme[]>(Crypto.Security.GetRoute(nameof(Catalog.Dart.Theme), nameof(Catalog.Dart.Theme)));
			Codes = await Http.GetFromJsonAsync<Codes[]>(Crypto.Security.GetRoute("Confirm", "Stocks"));
			IsClick = new Dictionary<string, bool>();
			Check = new bool[] { true, true, true, true, true };
			Theme = Collection;

			foreach (var context in Collection)
				IsClick[context.Index] = false;
		}
		protected override async Task OnAfterRenderAsync(bool render)
		{
			if (render)
			{
				Group = await Http.GetFromJsonAsync<GroupDetail[]>(Crypto.Security.GetRoute(nameof(Catalog.Dart.Theme), nameof(GroupDetail.Title)));
				Inclination = Enum.GetNames(typeof(Interface.KRX.Line));
				IsCheck = new bool[] { true, true, true, true, true, true };
			}
		}
		async Task WaitForTheScrollToMovement(int index)
		{
			try
			{
				await Runtime.InvokeVoidAsync(string.Concat(interop, "move"), 0x200);
				await Task.Delay(0x700);
				await Runtime.InvokeVoidAsync(string.Concat(interop, "selector"), index);
			}
			catch (Exception)
			{
				await WaitForTheScrollToMovement(index);
			}
		}
		List<Catalog.Dart.Theme> GetTheme(int initial, int quantity)
		{
			var result = new List<Catalog.Dart.Theme>();

			for (int i = initial; i < initial + quantity; i++)
				result.Add(Theme[i]);

			return result;
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
		Catalog.Dart.Theme[] Collection
		{
			get; set;
		}
		const string interop = "JsFunctions.";
	}
}