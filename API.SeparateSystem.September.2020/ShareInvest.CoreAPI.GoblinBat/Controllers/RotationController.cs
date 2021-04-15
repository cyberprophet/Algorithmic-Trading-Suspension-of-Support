using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class RotationController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.Rotation rotation)
		{
			try
			{
				if (await context.Rotations.AsNoTracking().AnyAsync(o => o.Date.Equals(rotation.Date) && o.Code.Equals(rotation.Code)))
				{
					if (context.Rotations.Find(rotation.Date, rotation.Code) is Models.Rotation model)
					{
						model.Date = rotation.Date;
						model.Code = rotation.Code;
						model.Purchase = rotation.Purchase;
						model.High = rotation.High;
						model.MaxReturn = rotation.MaxReturn;
						model.Low = rotation.Low;
						model.MaxLoss = rotation.MaxLoss;
						model.Close = rotation.Close;
						model.Liquidation = rotation.Liquidation;
					}
					else
						return NoContent();
				}
				else
					context.Rotations.Add(new Models.Rotation
					{
						Date = rotation.Date,
						Code = rotation.Code,
						Purchase = rotation.Purchase,
						High = rotation.High,
						MaxReturn = rotation.MaxReturn,
						Low = rotation.Low,
						MaxLoss = rotation.MaxLoss,
						Close = rotation.Close,
						Liquidation = rotation.Liquidation
					});
				return Ok(context.SaveChanges());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{rotation.Date}\n{ex.Message}\n{rotation.Code}\n{nameof(this.PostContextAsync)}");
			}
			return BadRequest();
		}
		public RotationController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}