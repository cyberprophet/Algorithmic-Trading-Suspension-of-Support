using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ShareInvest.Pages
{
	[Authorize]
	public partial class PortfolioBase : ComponentBase
	{
		protected override async Task OnInitializedAsync()
		{
			Enumerable = new Dictionary<int, Catalog.Models.Portfolio>();
			var temp = await Http.GetFromJsonAsync<string[]>(Crypto.Security.GetRoute("Portfolio"));
		}
		protected internal void OnClick(int sender, MouseEventArgs _)
		{
			if (Enumerable.TryGetValue(sender, out Catalog.Models.Portfolio portfolio))
				portfolio.IsClickedAmend = portfolio.IsClickedAmend is false;
		}
		protected internal void OnClick(int sender, char initial, MouseEventArgs _)
		{
			if (Enumerable.TryGetValue(sender, out Catalog.Models.Portfolio portfolio))
			{
				switch (initial)
				{
					case 'A':
					case 'D':
						if (portfolio.Temp is char.MinValue)
							portfolio.Temp = portfolio.SelectStrategics;

						break;

					case 'S':
						if (portfolio.RenderingStrategics)
						{
							if (portfolio.Temp > char.MinValue)
								portfolio.SelectStrategics = portfolio.Temp;

							portfolio.IsClickedAmend = portfolio.RenderingStrategics is false;
						}
						portfolio.RenderingStrategics = portfolio.RenderingStrategics is false;
						return;

					default:
						return;
				}
				portfolio.SelectStrategics = initial;
			}
		}
		protected internal Dictionary<int, Catalog.Models.Portfolio> Enumerable
		{
			get; private set;
		}
		protected internal static string ConvertFormat(char initial, string param)
		{
			var unit = string.Empty;

			switch (initial)
			{
				case 'D':
					unit = "일";
					break;

				case 'M':
					unit = "㎳";
					break;

				case 'S':
					unit = "주";
					break;

				case 'T':
					unit = "호가";
					break;

				case '%':
					break;
			}
			return string.Concat(param, string.IsNullOrEmpty(unit) ? initial : unit);
		}
		[Inject]
		HttpClient Http
		{
			get; set;
		}
	}
}