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

			if (param.MaturityMarketCap.Contains(Base.TransactionSuspension) == false)
				lock (Progress.Collection)
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
								Market = param.MarginRate == 1,
								OrderNumber = new Dictionary<string, dynamic>()
							};
							break;

						case 'O' when param.Code.Length == 8 && param.Code[0] == '1':
							Progress.Collection[param.Code] = new OpenAPI.Futures
							{
								Code = param.Code,
								Current = param.Code[1] == '0' ? 0D : 0,
								Offer = param.Code[1] == '0' ? 0D : 0,
								Bid = param.Code[1] == '0' ? 0D : 0,
								Collection = new Queue<Collect>(0x800),
								OrderNumber = new Dictionary<string, dynamic>()
							};
							break;

						case 'O' when param.Code.Length == 8 && param.Code[0] > '1':
							Progress.Collection[param.Code] = new OpenAPI.Options
							{
								Code = param.Code,
								Current = 0D,
								Offer = 0D,
								Bid = 0D,
								Collection = new Queue<Collect>(0x800),
								OrderNumber = new Dictionary<string, dynamic>()
							};
							break;
					}
					if (Progress.Collection.TryGetValue(param.Code, out Analysis analysis) && double.TryParse(param.Price, out double current))
					{
						if (Progress.Library.TryGetValue(param.Code, out Interface.IStrategics strategics))
							analysis.Strategics = strategics;

						if (Progress.Storage.TryGetValue(param.Code, out (Stack<double>, Stack<double>, Stack<double>) stack))
						{
							analysis.Short = stack.Item1.Count > 1 ? stack.Item1 : new Stack<double>(new double[] { current, current, current, current, current });
							analysis.Long = stack.Item2.Count > 1 ? stack.Item2 : new Stack<double>(new double[] { current, current, current, current, current });
							analysis.Trend = stack.Item3.Count > 1 ? stack.Item3 : new Stack<double>(new double[] { current, current, current, current, current });
						}
						else
						{
							analysis.Short = new Stack<double>(new double[] { current, current, current, current, current });
							analysis.Long = new Stack<double>(new double[] { current, current, current, current, current });
							analysis.Trend = new Stack<double>(new double[] { current, current, current, current, current });
						}
						bool library = Progress.Library.Remove(param.Code), storage = Progress.Storage.Remove(param.Code);

						if (library && storage)
							Base.SendMessage(param.Name, Progress.Library.Count, Progress.Collection.Count.ToString("N0"), Progress.Storage.Count, GetType());
					}
					return Ok(param.Name);
				}
			return Ok();
		}
	}
}