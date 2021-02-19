using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class SatisfyConditionsController : ControllerBase
	{
		[HttpGet(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContext(string key)
		{
			try
			{
				if (await context.Conditions.AnyAsync(o => o.Security.Equals(key)))
					return Ok(await context.Conditions.FindAsync(key));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContext)}");
			}
			return BadRequest();
		}
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContext([FromBody] SatisfyConditions conditions)
		{
			try
			{
				if (await context.Privacies.AnyAsync(o => o.Security.Equals(conditions.Security)))
				{
					if (await context.Conditions.AnyAsync(o => o.Security.Equals(conditions.Security)))
						context.Entry(conditions).State = EntityState.Modified;

					else
						context.Conditions.Add(conditions);

					await context.BulkSaveChangesAsync();

					return Ok();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.PostContext)}");
			}
			return BadRequest();
		}
		public SatisfyConditionsController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}