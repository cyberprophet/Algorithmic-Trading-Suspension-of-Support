using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

using ShareInvest.CoreAPI;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class DaysController : ControllerBase
    {
        [HttpGet, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetContexts()
        {
            Retention param;

            try
            {
                if (Registry.Retentions != null && Registry.Retentions.Count > 0)
                {
                    var registry = Registry.Retentions.First();
                    param = new Retention
                    {
                        Code = registry.Key,
                        LastDate = registry.Value
                    };
                    if (Registry.Retentions.Remove(registry.Key))
                        return Ok(param);
                }
            }
            catch (Exception ex)
            {
                param = new Retention
                {
                    Code = ex.TargetSite.Name,
                    LastDate = ex.StackTrace
                };
            }
            return NotFound(param);
        }
        [HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostContext([FromBody] Queue<Days> chart)
        {
            await context.BulkInsertAsync<Queue<Days>>(chart, o =>
            {
                o.InsertIfNotExists = true;
                o.BatchSize = 0x43AD;
                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                o.AutoMapOutputDirection = false;
            });
            return GetContexts();
        }
        public DaysController(CoreApiDbContext context) => this.context = context;
        readonly CoreApiDbContext context;
    }
}