using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

namespace ShareInvest.Client
{
    public class ConstituentStocks
    {
        public async Task<Stack<Catalog.Request.IncorporatedStocks>> GetConstituentStocks(int market, DateTime now)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestMKD99000001, Method.POST).AddHeader(security.ContentType, security.Form).AddParameter(security.CD[0], market).AddParameter(security.CD[1], security.GetMarket(market)).AddParameter(security.Language[0], security.Language[1]).AddParameter(security.Param[2], now.ToString(format)).AddParameter(security.Param[3], (await client.ExecuteAsync(new RestRequest(security.RequestOTP, Method.GET))).Content), source.Token);
                var stack = new Stack<Catalog.Request.IncorporatedStocks>();

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    foreach (var str in JsonConvert.DeserializeObject<List<Catalog.KRX.MKD99000001>>(JObject.Parse(response.Content)[security.OutPut].ToString()))
                        if (ulong.TryParse(str.Capitalization.Replace(",", string.Empty), out ulong capitalization))
                            stack.Push(new Catalog.Request.IncorporatedStocks
                            {
                                Code = str.Code,
                                Name = str.Name,
                                Date = now.ToString(format.Substring(2)),
                                Market = market == 1 ? 'P' : 'Q',
                                Capitalization = (int)(capitalization / 0x5F5E100)
                            });
                    return stack;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
        public ConstituentStocks(dynamic key)
        {
            security = new Security(key);

            if (security.GrantAccess)
            {
                client = new RestClient(security.Contents)
                {
                    Timeout = -1,
                    UserAgent = security.Agent,
                    CookieContainer = new CookieContainer()
                };
                cookie = client.CookieContainer.GetCookieHeader(new Uri(security.KRX));
                source = new CancellationTokenSource();
            }
        }
        const string format = "yyyyMMdd";
        readonly string cookie;
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly IRestClient client;
    }
}