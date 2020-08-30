using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

namespace ShareInvest.Client
{
    public sealed class Consensus
    {
        [Conditional("DEBUG")]
        void SendMessage(string message) => Console.WriteLine(message);
        public async Task<Queue<Catalog.ConvertConsensus>> GetContextAsync(int quarter, string code)
        {
            var queue = new Queue<Catalog.ConvertConsensus>();

            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestCode(quarter, code), Method.GET), source.Token);

                foreach (var convert in JsonConvert.DeserializeObject<List<Catalog.UpdateConsensus>>(JObject.Parse(response.Content)[security.Data].ToString()))
                    if (double.TryParse(convert.NP, out double np) && double.TryParse(convert.OP, out double op) && double.TryParse(convert.SALES, out double sales))
                        queue.Enqueue(new Catalog.ConvertConsensus
                        {
                            Code = code,
                            Date = convert.YYMM.Substring(2),
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
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return queue;
        }
        public Consensus(dynamic key)
        {
            security = new Security(key);
            GrantAccess = security.InternalAccess;

            if (GrantAccess && security.GrantAccess)
            {
                client = new RestClient(security.Info)
                {
                    Timeout = -1
                };
                source = new CancellationTokenSource();
            }
        }
        public bool GrantAccess
        {
            get;
        }
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly IRestClient client;
    }
}