using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class PrivaciesController : ControllerBase
	{
		[HttpGet(Security.security), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetContext(string key, string security)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				var context = this.context.Privacies.Find(security);

				if (context is null)
					return NotFound();

				return Ok(context);
			}
			return BadRequest();
		}
		[HttpGet(Security.routeKey), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContext(string key)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				var dictionary = new Dictionary<string, int>();

				foreach (var pv in context.Privacies.Where(o => string.IsNullOrEmpty(o.CodeStrategics) == false).AsNoTracking().Select(o => new { o.CodeStrategics }))
					foreach (var strategics in pv.CodeStrategics.Split(';'))
						if (strategics.Contains('|'))
						{
							var code = strategics.Split('|')[1];

							if (dictionary.TryGetValue(code, out int count))
								dictionary[code] = count + 1;

							else
								dictionary[code] = 1;
						}
				return Ok(dictionary);
			}
			return BadRequest();
		}
		[HttpGet, ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> GetContext() => Ok(await context.Privacies.LongCountAsync());
		[HttpPost(Security.routeKey), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status202Accepted), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContext(string key, [FromBody] Privacy privacy)
		{
			if (privacy.Security.Equals(Security.GetGrantAccess(key)))
			{
				if (await context.Privacies.AnyAsync(o => o.Security.Equals(privacy.Security)))
					return Accepted((await context.Privacies.FirstAsync(o => o.Security.Equals(privacy.Security))).Coin);

				context.Privacies.Add(privacy);
				await context.BulkSaveChangesAsync();

				return Ok((await context.Privacies.FirstAsync(o => o.Security.Equals(privacy.Security))).Coin);
			}
			return BadRequest();
		}
		[HttpPut(Security.security), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PutContext(string key, string security, [FromBody] Privacy privacy)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				context.Entry(privacy).State = EntityState.Modified;
				await context.BulkSaveChangesAsync();

				return Ok((await context.Privacies.FirstAsync(o => o.Security.Equals(security))).Coin);
			}
			return BadRequest();
		}
		[HttpDelete(Security.security), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> DeleteContext(string key, string security)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				var context = this.context.Privacies.Find(security);

				if (context is not null)
				{
					this.context.Privacies.Remove(context);
					await this.context.BulkSaveChangesAsync();

					return Ok();
				}
				return NotFound(security);
			}
			return BadRequest();
		}
		public PrivaciesController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}