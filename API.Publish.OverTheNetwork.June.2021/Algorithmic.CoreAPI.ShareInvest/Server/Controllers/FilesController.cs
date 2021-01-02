using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return Ok();
		}
	}
}