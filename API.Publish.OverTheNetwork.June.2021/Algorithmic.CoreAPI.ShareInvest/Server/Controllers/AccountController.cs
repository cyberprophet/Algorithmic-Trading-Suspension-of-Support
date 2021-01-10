using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;
using ShareInvest.Filter;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class AccountController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Account param)
		{
			try
			{
				if (param.Length > 0 && Security.User.ContainsKey(param.Identity) is false)
				{
					var temp = new string[param.Length];

					for (int i = 0; i < param.Length; i++)
						temp[i] = Base.IsDebug ? param.Number[i] : Crypto.Security.Decipher(param.Number[i]);

					Security.User[param.Identity] = new User
					{
						Account = new Account
						{
							Length = param.Length,
							Identity = param.Identity,
							Name = param.Name,
							Security = param.Security,
							Number = temp
						},
						Logs = new Queue<Log>()
					};
					return Ok(temp[^1]);
				}
			}
			catch (Exception ex)
			{
				await new Task(() => Base.SendMessage(ex.StackTrace, GetType()));
			}
			return Ok();
		}
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<HttpStatusCode> PutContextAsync([FromBody] Privacies param)
		{
			try
			{
				if (string.IsNullOrEmpty(param.Account) is false)
				{
					var privacy = new Privacies
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
		[HttpGet, ServiceFilter(typeof(ClientIpCheckActionFilter))]
		public IEnumerable<string> GetContext()
		{
			return null;
		}
	}
}