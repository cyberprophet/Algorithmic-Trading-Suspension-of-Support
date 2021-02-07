using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class ExposureController : ControllerBase
	{
		[HttpGet]
		public string GetContext(string key) => key.Length == 6 || key.Length == 8 ? (context.Securities.Count(o => o.Code.Equals(key)) / (double)context.Securities.Count()).ToString("P5") : 0.ToString("P");
		public ExposureController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}