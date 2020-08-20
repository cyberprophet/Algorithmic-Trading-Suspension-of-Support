using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.CoreAPI;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class FileOfGoblinBatController : ControllerBase
    {
        [HttpGet(Security.routeRetention), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetContext(string code)
        {
            if (await context.File.AnyAsync(o => o.Version.CompareTo(code) > 0))
                return Ok((await context.File.FirstAsync(o => o.Version.CompareTo(code) > 0)).Content);

            return NoContent();
        }
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext([FromBody] FileOfGoblinBat param)
        {
            if (await context.File.AnyAsync(o => o.Version.CompareTo(param.Version) == 0))
                context.Entry(param).State = EntityState.Modified;

            else
                context.File.Add(param);

            await context.BulkSaveChangesAsync();

            return Ok();
        }
        public FileOfGoblinBatController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}