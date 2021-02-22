using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using ShareInvest.Catalog;

namespace ShareInvest.Client
{
	public sealed class Consensus
	{
		public async Task<Queue<ConvertConsensus>> GetContextAsync(int quarter, string code)
		{
			try
			{
				var queue = new Queue<ConvertConsensus>();
				var response = await client.ExecuteAsync(new RestRequest(Security.RequestTheIntegratedAddress(quarter, code), Method.GET), source.Token);

				foreach (var convert in JsonConvert.DeserializeObject<List<UpdateConsensus>>(JObject.Parse(response.Content)[Security.Data].ToString()))
					if (double.TryParse(convert.NP, out double np) && double.TryParse(convert.OP, out double op) && double.TryParse(convert.SALES, out double sales))
						queue.Enqueue(new ConvertConsensus
						{
							Code = code,
							Date = convert.YYMM[2..],
							Quarter = quarter.ToString(),
							Sales = (long)(sales * 0x5F5E100),
							YoY = double.TryParse(convert.YOY, out double yoy) ? 1 + yoy * 1e-2 : 0,
							Op = (long)(op * 0x5F5E100),
							Np = (long)(np * 0x5F5E100),
							Eps = double.TryParse(convert.EPS, out double eps) ? (int)eps : 0,
							Bps = double.TryParse(convert.BPS, out double bps) ? (int)bps : 0,
							Per = double.TryParse(convert.PER, out double per) ? per : 0,
							Pbr = double.TryParse(convert.PBR, out double pbr) ? pbr : 0,
							Roe = convert.ROE,
							Ev = convert.EV
						});
				return queue;
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
			}
			return null;
		}
		public Consensus(dynamic key)
		{
			var security = new Security(key);
			GrantAccess = security.InternalAccess;

			if (GrantAccess && security.GrantAccess)
				client = new RestClient(security.Info)
				{
					Timeout = -1
				};
			source = new CancellationTokenSource();
		}
		public bool GrantAccess
		{
			get;
		}
		readonly CancellationTokenSource source;
		readonly IRestClient client;
	}
}