using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class FilesController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Files param)
		{
			try
			{
				var file = string.Concat(param.Path, param.Name);
				var directory = new DirectoryInfo(param.Path);

				if (param.Contents is not null && param.Contents.Length > 0)
				{
					if (directory.Exists is false)
						directory.Create();

					using var stream = new FileStream(file, FileMode.Create);
					await stream.WriteAsync(param.Contents.AsMemory(0, param.Contents.Length));
				}
				else if (directory.Exists)
				{
					var info = new FileInfo(file);

					switch (param.Name.Split('.')[^1])
					{
						case "zip":
							if (info.LastWriteTime.CompareTo(param.LastWriteTime) > 0)
								return Ok(new Files
								{
									LastWriteTime = info.LastWriteTime,
									Path = param.Path,
									Name = param.Name,
									Contents = await System.IO.File.ReadAllBytesAsync(file)
								});
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.PostContextAsync)}");
			}
			return Ok();
		}
		[HttpGet, ProducesResponseType(StatusCodes.Status204NoContent)]
		public IActionResult GetContext(string key)
		{
			if (string.IsNullOrEmpty(key) is false && context.User.AsNoTracking().Any(o => o.Email.Equals(key)))
				return File(System.IO.File.OpenRead(Security.File), Security.Stream, Path.GetFileName(Security.File));

			return NoContent();
		}
		public FilesController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}