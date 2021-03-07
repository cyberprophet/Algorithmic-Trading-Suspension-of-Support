using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class ThemeController : ControllerBase
	{
		[HttpGet]
		public object GetContext(string key)
		{
			switch (key)
			{
				case string when Array.TrueForAll(key.ToCharArray(), o => char.IsDigit(o)):
					var detail = new Stack<Catalog.Models.GroupDetail>();

					foreach (var search in from o in context.Group.AsNoTracking() where o.Index.Equals(key) select o.Code)
						if (context.Details.Find(search) is Models.GroupDetail gd)
						{
							var tick = new int[5];
							var inclination = new double[5];
							var index = 0;

							foreach (var td in from o in context.Tendencies.AsNoTracking() where o.Code.Equals(search) select new { o.Tick, o.Inclination })
							{
								tick[index] = td.Tick;
								inclination[index++] = td.Inclination;
							}
							detail.Push(new Catalog.Models.GroupDetail
							{
								Code = search,
								Compare = gd.Compare,
								Current = gd.Current,
								Date = gd.Date,
								Rate = gd.Rate,
								Percent = gd.Percent,
								Tick = tick,
								Inclination = inclination,
								Page = context.Page.Find(search).Tistory
							});
						}
					return detail.ToArray();

				case nameof(Catalog.Models.GroupDetail.Title):
					var group = new Stack<Catalog.Models.GroupDetail>();

					foreach (var title in context.Group.AsNoTracking())
						group.Push(new Catalog.Models.GroupDetail
						{
							Code = title.Code,
							Title = title.Title,
							Index = title.Index
						});
					return group.ToArray();

				case nameof(Catalog.Dart.Theme):
					var theme = new Stack<Catalog.Dart.Theme>();

					foreach (var th in context.Theme.AsNoTracking())
						theme.Push(new Catalog.Dart.Theme
						{
							Index = th.Index,
							Name = th.Name,
							Rate = th.Rate,
							Average = th.Average,
							Code = th.Code,
							Date = th.Date
						});
					return theme.ToArray();
			}
			return null;
		}
		public ThemeController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}