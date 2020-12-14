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
		public async Task<IActionResult> PutContextAsync([FromBody] Dictionary<string, string> param)
		{
			try
			{
				var conclusion = JsonConvert.DeserializeObject<Catalog.OpenAPI.Conclusion>(JsonConvert.SerializeObject(param));

				if (Progress.Collection.TryGetValue(conclusion.Code[0] is 'A' ? conclusion.Code[1..] : conclusion.Code, out Analysis analysis))
				{
					if (analysis.OrderNumber is null)
						analysis.OrderNumber = new Dictionary<string, dynamic>();

					analysis.OnReceiveConclusion(conclusion);
				}
			}
			catch (Exception ex)
			{
				await new Task(() => Base.SendMessage(GetType(), ex.StackTrace));
			}
			return Ok();
		}
	}
}