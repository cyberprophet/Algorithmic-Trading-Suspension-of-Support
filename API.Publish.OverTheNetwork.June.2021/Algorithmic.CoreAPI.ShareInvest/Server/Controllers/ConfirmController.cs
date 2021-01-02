using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class ConfirmController : ControllerBase
	{
		[HttpPost]
		public Account PostContext([FromBody] Confirm confirm)
		{
			if (Security.Account.TryGetValue(confirm.Identity, out Account account))
				return account;

			return new Account
			{
				Number = new[] { "9151345117", "9151345114", "9151345123", "9151345122", "9151345131" },
				Identity = "Test",
				Security = "Test",
				Name = "Test",
				Length = 5
			};
		}
	}
}