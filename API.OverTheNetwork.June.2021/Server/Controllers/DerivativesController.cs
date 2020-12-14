using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class DerivativesController : ControllerBase
	{
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PutContextAsync([FromBody] Dictionary<string, string> param)
		{
			try
			{
				var derivatives = JsonConvert.DeserializeObject<Catalog.OpenAPI.Derivatives>(JsonConvert.SerializeObject(param));

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