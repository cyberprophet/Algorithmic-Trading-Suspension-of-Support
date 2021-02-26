using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class ExposureController : ControllerBase
	{
		[HttpGet]
		public string GetContext(string key) => key.Length is 6 or 8 ? (context.Securities.AsNoTracking().Count(o => o.Code.Equals(key)) / (double)context.Securities.AsNoTracking().Count()).ToString("P5") : 0.ToString("P");
		public ExposureController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}