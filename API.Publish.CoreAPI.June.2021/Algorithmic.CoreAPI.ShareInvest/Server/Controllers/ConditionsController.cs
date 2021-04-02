using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

			}
			else if (int.TryParse(key, out int index))
				return Security.Conditions[index] is HashSet<string> ? Security.Conditions[index].ToList() : new List<string>();

			return null;
		}
		public ConditionsController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}