using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using ShareInvest.Models;

namespace ShareInvest.Controllers
{
    [ApiController, Route(Security.route), Produces(Security.produces)]
    public class CollectController : ControllerBase
    {
        [HttpPost(Security.stocks), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContexts(string code, string date, [FromBody] IEnumerable<Collect> collection)
        {
            try
            {
                if (Security.separate.ContainsKey(code) == false)
                    Security.separate[code] = new Repository(code);

                if (Security.separate.TryGetValue(code, out Repository repository))
                {
                    repository.Insert(code, collection);

                    return Ok(string.Concat(collection.Count().ToString("N0"), " Bytes_", (await repository.InsertAsync(code, date, JsonConvert.SerializeObject(repository.Sort))).ToString("N0")));
                }
            }
            catch (Exception ex)
            {
                await Record.SendToErrorMessage(string.Concat(GetType().Name, '_', code), ex.StackTrace);
            }
            return BadRequest();
        }
        [HttpPost(Security.collect), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostContexts(string code, [FromBody] IEnumerable<Collect> collection)
        {
            try
            {
                if (Security.separate.ContainsKey(code) == false)
                    Security.separate[code] = new Repository(code);

                if (Security.separate.TryGetValue(code, out Repository repository))
                {
                    repository.Insert(code, collection);

                    return Ok(repository.Count.ToString("N0"));
                }
            }
            catch (Exception ex)
            {
                await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
            }
            return BadRequest();
        }
        [HttpGet, ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContexts()
        {
            try
            {
                var now = DateTime.Now;
                var count = 0;

                foreach (var kv in Security.separate)
                    if (kv.Value.Count > 0 && await kv.Value.InsertAsync(kv.Key, (now.Hour < 9 ? now.AddDays(-1) : now).ToString(format), JsonConvert.SerializeObject(kv.Value.Sort)) > 0)
                        count++;

                Security.separate.Clear();

                return Ok(count.ToString("N0"));
            }
            catch (Exception ex)
            {
                await Record.SendToErrorMessage(GetType().Name, ex.StackTrace);
            }
            return BadRequest();
        }
        const string format = "yyyyMMdd";
    }
}