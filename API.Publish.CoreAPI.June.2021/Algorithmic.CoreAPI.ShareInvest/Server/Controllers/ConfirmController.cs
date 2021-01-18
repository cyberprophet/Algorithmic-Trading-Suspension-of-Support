using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class ConfirmController : ControllerBase
	{
		[HttpPost]
		public Account PostContext([FromBody] Confirm confirm)
		{
			if (Security.User.TryGetValue(confirm.Identity, out User user))
				return user.Account;

			return new Account
			{
				Number = new[] { "TestTest17", "TestTest14", "TestTest23", "TestTest22", "TestTest31" },
				Identity = "Test",
				Security = "Test",
				Name = "Test",
				Length = 5
			};
		}
	}
}