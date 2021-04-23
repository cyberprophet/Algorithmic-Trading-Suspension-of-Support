using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), IgnoreAntiforgeryToken]
	public class VolumeModel : PageModel
	{
		public async Task<IActionResult> OnGetAsync()
		{
			var code = Request.Query["code"];

			if (await context.Codes.AsNoTracking().AnyAsync(o => o.Code.Equals(code)) && DateTime.TryParseExact(Request.Query["end"].ToString()[2..].Replace("-", string.Empty), Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime end) && DateTime.TryParseExact(Request.Query["start"].ToString()[2..].Replace("-", string.Empty), Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
			{
				if (date.CompareTo(end.AddDays(1)) < 0)
				{
					if (context.RevisedStockPrices.AsNoTracking().Where(o => o.Code.Equals(code)).Select(o => o.Date).ToArray() is string[] models && models.Length > 0 && DateTime.TryParseExact(models.Max(), Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime max))
					{
						if (date.CompareTo(max) <= 0)
							date = max.AddDays(1);

						if (end.CompareTo(max) <= 0)
							end = max.AddDays(1);
					}
					if (context.Codes.Find(code) is Models.Codes model)
					{
						Title = model.Name;
						Start = date.ToLongDateString();
						End = end.Equals(date) ? string.Empty : end.ToLongDateString();
					}
					if (context.Group.Find(code) is Models.Group group)
					{
						Name = context.Theme.Find(group.Index).Name;
						Summary = group.Title.Replace(". ", ".\n");
					}
					var dic = new Dictionary<int, long>();

					while (date.CompareTo(end.AddDays(1)) < 0)
					{
						if (Base.DisplayThePage(date) is false)
							foreach (var cs in from o in context.Stocks.AsNoTracking() where o.Code.Equals(code) && o.Date.StartsWith(date.ToString(Base.DateFormat)) select new { o.Price, o.Volume })
								if (int.TryParse(cs.Price, out int price))
								{
									if (dic.TryGetValue(price, out long volume))
										dic[price] = volume + cs.Volume;

									else
										dic[price] = cs.Volume;

									Sum += cs.Volume;
								}
						date = date.AddDays(1);
					}
					var cal = SecondaryIndicators.Bollinger.Calculate(dic);
					Mean = cal.Item1;
					Sigma = cal.Item2;
					Normalize = new SecondaryIndicators.Normalization(dic.Max(o => o.Value), dic.Min(o => o.Value));

					if (dic.OrderBy(o => o.Key) is IEnumerable<KeyValuePair<int, long>> enumerable)
					{
						double[] my = new double[dic.Count], mx = new double[dic.Count];
						var nor = new SecondaryIndicators.Normalization(enumerable.Last().Key, enumerable.First().Key);
						var index = 0;

						foreach (var kv in enumerable)
						{
							mx[index] = nor.Normalize((long)kv.Key);
							my[index++] = Normalize.Normalize(kv.Value);
						}
						Slope = new SecondaryIndicators.LinearRegressionLine(mx, my).Slope;
					}
					Enumerable = dic.OrderByDescending(o => o.Key);

					return Page();
				}
				else
					return BadRequest();
			}
			return Unauthorized();
		}
		public VolumeModel(CoreApiDbContext context) => this.context = context;
		public double Slope
		{
			get; private set;
		}
		public double Mean
		{
			get; private set;
		}
		public double Sigma
		{
			get; private set;
		}
		public double Sum
		{
			get; private set;
		}
		public double Past
		{
			get; set;
		}
		public string Name
		{
			get; private set;
		}
		public string Summary
		{
			get; private set;
		}
		public string Title
		{
			get; private set;
		}
		public string Start
		{
			get; private set;
		}
		public string End
		{
			get; private set;
		}
		public SecondaryIndicators.Normalization Normalize
		{
			get; private set;
		}
		public IEnumerable<KeyValuePair<int, long>> Enumerable
		{
			get; private set;
		}
		readonly CoreApiDbContext context;
	}
}