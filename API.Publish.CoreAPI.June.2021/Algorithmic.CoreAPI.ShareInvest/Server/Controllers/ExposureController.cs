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
		public Catalog.Models.Exposure GetContext(string key) => key.Length is 6 or 8 && context.Group.AsNoTracking().Any(o => o.Code.Equals(key)) ? new Catalog.Models.Exposure
		{
			Ratio = (context.Securities.AsNoTracking().Count(o => o.Code.Equals(key)) / (double)context.Securities.AsNoTracking().Count()).ToString("P2"),
			Theme = context.Theme.Find(context.Group.AsNoTracking().Single(o => o.Code.Equals(key)).Index).Name

		} : new Catalog.Models.Exposure
		{
			Theme = string.Empty,
			Ratio = (context.Securities.AsNoTracking().Count(o => o.Code.Equals(key)) / (double)context.Securities.AsNoTracking().Count()).ToString("P2")
		};
		public ExposureController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}