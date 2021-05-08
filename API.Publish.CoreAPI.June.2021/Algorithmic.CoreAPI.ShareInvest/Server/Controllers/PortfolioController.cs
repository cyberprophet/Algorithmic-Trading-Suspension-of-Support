using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class PortfolioController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<Catalog.Models.BringIn> GetContextAsync(string key)
		{
			if (context.User.AsNoTracking().Any(o => o.Email.Equals(key)))
			{
				var stack = new Stack<Catalog.Models.BringIn>();

				foreach (var find in from o in context.User.AsNoTracking() where o.Email.Equals(key) select o)
					foreach (var sk in from o in context.Securities.AsNoTracking() where o.Methods.Equals(find.Kiwoom) select o)
						stack.Push(new Catalog.Models.BringIn
						{
							Code = sk.Code,
							Contents = sk.Contents,
							Methods = sk.Methods,
							Strategics = sk.Strategics,
							Theme = string.Empty,
							Date = sk.Date
						});
				if (stack.Count > 0)
					return stack.ToArray();
			}
			return Array.Empty<Catalog.Models.BringIn>();
		}
		[HttpPost]
		public async Task<HttpStatusCode> PostContextAsync([FromBody] Catalog.Models.BringIn bring)
		{
			if (await context.Privacies.AsNoTracking().AnyAsync(o => o.CodeStrategics.Equals(bring.Methods)))
			{
				var identity = new Models.Identify
				{
					Code = bring.Code,
					Security = context.Privacies.First(o => o.CodeStrategics.Equals(bring.Methods)).Security,
					Methods = bring.Methods,
					Contents = bring.Contents,
					Strategics = bring.Strategics,
					Date = DateTime.Now.Ticks
				};
				if (context.Securities.Any(o => o.Code.Equals(bring.Code) && o.Security.Equals(identity.Security)))
					context.Entry(identity).State = EntityState.Modified;

				else
					context.Securities.Add(identity);

				if (context.SaveChanges() > 0)
					return HttpStatusCode.OK;
			}
			return HttpStatusCode.BadRequest;
		}
		[HttpDelete]
		public HttpStatusCode DeleteContext(string key)
		{
			if (string.IsNullOrEmpty(key) is false && key.Split(';') is string[] arr)
			{
				context.Securities.RemoveRange(from o in context.Securities where o.Methods.Equals(arr[0]) && o.Code.Equals(arr[1]) select o);

				if (context.SaveChanges() > 0)
					return HttpStatusCode.OK;
			}
			return HttpStatusCode.BadRequest;
		}
		public PortfolioController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}