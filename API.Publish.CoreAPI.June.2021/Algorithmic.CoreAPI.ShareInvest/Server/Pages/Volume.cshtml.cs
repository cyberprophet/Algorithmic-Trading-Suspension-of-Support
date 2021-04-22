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
					Normalize = new SecondaryIndicators.Normalization(dic.Max(o => o.Value), dic.Min(o => o.Value));
					Enumerable = dic.OrderByDescending(o => o.Key);

					return Page();
				}
				else
					return BadRequest();
			}
			return Unauthorized();
		}
		public VolumeModel(CoreApiDbContext context) => this.context = context;
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