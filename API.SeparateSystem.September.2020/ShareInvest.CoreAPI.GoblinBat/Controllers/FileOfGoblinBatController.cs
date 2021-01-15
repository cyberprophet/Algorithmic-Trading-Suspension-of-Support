using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class FileOfGoblinBatController : ControllerBase
	{
		[HttpGet(Security.routeFile), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContext(string code)
		{
			if (await context.File.AnyAsync(o => o.Version.CompareTo(code) > 0))
				return Ok((await context.File.FirstAsync(o => o.Version.CompareTo(code) > 0)).Content);

			return NotFound();
		}
		[HttpPost(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContext(string key, [FromBody] FileOfGoblinBat param)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				if (await context.File.AnyAsync(o => o.Version.CompareTo(param.Version) == 0))
					context.Entry(param).State = EntityState.Modified;

				else
					context.File.Add(param);

				await context.BulkSaveChangesAsync();

				return Ok();
			}
			return BadRequest();
		}
		public FileOfGoblinBatController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}