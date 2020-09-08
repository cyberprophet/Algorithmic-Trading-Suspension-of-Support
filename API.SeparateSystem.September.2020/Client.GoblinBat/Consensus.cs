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
        public async Task<Queue<Catalog.Dart.FinancialStatement>> GetContextAsync(string code, int quarter)
        {
            var queue = new Queue<Catalog.Dart.FinancialStatement>();

            try
            {
                string param = string.Empty, id = string.Empty;

                foreach (var str in (await client.ExecuteAsync(new RestRequest(security.RequestParameter(code), Method.GET), source.Token)).Content.Split(security.Exception))
                {
                    if (str.Trim().StartsWith(security.Enc))
                        param = str.Trim().Replace(security.Enc, string.Empty);

                    else if (str.Trim().StartsWith(security.Id))
                        id = str.Trim().Replace(security.Id, string.Empty).Split('?')[0];
                }
                param = param.Remove(param.Length - 1, 1);
                id = id.Remove(id.Length - 2, 2);
                var read = false;
                var index = 0;
                var temp = new string[8];
                var dictionary = new Dictionary<string, string[]>();

                foreach (var str in (await client.ExecuteAsync(new RestRequest(security.RequestParameter(code, quarter, param, id), Method.GET), source.Token)).Content.Split(security.Exception))
                {
                    param = str.Replace("&nbsp;", string.Empty).Replace("tr", string.Empty).Replace("tbody", string.Empty).Replace("th", string.Empty).Replace("br", string.Empty).Replace("span", string.Empty).Replace("td", string.Empty).Replace("/", string.Empty).Trim();

                    if (read == false)
                        read = param.Equals("주요재무정보");

                    else if (string.IsNullOrWhiteSpace(param) == false && read && param.Length > 0 && param.StartsWith("caption") == false && param.StartsWith("row=") == false && param.StartsWith("주요재무정보") == false && param.StartsWith("ead") == false && param.StartsWith("table") == false && param.StartsWith("(IFRS별도)") == false && param.StartsWith("(GAAP개별)") == false && param.StartsWith("(IFRS연결)") == false && param.StartsWith("class=") == false)
                    {
                        if (char.IsLetter(param[0]))
                        {
                            if (Array.Exists(temp, o => string.IsNullOrEmpty(o)) == false)
                                dictionary[id] = temp;

                            id = param;
                            index = 0;
                            temp = new string[8];
                        }
                        else
                            temp[index++] = param;
                    }
                }
                for (index = 0; index < temp.Length; index++)
                {
                    var serialize = new Dictionary<string, string>()
                    {
                        { "종목코드", code }
                    };
                    foreach (var kv in dictionary)
                        serialize[kv.Key] = kv.Key.Equals("연간") ? (kv.Value[index].EndsWith("(E)") ? kv.Value[index].Insert(4, ".") : kv.Value[index].Insert(kv.Value[index].Length, "(A)").Insert(4, ".")).Substring(2) : kv.Value[index].Replace(",", string.Empty);

                    queue.Enqueue(JsonConvert.DeserializeObject<Catalog.Dart.FinancialStatement>(JsonConvert.SerializeObject(serialize)));
                }
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
            }
            source = new CancellationTokenSource();
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