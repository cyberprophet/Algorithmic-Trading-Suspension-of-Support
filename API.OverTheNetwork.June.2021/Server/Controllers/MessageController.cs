using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class MessageController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContextAsync([FromBody] Message param)
		{
			try
			{
				if (Logs is null)
					Logs = new Queue<Log>(0x80);

				var log = param.Convey.Split('(');
				var message = log[0].Split(']');
				Logs.Enqueue(new Log
				{
					Time = DateTime.Now,
					Message = message[^1].Trim(),
					Code = message[0].Replace("[", string.Empty),
					Screen = log[^1].Remove(log[^1].Length - 1)
				});
			}
			catch (Exception ex)
			{
				await new Task(() => Base.SendMessage(ex.StackTrace, GetType()));
			}
			return Ok();
		}
		[HttpGet]
		public IEnumerable<Log> GetContext()
		{
			if (Logs is null)
				Logs = new Queue<Log>(0x80);

			return Logs.ToArray();
		}
		static Queue<Log> Logs
		{
			get; set;
		}
	}
}