using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class ThemeController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Models.Theme theme)
		{
			try
			{
				if (await context.Theme.AnyAsync(o => o.Index.Equals(theme.Index)))
				{
					if (context.Theme.Any(o => o.Date.Equals(theme.Date) && o.Index.Equals(theme.Index)))
						return Ok();

					context.Entry(theme).State = EntityState.Modified;
				}
				else
					context.Theme.Add(theme);

				return Ok(context.SaveChanges());
			}
			catch (Exception ex)
			{
				await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
			}
			return BadRequest();
		}
		public ThemeController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}