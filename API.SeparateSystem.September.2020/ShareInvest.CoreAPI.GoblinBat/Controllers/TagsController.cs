using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class TagsController : ControllerBase
	{
		[HttpPut, ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> PutContextAsync([FromBody] Catalog.Dart.Tags tag)
		{
			try
			{
				switch (tag.Key.Length)
				{
					case 6:
						if (context.StockTags.Find(tag.Key) is Models.StockTags st)
						{
							st.ID = tag.ID;
							st.Tags = tag.Json;
							st.Size = tag.Size;
						}
						else if (context.Codes.Find(tag.Key) is Models.Codes cs)
							cs.Tags = new Models.StockTags
							{
								Code = tag.Key,
								ID = tag.ID,
								Tags = tag.Json,
								Size = tag.Size
							};
						else
							return NoContent();

						break;

					default:
						if (context.ThemeTags.Find(tag.Key) is Models.ThemeTags ts)
						{
							ts.ID = tag.ID;
							ts.Tags = tag.Json;
							ts.Size = tag.Size;
						}
						else if (context.Theme.Find(tag.Key) is Models.Theme theme)
							theme.Tags = new Models.ThemeTags
							{
								Index = tag.Key,
								ID = tag.ID,
								Tags = tag.Json,
								Size = tag.Size
							};
						else
							return NoContent();

						break;
				}
				return Ok(await context.SaveChangesAsync());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{tag.Key}\n{ex.Message}\n{tag.ID}\n{nameof(this.PutContextAsync)}");
			}
			return BadRequest();
		}
		public TagsController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}