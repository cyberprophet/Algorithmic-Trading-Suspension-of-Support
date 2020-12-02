using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class MessageController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public IActionResult PostContextAsync([FromBody] Message param)
		{
			Base.SendMessage(DateTime.Now, param.Convey, GetType());
			
			return Ok();
		}
	}
}