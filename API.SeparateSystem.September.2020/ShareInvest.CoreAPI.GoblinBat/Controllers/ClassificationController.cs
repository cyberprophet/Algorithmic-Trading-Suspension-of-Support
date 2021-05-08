using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class ClassificationController : ControllerBase
	{
		[HttpPost, ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> PostContextAsync([FromBody] Models.Classification model)
		{
			try
			{
				if (await context.Classifications.AsNoTracking().AnyAsync(o => o.Code.Equals(model.Code) && o.Index.Equals(model.Index)))
				{
					if (context.Classifications.AsNoTracking().Single(o => o.Code.Equals(model.Code) && o.Index.Equals(model.Index)).Title.Equals(model.Title))
						return NoContent();

					else
						context.Classifications.Find(new
						{
							model.Code,
							model.Index

						}).Title = model.Title;
				}
				else
					context.Classifications.Add(model);

				return Ok(context.SaveChanges());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{nameof(this.PostContextAsync)}");
			}
			return BadRequest();
		}
		public ClassificationController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}