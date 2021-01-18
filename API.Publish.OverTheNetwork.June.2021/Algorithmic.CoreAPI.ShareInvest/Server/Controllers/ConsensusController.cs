using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[Authorize, ApiController, Route(Security.route), Produces(Security.produces)]
	public class ConsensusController : ControllerBase
	{
		[HttpGet]
		public async Task<IEnumerable<Consensus>> GetContextAsync()
		{
			var list = await context.Codes.Where(o => o.Code.Length == 6).Select(o => new { o.Code, o.Name, o.MarginRate, o.MaturityMarketCap, o.Price }).AsNoTracking().ToListAsync();
			var queue = new Queue<Consensus>();

			foreach (var st in Enum.GetNames(typeof(Catalog.AnalysisType)))
			{
				var find = string.Concat("TC.", st);
				var where = context.Estimate.Where(o => o.Strategics.Equals(find)).AsNoTracking();
				var max = await where.MaxAsync(o => o.Date);

				foreach (var con in await where.Where(o => o.Date.Equals(max)).Select(o => new { o.Code, o.FirstQuarter, o.SecondQuarter, o.ThirdQuarter, o.Quarter, o.TheNextYear, o.TheYearAfterNext }).ToListAsync())
				{
					var key = list.Find(o => o.Code.Equals(con.Code));

					if (key.MaturityMarketCap.Contains(Base.TransactionSuspension) || string.IsNullOrEmpty(con.Code))
						Base.SendMessage(GetType(), key.Code, key.MaturityMarketCap);

					else
						queue.Enqueue(new Consensus
						{
							Code = con.Code,
							Date = key.Name,
							Strategics = st,
							FirstQuarter = con.FirstQuarter,
							SecondQuarter = con.SecondQuarter,
							ThirdQuarter = con.ThirdQuarter,
							Quarter = con.Quarter,
							TheNextYear = con.TheNextYear,
							TheYearAfterNext = con.TheYearAfterNext
						});
				}
			}
			return queue.ToArray();
		}
		public ConsensusController(CoreApiDbContext context) => this.context = context;
		readonly CoreApiDbContext context;
	}
}