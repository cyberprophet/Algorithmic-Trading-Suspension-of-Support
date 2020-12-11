using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class ConclusionController : ControllerBase
	{
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PutContextAsync([FromBody] Catalog.OpenAPI.Conclusion conclusion)
		{
			try
			{
				if (Progress.Collection.TryGetValue(conclusion.Code[0] is 'A' ? conclusion.Code[1..] : conclusion.Code, out Analysis analysis))
				{
					if (analysis.OrderNumber is null)
						analysis.OrderNumber = new Dictionary<string, dynamic>();

					await new Task(() => analysis.OnReceiveConclusion(conclusion));
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace, conclusion);
				Base.SendMessage(JsonConvert.SerializeObject(conclusion), conclusion.GetType());
			}
			return Ok();
		}
	}
}