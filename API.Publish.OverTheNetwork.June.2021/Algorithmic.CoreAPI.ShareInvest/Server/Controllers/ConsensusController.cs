using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class ConsensusController : ControllerBase
	{
		[HttpGet]
		public async Task<IEnumerable<Consensus>> GetContextAsync()
		{
			var list = await Security.API.GetContextAsync(new Codes(), 6) as List<Codes>;
			var queue = new Queue<Consensus>();

			foreach (var st in Enum.GetNames(typeof(Catalog.AnalysisType)))
				foreach (var con in await Security.API.GetContextAsync(new Consensus { Strategics = string.Concat("TC.", st) }) as List<Consensus>)
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
			return queue.ToArray();
		}
	}
}