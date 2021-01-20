using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class AccountController : ControllerBase
	{
		[AllowAnonymous, HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
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
		public async Task<HttpStatusCode> PutContextAsync([FromBody] Models.Privacy param)
		{
			try
			{
				if (await context.Privacies.AnyAsync(o => o.Security.Equals(param.Security)))
				{
					context.Entry(param).State = EntityState.Modified;
					await context.BulkSaveChangesAsync();

					return HttpStatusCode.OK;
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return HttpStatusCode.BadRequest;
		}
		public AccountController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}