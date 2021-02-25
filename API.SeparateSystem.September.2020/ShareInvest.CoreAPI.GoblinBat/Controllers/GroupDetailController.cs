using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using ShareInvest.Filter;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class GroupDetailController : ControllerBase
	{
		static Tendency[] GetTendencies(Catalog.Models.GroupDetail gd)
		{
			var tendencies = new Tendency[gd.Tick.Length];

			for (int i = 0; i < gd.Tick.Length; i++)
				tendencies[i] = new Tendency
				{
					Code = gd.Code,
					Tick = gd.Tick[i],
					Inclination = double.IsNaN(gd.Inclination[i]) ? 0 : gd.Inclination[i]
				};
			return tendencies;
		}
		[HttpGet(Security.collect), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> GetContextAsync(string code)
		{
			var now = DateTime.Now;

			if (await context.Details.FindAsync(code) is GroupDetail detail && detail.Date.Equals((now.Hour < 0x12 ? now.AddDays(-1) : now).ToString(Base.DateFormat)))
				return Ok(context.Group.Find(code).Index);

			else
				return NoContent();
		}
		[HttpPost, ServiceFilter(typeof(ClientIpCheckActionFilter)), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContextAsync([FromBody] Catalog.Models.GroupDetail gd)
		{
			try
			{
				if (await context.Group.FindAsync(gd.Code) is Group rp)
				{
					if (context.Details.Find(gd.Code) is GroupDetail detail && gd.Date.CompareTo(detail.Date) > 0)
					{
						rp.Title = gd.Title;
						rp.Index = gd.Index;
						detail.Date = gd.Date;
						detail.Compare = gd.Compare;
						detail.Current = gd.Current;
						detail.Percent = gd.Percent;
						detail.Rate = gd.Rate;

						foreach (var tendency in GetTendencies(gd))
							context.Tendencies.Find(tendency.Code, tendency.Tick).Inclination = tendency.Inclination;
					}
					else if (rp.Index.Equals(gd.Index) is false && context.Theme.Find(rp.Index).Rate < context.Theme.Find(gd.Index).Rate)
						rp.Index = gd.Index;

					else
						return NoContent();
				}
				else
					context.Group.Add(new Group
					{
						Code = gd.Code,
						Title = gd.Title,
						Index = gd.Index,
						Details = new GroupDetail
						{
							Code = gd.Code,
							Date = gd.Date,
							Compare = gd.Compare,
							Current = gd.Current,
							Percent = gd.Percent,
							Rate = gd.Rate,
							Tendencies = GetTendencies(gd)
						}
					});
				return Ok(context.SaveChanges());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{GetType()}\n{ex.Message}\n{JsonConvert.SerializeObject(gd)}\n{ex.InnerException.Message}\n{nameof(this.PostContextAsync)}");
			}
			return BadRequest();
		}
		public GroupDetailController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}