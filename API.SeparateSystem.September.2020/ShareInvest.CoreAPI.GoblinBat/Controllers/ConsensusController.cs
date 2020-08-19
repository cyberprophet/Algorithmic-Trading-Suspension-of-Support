using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.CoreAPI;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class ConsensusController : ControllerBase
    {
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContext([FromBody] IEnumerable<Consensus> param)
        {
            foreach (var consensus in param)
            {
                if (await context.Consensus.AnyAsync(o => o.Code.Equals(consensus.Code) && o.Date.Equals(consensus.Date) && o.Quarter.Equals(consensus.Quarter)))
                    context.Entry(consensus).State = EntityState.Modified;

                else
                    context.Consensus.Add(consensus);

                await context.BulkSaveChangesAsync();
            }
            return Ok();
        }
        [HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContext(string code) => Ok(await context.Consensus.Where(o => o.Code.Equals(code)).AsNoTracking().ToListAsync());
        public ConsensusController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}