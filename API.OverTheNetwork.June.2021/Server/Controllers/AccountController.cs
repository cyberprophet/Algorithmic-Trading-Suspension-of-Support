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
		public async Task<HttpStatusCode> PutContextAsync([FromBody] Catalog.Models.Privacies param)
		{
			try
			{
				var encrypt = Crypto.Security.Encrypt(Progress.Key, param.Account, string.IsNullOrEmpty(param.Account) is false);

				if (string.IsNullOrEmpty(encrypt) is false)
				{
					var privacy = new Catalog.Models.Privacies
					{
						Security = Progress.Key.Security,
						SecuritiesAPI = Progress.Key.SecuritiesAPI,
						SecurityAPI = encrypt,
						Account = param.Account.Contains(";") ? "B" : Progress.Key.Account,
						Commission = Progress.Key.Commission,
						CodeStrategics = Progress.Key.CodeStrategics,
						Coin = Progress.Key.Coin
					};
					var response = string.IsNullOrEmpty(Progress.Account) ? await Progress.Client.PostContextAsync(privacy) : await Progress.Client.PutContextAsync(privacy);
					Progress.Account = param.Account;

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