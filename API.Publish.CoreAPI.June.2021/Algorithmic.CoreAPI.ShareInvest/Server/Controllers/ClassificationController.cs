using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class ClassificationController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<Catalog.Models.News> GetContext(string key)
		{
			if (context.Classifications.AsNoTracking().Any(o => o.Code.Equals(key)))
			{
				var stack = new Stack<Catalog.Models.News>();

				foreach (var cf in context.Classifications.AsNoTracking().Where(o => o.Code.Equals(key)).Select(o => new { o.Index, o.Title }))
					stack.Push(new Catalog.Models.News
					{
						Link = cf.Index,
						Title = string.Concat(context.Theme.AsNoTracking().Single(o => o.Index.Equals(cf.Index)).Name, '\n', cf.Title.Replace(". ", ".\n"))
					});
				return stack.ToArray();
			}
			return Array.Empty<Catalog.Models.News>();
		}
		public ClassificationController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}