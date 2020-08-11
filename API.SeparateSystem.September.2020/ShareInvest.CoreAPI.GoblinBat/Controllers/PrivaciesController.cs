using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.CoreAPI;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class PrivaciesController : ControllerBase
    {
        [HttpGet(Security.security), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContext(string security)
        {
            var context = await this.context.Privacies.FindAsync(security);

            if (context == null)
                return NotFound();

            return Ok(context);
        }
        [HttpGet, ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetContexts() => NoContent();
        [HttpPost, ProducesResponseType(StatusCodes.Status202Accepted), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext([FromBody] Privacy privacy)
        {
            if (await context.Privacies.AnyAsync(o => o.Security.Equals(privacy.Security)))
                return Accepted((await context.Privacies.FirstAsync(o => o.Security.Equals(privacy.Security))).Coin);

            context.Privacies.Add(privacy);
            await context.BulkSaveChangesAsync();

            return Ok((await context.Privacies.FirstAsync(o => o.Security.Equals(privacy.Security))).Coin);
        }
        [HttpPut(Security.security), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutContext(string security, [FromBody] Privacy privacy)
        {
            if (privacy.Security.Equals(security))
            {
                context.Entry(privacy).State = EntityState.Modified;
                await context.BulkSaveChangesAsync();

                return Ok((await context.Privacies.FirstAsync(o => o.Security.Equals(security))).Coin);
            }
            return BadRequest();
        }
        [HttpDelete(Security.security), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteContext(string security)
        {
            var context = await this.context.Privacies.FindAsync(security);

            if (context != null)
            {
                this.context.Privacies.Remove(context);
                await this.context.BulkSaveChangesAsync();

                return Ok();
            }
            return NotFound(security);
        }
        public PrivaciesController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}