using System;
using System.Collections.Generic;
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
					if (Accounts is null)
						Accounts = new List<string>();

					else
						Accounts.Clear();

					foreach (var str in param.Number)
						if (string.IsNullOrEmpty(str) == false && str.Length == 0xA && str[^2..].CompareTo("32") < 0)
							Accounts.Add(str);
				}
			}
			catch (Exception ex)
			{
				await new Task(() => Base.SendMessage(ex.StackTrace, GetType()));
			}
			return Ok();
		}
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PutContextAsync([FromBody] Catalog.Models.Privacies param)
		{
			try
			{
				if (string.IsNullOrEmpty(Progress.Account))
					return Ok(await Progress.Client.PostContextAsync(new Catalog.Models.Privacies
					{
						Account = param.Account
					}));
				else
					return Ok(await Progress.Client.PutContextAsync(new Catalog.Models.Privacies
					{
						Account = param.Account
					}));
			}
			catch (Exception ex)
			{
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return BadRequest();
		}
		[HttpGet]
		public IEnumerable<string> GetContext()
		{
			if (Accounts is null)
				Accounts = new List<string>();

			if (Accounts.FindIndex(o => o.Equals(separate)) is int count && count > -1)
				Accounts.RemoveRange(count, Accounts.Count - count);

			if (string.IsNullOrEmpty(Progress.Account) == false)
			{
				Accounts.Add(separate);

				foreach (var str in Progress.Account.Split(';'))
					if (string.IsNullOrEmpty(str) is false && str.Length == 0xA)
						Accounts.Add(str);
			}
			return Accounts.ToArray();
		}
		static List<string> Accounts
		{
			get; set;
		}
		const string separate = "Separation";
	}
}