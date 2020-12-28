using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.CoreAPI;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class FinancialStatementController : ControllerBase
	{
		[HttpPost(Security.routeKey), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostContext(string key, [FromBody] IEnumerable<FinancialStatement> param)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
			{
				foreach (var consensus in param)
				{
					if (await context.Financials.AnyAsync(o => o.Code.Equals(consensus.Code) && o.Date.Equals(consensus.Date)))
						context.Entry(consensus).State = EntityState.Modified;

					else
						context.Financials.Add(consensus);

					await context.BulkSaveChangesAsync();
				}
				return Ok();
			}
			return BadRequest();
		}
		/*
	[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> PostContext([FromBody] IEnumerable<FinancialStatement> param)
	{
		foreach (var consensus in param.Cast<QuarterlyFinancialStatements>())
		{
			if (await context.Quarter.AnyAsync(o => o.Code.Equals(consensus.Code) && o.Date.Equals(consensus.Date)))
				context.Entry(consensus).State = EntityState.Modified;

			else
				context.Quarter.Add(consensus);

			await context.BulkSaveChangesAsync();
		}
		return Ok();
	}
		*/
		[HttpGet(Security.routeKey), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContext(string key)
		{
			if (key.Length == 6)
			{

			}
			/*
				return Ok(await context.Quarter.Where(o => o.Code.Equals(key)).AsNoTracking().Select(o => new
				{
					o.Date,
					o.Revenues,
					o.IncomeFromOperations,
					o.NetIncome,
					o.OperatingActivities
				}).ToListAsync());
			*/
			else if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
				return Ok(await context.Financials.AsNoTracking().ToListAsync());

			return BadRequest();
		}
		[HttpGet(Security.routeCodes), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContext(string key, string code)
		{
			if (await context.Privacies.AnyAsync(o => o.Security.Equals(Security.GetGrantAccess(key))))
				return Ok(await context.Financials.Where(o => o.Code.Equals(code)).AsNoTracking().Select(o => new
				{
					o.Date,
					o.Revenues,
					o.IncomeFromOperations,
					o.NetIncome,
					o.OperatingActivities
				}).ToListAsync());
			return BadRequest();
		}
		public FinancialStatementController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}