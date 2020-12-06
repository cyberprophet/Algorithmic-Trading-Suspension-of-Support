using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class CodesController : ControllerBase
	{
		[HttpPut, ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> PutContextAsync([FromBody] Codes param)
		{
			if (Progress.Company is 'O' && (param.Code.Length == 6 || param.Code.Length == 8 && param.Code[0] > '1')
				&& await Progress.Client.PutContextAsync(param) is string code)
				Base.SendMessage(code, Progress.Collection.Count, GetType());

			if (param.MaturityMarketCap.Contains(Base.TransactionSuspension))
				Base.SendMessage(param.Name, Progress.Collection.Remove(param.Code), param.Code, param.MaturityMarketCap, GetType());

			else
			{
				switch (Progress.Company)
				{
					case 'O' when param.Code.Length == 6:
						Progress.Collection[param.Code] = new OpenAPI.Stocks
						{
							Code = param.Code,
							Current = 0,
							Offer = 0,
							Bid = 0,
							Collection = new Queue<Collect>(0x800),
							Market = param.MarginRate == 1
						};
						break;

					case 'O' when param.Code.Length == 8 && param.Code[0] == '1':
						Progress.Collection[param.Code] = new OpenAPI.Futures
						{
							Code = param.Code,
							Current = param.Code[1] == '0' ? 0D : 0,
							Offer = param.Code[1] == '0' ? 0D : 0,
							Bid = param.Code[1] == '0' ? 0D : 0,
							Collection = new Queue<Collect>(0x800)
						};
						break;

					case 'O' when param.Code.Length == 8 && param.Code[0] > '1':
						Progress.Collection[param.Code] = new OpenAPI.Options
						{
							Code = param.Code,
							Current = 0D,
							Offer = 0D,
							Bid = 0D,
							Collection = new Queue<Collect>(0x800)
						};
						break;
				}
				if (Progress.Library.TryGetValue(param.Code, out Interface.IStrategics strategics)
					&& Progress.Collection.TryGetValue(param.Code, out Analysis analysis))
				{
					if (Progress.Storage.TryGetValue(param.Code, out (Stack<double>, Stack<double>, Stack<double>) stack))
					{
						analysis.Short = stack.Item1;
						analysis.Long = stack.Item2;
						analysis.Trend = stack.Item3;
						Base.SendMessage(param.Name, stack.Item3.Count, analysis.GetType());
					}
					analysis.Strategics = strategics;
					Base.SendMessage(param.Name, strategics.GetType());
				}
				return Ok(param.Name);
			}
			return Ok();
		}
	}
}