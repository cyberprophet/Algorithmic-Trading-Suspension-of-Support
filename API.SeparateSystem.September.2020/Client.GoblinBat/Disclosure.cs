using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using ShareInvest.Catalog.KRX;

namespace ShareInvest.Client
{
    public class Disclosure
    {
        public async Task GetDisclosureInformation(string code, string name) => Process.Start(security.GetUrl(await GetDocumentNumber(code), name));
        public async Task<List<MKD30015>> GetMarketCap(dynamic key, int page)
        {
            try
            {
                var now = DateTime.Now;
                var response = (await client.ExecuteAsync(new RestRequest(security.RequestMKD99000001, Method.POST).AddHeader(security.ContentType, security.Form).AddParameter(security.Param[0], (Path)key).AddParameter(security.Param[1], Path.ALL).AddParameter(security.Param[2], now.Hour > 15 ? now.ToString(format) : now.AddDays(-1).ToString(format)).AddParameter(security.Param[3], (await client.ExecuteAsync(new RestRequest(security.RequestMarketCap, Method.GET))).Content).AddParameter(security.Param[4], page), source.Token));

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<List<MKD30015>>(JObject.Parse(response.Content)[security.MKD99000001].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
        public async Task<Queue<Catalog.ConvertConsensus>> GetFinancialStatements(string code, string year)
        {
            try
            {
                var statements = new Queue<Catalog.ConvertConsensus>();
                var param = new string[security.InfoParam.Length];
                var index = 0;
                var exception = new char[] { '\n', '<', '>', '\t' };
                Dictionary<string, long> dp = new Dictionary<string, long>(), lp = new Dictionary<string, long>(), p = new Dictionary<string, long>();

                foreach (var str in (await client.ExecuteAsync(new RestRequest(security.SearchCorp, Method.POST).AddHeader(security.ContentType, security.Form).AddParameter(security.Text, code))).Content.Split(exception))
                    if (str.StartsWith(security.Filter[0]))
                    {
                        var split = str.Split('=');
                        param[index++] = split[split.Length - 1].Replace("'", string.Empty);
                    }
                param[index++] = year.Length == 4 ? year : year.Substring(0, 4);
                param[index++] = security.RequestQuarter(year.Length == 4 ? 0 : security.RequestQuarter(year.Substring(5)));
                param[index] = "0";
                var request = new RestRequest(security.DisclosureInfo, Method.POST).AddHeader(security.ContentType, security.Form);
                var url = string.Empty;

                for (index = 0; index < param.Length; index++)
                    request.AddParameter(security.InfoParam[index], param[index]);

                foreach (var str in (await client.ExecuteAsync(request)).Content.Split(exception))
                    if (str.StartsWith(security.Filter[1]))
                        url = str.Replace(security.Filter[1], string.Empty);

                if (string.IsNullOrEmpty(url) == false)
                {
                    var response = await new RestClient(url.Remove(url.Length - 1, 1).Remove(0, 1)).ExecuteAsync(new RestRequest(Method.GET));

                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        bool filter = false, assets = true;
                        var turn = 0;
                        var temp = new long[3];
                        var queue = new Queue<long[]>();
                        var names = new Queue<string>();

                        foreach (var str in response.Content.Replace(@"&nbsp;", string.Empty).Split(exception))
                        {
                            var sTrim = str.Trim();

                            if (filter)
                            {
                                sTrim = sTrim.Replace(",", string.Empty);

                                if (sTrim.StartsWith("(단위") || sTrim.EndsWith("(단위:원)"))
                                    switch (sTrim)
                                    {
                                        case string unit when unit.EndsWith("백만원)"):
                                            index = 0xF4240;
                                            break;

                                        case string unit when unit.EndsWith("만원)"):
                                            index = 0x2710;
                                            break;

                                        default:
                                            index = 1;
                                            break;
                                    }
                                else if (long.TryParse((sTrim.StartsWith("(") ? sTrim.Replace("(", "-").Replace(")", string.Empty) : sTrim).Replace(",", string.Empty), out long num) && assets)
                                {
                                    if (turn < 3)
                                        temp[turn] = num * index;

                                    if (turn++ == 2)
                                    {
                                        queue.Enqueue(temp);
                                        names.Enqueue(url.Replace(" ", string.Empty));
                                    }
                                }
                                else
                                {
                                    if (sTrim.StartsWith("제") && sTrim.Contains("."))
                                    {
                                        var date = sTrim.Split('.')[0];

                                        if (int.TryParse(date.Substring(date.Length - 4), out int dYear))
                                            temp[turn++] = dYear;

                                        if (turn == 3)
                                            queue.Enqueue(temp);
                                    }
                                    else
                                    {
                                        if (sTrim.EndsWith(" (기초자본)"))
                                            assets = false;

                                        else if (sTrim.StartsWith("영업활동") && sTrim.EndsWith("현금흐름"))
                                            assets = true;

                                        turn = 0;
                                    }
                                    url = sTrim;
                                }
                                if (turn == 0)
                                    temp = new long[3];
                            }
                            filter = sTrim.Equals("P");
                        }
                        while (queue.Count > 0)
                        {
                            var array = queue.Dequeue();
                            url = array[0].ToString().Length == 4 && array[1].ToString().Length == 4 && array[2].ToString().Length == 4 ? string.Empty : names.Dequeue();

                            if (string.IsNullOrEmpty(url))
                            {
                                if (dp.Count > 0 && lp.Count > 0 && p.Count > 0 && code.Length == 6)
                                    for (index = 0; index < array.Length; index++)
                                    {
                                        Catalog.Dart.BalanceSheet sheet;
                                        Catalog.Dart.IncomeStatement income;
                                        Catalog.Dart.CashFlowStatement flow;
                                        string serialize;

                                        switch (index)
                                        {
                                            case 0:
                                                serialize = JsonConvert.SerializeObject(p, Formatting.Indented);
                                                break;

                                            case 1:
                                                serialize = JsonConvert.SerializeObject(lp, Formatting.Indented);
                                                break;

                                            case 2:
                                                serialize = JsonConvert.SerializeObject(dp, Formatting.Indented);
                                                break;

                                            default:
                                                continue;
                                        }
                                        if (dp.ContainsKey(non) && lp.ContainsKey(non) && p.ContainsKey(non) && dp.ContainsKey(current) && lp.ContainsKey(current) && p.ContainsKey(current))
                                        {
                                            sheet = JsonConvert.DeserializeObject<Catalog.Dart.BalanceSheet>(serialize);
                                            sheet.Code = code;
                                            sheet.Date = array[index].ToString("D4");
                                        }
                                        else if (dp.ContainsKey(cost) && lp.ContainsKey(cost) && p.ContainsKey(cost) && dp.ContainsKey(profit) && lp.ContainsKey(profit) && p.ContainsKey(profit))
                                        {
                                            income = JsonConvert.DeserializeObject<Catalog.Dart.IncomeStatement>(serialize);
                                            income.Code = code;
                                            income.Date = array[index].ToString("D4");
                                            statements.Enqueue(new Catalog.ConvertConsensus
                                            {
                                                Code = income.Code,
                                                Date = string.Concat(income.Date.Substring(2), ".12(A)"),
                                                Quarter = "0",
                                                Sales = income.Revenues,
                                                Op = income.IncomeFromOperations,
                                                Np = income.NetIncome
                                            });
                                        }
                                        else if (dp.ContainsKey(finance) && lp.ContainsKey(finance) && p.ContainsKey(finance) && dp.ContainsKey(invest) && lp.ContainsKey(invest) && p.ContainsKey(invest) && dp.ContainsKey(operate) && lp.ContainsKey(operate) && p.ContainsKey(operate))
                                        {
                                            flow = JsonConvert.DeserializeObject<Catalog.Dart.CashFlowStatement>(serialize);
                                            flow.Code = code;
                                            flow.Date = array[index].ToString("D4");
                                        }
                                    }
                                dp = new Dictionary<string, long>();
                                lp = new Dictionary<string, long>();
                                p = new Dictionary<string, long>();
                            }
                            else
                            {
                                dp[url] = array[2];
                                lp[url] = array[1];
                                p[url] = array[0];
                            }
                        }
                        return statements;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
        public Disclosure(dynamic key, dynamic path)
        {
            security = new Security(key);
            string url;

            if (security.GrantAccess)
            {
                switch (Enum.ToObject(typeof(Path), path))
                {
                    case Path.Dart:
                        url = security.Disclosure;
                        break;

                    case Path.KRX:
                        url = security.KRX;
                        break;

                    case Path.Open:
                        url = security.Open;
                        break;

                    default:
                        return;
                }
                client = new RestClient(url)
                {
                    Timeout = -1,
                    UserAgent = security.Agent,
                    CookieContainer = new CookieContainer()
                };
                cookie = client.CookieContainer.GetCookieHeader(new Uri(url));
                source = new CancellationTokenSource();
            }
        }
        async Task<string> GetDocumentNumber(string code) => (await client.ExecuteAsync(new RestRequest(security.RequestSearchExistAll, Method.POST).AddHeader(security.ContentType, security.Form).AddParameter(security.Text, code), source.Token)).Content;
        readonly string cookie;
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly IRestClient client;
        const string format = "yyyyMMdd";
        const string cost = "매출원가";
        const string profit = "매출총이익";
        const string current = "유동자산";
        const string non = "비유동자산";
        const string operate = "영업활동현금흐름";
        const string invest = "투자활동현금흐름";
        const string finance = "재무활동현금흐름";
    }
}