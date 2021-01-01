using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class AccountController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.Account param)
		{
			try
			{
				if (param.Length > 0)
				{
					var sb = new StringBuilder();

					foreach (var str in param.Number)
						if (string.IsNullOrEmpty(str) is false && str.Length == 0xA && str[^2..].CompareTo("32") < 0)
							sb.Append(str).Append(';');

					Security.Account[Crypto.Security.Decipher(param)] = sb.Remove(sb.Length - 1, 1).ToString().Split(';');
				}
			}
			catch (Exception ex)
			{
				await new Task(() => Base.SendMessage(ex.StackTrace, GetType()));
			}
			return Ok();
		}
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<HttpStatusCode> PutContextAsync([FromBody] Catalog.Models.Privacies param)
		{
			try
			{
				if (string.IsNullOrEmpty(param.Account) is false)
				{
					var privacy = new Catalog.Models.Privacies
					{

					};
					var response = string.IsNullOrEmpty(param.Account) ? await Security.API.PostContextAsync(privacy) : await Security.API.PutContextAsync(privacy);

					return (HttpStatusCode)response;
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return HttpStatusCode.BadRequest;
		}
		[HttpGet]
		public IEnumerable<string> GetContext()
		{
			return null;
		}
	}
}