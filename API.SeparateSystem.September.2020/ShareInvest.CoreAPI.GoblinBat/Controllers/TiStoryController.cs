using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class TiStoryController : ControllerBase
	{
		[HttpGet(Security.collect), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> GetContextAsync(string code)
		{
			if (await context.Group.FindAsync(code) is Models.Group group)
				return Ok(new
				{
					group.Index,
					group.Title
				});
			else
				return NoContent();
		}
		public TiStoryController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}