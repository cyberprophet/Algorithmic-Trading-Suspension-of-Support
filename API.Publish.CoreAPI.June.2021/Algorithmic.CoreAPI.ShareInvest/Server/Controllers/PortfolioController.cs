using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class PortfolioController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<string> GetContextAsync()
		{
			return Array.Empty<string>();
		}
		public PortfolioController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}