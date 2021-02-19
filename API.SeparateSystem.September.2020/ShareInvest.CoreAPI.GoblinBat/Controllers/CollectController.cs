using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class CollectController : ControllerBase
	{
		[HttpPost(Security.stocks), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContexts(string code, string date, [FromBody] IEnumerable<Models.Collect> collection)
		{
			try
			{
				if (Security.separate.ContainsKey(code) == false)
					Security.separate[code] = new Models.Repository(code);

				if (Security.separate.TryGetValue(code, out Models.Repository repository))
				{
					repository.Insert(code, collection);

					return Ok(string.Concat(collection.Count().ToString("N0"), " Bytes_", (await repository.InsertAsync(code, date, JsonConvert.SerializeObject(repository.Sort))).ToString("N0")));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.PostContexts)}");
			}
			return BadRequest();
		}
		[HttpPost(Security.collect), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContexts(string code, [FromBody] IEnumerable<Models.Collect> collection)
		{
			try
			{
				if (Security.separate.ContainsKey(code) == false)
					Security.separate[code] = new Models.Repository(code);

				if (Security.separate.TryGetValue(code, out Models.Repository repository))
				{
					repository.Insert(code, collection);

					return Ok(repository.Count.ToString("N0"));
				}
			}
			catch (Exception ex)
			{
				await new Task(() => Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.PostContexts)}"));
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
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.GetContexts)}");
			}
			return BadRequest();
		}
		const string format = "yyyyMMdd";
	}
}