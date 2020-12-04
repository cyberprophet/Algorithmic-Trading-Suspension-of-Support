using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class BalanceController : ControllerBase
	{
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PutContextAsync([FromBody] Catalog.OpenAPI.Balance balance)
		{
			try
			{
				if (Progress.Collection.TryGetValue(balance.Code[0] is 'A' ? balance.Code[1..] : balance.Code, out Analysis analysis))
				{
					if (analysis.Balance is null)
						analysis.Balance = new Balance(balance.Name);

					await analysis.OnReceiveBalance(balance);
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return Ok();
		}
	}
}