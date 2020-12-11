using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class DerivativesController : ControllerBase
	{
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PutContextAsync([FromBody] Catalog.OpenAPI.Derivatives derivatives)
		{
			try
			{
				if (Progress.Collection.TryGetValue(derivatives.Code, out Analysis analysis))
				{
					if (analysis.Balance is null)
						analysis.Balance = new Balance
						{
							Market = derivatives.Code.Length == 8 && derivatives.Code[1] is '0',
							Name = derivatives.Name.Split(' ')[0].Trim()
						};
					await new Task(() => analysis.OnReceiveBalance(derivatives));
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