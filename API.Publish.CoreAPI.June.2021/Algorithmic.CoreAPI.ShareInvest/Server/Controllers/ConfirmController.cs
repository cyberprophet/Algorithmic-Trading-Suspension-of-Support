using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class ConfirmController : ControllerBase
	{
		[HttpPost]
		public IEnumerable<UserInformation> PostContext([FromBody] Confirm confirm)
		{
			if (Security.User.TryGetValue(confirm.Key, out User user) && confirm.First == user.Account.Name[0] && user.Account.Name[^1] == confirm.Last && context.User.Any(o => o.Kiwoom.Equals(confirm.Key) && o.Email.Equals(confirm.Email)) is false)
			{
				var sb = new StringBuilder();

				foreach (var str in user.Account.Number)
					sb.Append(str).Append(';');

				context.User.Add(new Models.Connection
				{
					Email = confirm.Email,
					Kiwoom = confirm.Key,
					Account = Crypto.Security.Encrypt(confirm.Email, sb.Remove(sb.Length - 1, 1))
				});
				if (context.SaveChanges() > 0)
				{
					var queue = new Queue<UserInformation>();

					foreach (var renewal in from o in context.User where o.Email.Equals(confirm.Email) select o)
					{
						var check = string.Empty;

						if (context.Privacies.Any(o => o.CodeStrategics.Equals(renewal.Kiwoom)))
						{
							var tick = double.NaN;

							foreach (var privacy in from o in context.Privacies where o.CodeStrategics.Equals(renewal.Kiwoom) select o)
								if (double.IsNaN(tick) || privacy.Commission > tick)
								{
									tick = privacy.Commission;
									check = Crypto.Security.Decipher(privacy.Security, privacy.SecuritiesAPI, privacy.SecurityAPI);
								}
						}
						queue.Enqueue(new UserInformation
						{
							Account = Crypto.Security.Decipher(renewal.Email, renewal.Account).Split(';'),
							Key = renewal.Kiwoom,
							Remaining = renewal.Payment > 0 ? new DateTime(renewal.Payment).AddDays(renewal.Coupon) : DateTime.Now,
							Check = check,
							Name = Security.User.TryGetValue(renewal.Kiwoom, out User su) ? su.Account.Name : string.Empty
						});
					}
					if (queue.Count > 0)
						return queue.ToArray();
				}
			}
			return Array.Empty<UserInformation>();
		}
		[HttpGet]
		public IEnumerable<Codes> GetContext(string key)
		{
			var stack = new Stack<Codes>();

			foreach (var co in from o in context.Codes where o.Code.Length == key.Length select o)
				if (string.IsNullOrEmpty(co.Price) is false && co.MaturityMarketCap.StartsWith(Base.Margin) && co.MaturityMarketCap.Contains(Base.TransactionSuspension) is false && (co.MarginRate == 1 || co.MarginRate == 2) && int.TryParse(co.Price, out int price) && price > 0)
					stack.Push(new Codes
					{
						Code = co.Code,
						Name = co.Name,
						Price = co.Price,
						MaturityMarketCap = co.MaturityMarketCap,
						MarginRate = co.MarginRate
					});
			return stack.Count > 0 ? stack.ToArray() : Array.Empty<Codes>();
		}
		public ConfirmController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}