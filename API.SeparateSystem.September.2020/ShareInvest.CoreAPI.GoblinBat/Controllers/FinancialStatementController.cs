using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
		[HttpPost, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PostContext([FromBody] IEnumerable<FinancialStatement> param)
		{
			foreach (var consensus in param)
			{
				var quarter = new QuarterlyFinancialStatements
				{
					BPS = consensus.BPS,
					CAPEX = consensus.CAPEX,
					Code = consensus.Code,
					ControllingEquity = consensus.ControllingEquity,
					ControllingNetIncome = consensus.ControllingNetIncome,
					Date = consensus.Date,
					DebtRatio = consensus.DebtRatio,
					DividendYield = consensus.DividendYield,
					DPS = consensus.DPS,
					EPS = consensus.EPS,
					EquityCapital = consensus.EquityCapital,
					FCF = consensus.FCF,
					FinancingActivities = consensus.FinancingActivities,
					IncomeFromOperation = consensus.IncomeFromOperation,
					IncomeFromOperations = consensus.IncomeFromOperations,
					InterestAccruingLiabilities = consensus.InterestAccruingLiabilities,
					InvestingActivities = consensus.InvestingActivities,
					IssuedStocks = consensus.IssuedStocks,
					NetIncome = consensus.NetIncome,
					NetMargin = consensus.NetMargin,
					NonControllingEquity = consensus.NonControllingEquity,
					NonControllingNetIncome = consensus.NonControllingNetIncome,
					OperatingActivities = consensus.OperatingActivities,
					OperatingMargin = consensus.OperatingMargin,
					PayoutRatio = consensus.PayoutRatio,
					PBR = consensus.PBR,
					PER = consensus.PER,
					ProfitFromContinuingOperations = consensus.ProfitFromContinuingOperations,
					RetentionRatio = consensus.RetentionRatio,
					Revenues = consensus.Revenues,
					ROA = consensus.ROA,
					ROE = consensus.ROE,
					TotalAssets = consensus.TotalAssets,
					TotalEquity = consensus.TotalEquity,
					TotalLiabilites = consensus.TotalLiabilites
				};
				if (await context.Quarter.AnyAsync(o => o.Code.Equals(quarter.Code) && o.Date.Equals(quarter.Date)))
					context.Entry(quarter).State = EntityState.Modified;

				else
					context.Quarter.Add(quarter);

				await context.BulkSaveChangesAsync();
			}
			return Ok();
		}
		[HttpGet(Security.routeKey), ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContext(string key)
		{
			if (key.Length == 6)
				return Ok(await context.Quarter.Where(o => o.Code.Equals(key)).AsNoTracking().Select(o => new
				{
					o.Date,
					o.Revenues,
					o.IncomeFromOperations,
					o.NetIncome,
					o.OperatingActivities
				}).ToListAsync());
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