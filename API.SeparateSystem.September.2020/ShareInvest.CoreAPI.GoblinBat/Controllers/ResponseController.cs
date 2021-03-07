using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class ResponseController : ControllerBase
	{
		[HttpGet(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest), SupportedOSPlatform("windows")]
		public async Task<IActionResult> GetContextAsync(string key)
		{
			try
			{
				return Ok(Repository.ReadTheFile(key));
			}
			catch (Exception ex)
			{
				await new Task(() => Console.WriteLine($"{GetType()}\n{ex.Message}\n{key}\n{ex.InnerException.Message}\n{nameof(this.GetContextAsync)}"));
			}
			return BadRequest();
		}
		[HttpGet(Security.routeStocks), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetContextAsync(string key, string code)
		{
			try
			{
				var now = DateTime.Now;
				var minus = 0;

				while (now.Month - minus > 0)
					foreach (var file in Directory.GetFiles(Path.Combine("F:\\Res", code, now.Year.ToString("D4"), (now.Month - minus++).ToString("D2")), ".res", SearchOption.TopDirectoryOnly).OrderByDescending(o => o))
					{
						var split = file.Split('_');
						using var sr = new StreamReader(file);

						return Ok(new Catalog.Models.Response
						{
							Status = split[^1].Split('.')[0],
							Url = split[0].Split('\\')[^1],
							Post = await sr.ReadToEndAsync()
						});
					}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{key}\n{ex.InnerException.Message}\n{nameof(this.GetContextAsync)}");
			}
			return BadRequest();
		}
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest), SupportedOSPlatform("windows")]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.Response response)
		{
			try
			{
				var exists = response.Url.Split('_')[0];
				Repository.CreateTheDirectory(new DirectoryInfo(exists.Remove(exists.Length - 3)));

				if (Repository.ReadTheFile(response.Url) is false)
					using (var sw = new StreamWriter(response.Url, false))
					{
						await sw.WriteLineAsync(response.Post);

						return Ok();
					}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{response.Post}\n{ex.InnerException.Message}\n{nameof(this.PostContextAsync)}");
			}
			return BadRequest();
		}
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PutContextAsync([FromBody] Catalog.Models.Response response)
		{
			try
			{
				if (await context.Group.AnyAsync(o => o.Code.Equals(response.Status)) && int.TryParse(response.Post, out int page))
				{
					if (context.Group.Find(response.Status) is Models.Group group)
						group.Page = new Models.Response
						{
							Code = response.Status,
							Tistory = page
						};
					else if (context.Page.Find(response.Status) is Models.Response res)
						res.Tistory = page;

					else
						return NoContent();

					return Ok(context.SaveChanges());
				}
				else
					return NoContent();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{response.Url}\n{ex.InnerException.Message}\n{nameof(this.PutContextAsync)}");
			}
			return BadRequest();
		}
		public ResponseController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}