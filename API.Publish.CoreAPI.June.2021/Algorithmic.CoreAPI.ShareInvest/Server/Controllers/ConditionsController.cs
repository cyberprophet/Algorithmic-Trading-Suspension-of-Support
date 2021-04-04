using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class ConditionsController : ControllerBase
	{
		[HttpGet]
		public object GetContext(string key)
		{
			if (key.Length == 6)
			{
				var name = context.Codes.AsNoTracking().SingleOrDefault(o => o.Code.Equals(key))?.Name;

				if (context.Group.AsNoTracking().Any(o => o.Code.Equals(key)) && context.Group.AsNoTracking().Single(o => o.Code.Equals(key)) is Models.Group group)
					return new Catalog.Models.Intro
					{
						Code = group.Code,
						Name = name,
						Title = group.Title,
						Index = group.Index,
						Theme = context.Theme.AsNoTracking().FirstOrDefault(o => o.Index.Equals(group.Index)).Name,
						Inclination = context.Tendencies.AsNoTracking().Where(o => o.Code.Equals(key)).OrderBy(o => o.Tick).Select(o => o.Inclination).ToArray()
					};
				else
					return new Catalog.Models.Intro
					{
						Code = key,
						Name = name,
						Title = string.Empty,
						Theme = string.Empty,
						Index = string.Empty
					};
			}
			else if (key.Length == 2 && int.TryParse(key, out int index))
				return Security.Conditions[index] is HashSet<string> ? Security.Conditions[index].ToList() : new List<string>();

			return null;
		}
		public ConditionsController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}