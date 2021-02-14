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
		public async Task<Stack<Catalog.IncorporatedStocks>> GetConstituentStocks(int market, DateTime now)
		{
			try
			{
				var stack = new Stack<Catalog.IncorporatedStocks>();
				var response = await client.ExecuteAsync(new RestRequest(Security.Contents[3], Method.POST).AddHeader(Security.content_type, Security.Contents[4]).AddParameter(Security.Contents[5], market).AddParameter(Security.Contents[6], security.GetMarket(market)).AddParameter(Security.Contents[7], Security.Contents[8]).AddParameter(Security.Contents[9], now.ToString(format)).AddParameter(Security.Contents[0xA], (await client.ExecuteAsync(new RestRequest(Security.Contents[0xB], Method.GET))).Content), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
				{
					foreach (var str in JsonConvert.DeserializeObject<List<Catalog.Dart.MKD99000001>>(JObject.Parse(response.Content)[Security.Contents[0xC]].ToString()))
						if (ulong.TryParse(str.Capitalization.Replace(",", string.Empty), out ulong capitalization))
							stack.Push(new Catalog.IncorporatedStocks
							{
								Code = str.Code,
								Name = str.Name,
								Date = now.ToString(format[2..]),
								Market = market == 1 ? 'P' : 'Q',
								Capitalization = (int)(capitalization / 0x5F5E100)
							});
					return stack;
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return null;
		}
		public ConstituentStocks(dynamic key)
		{
			security = new Security(key);

			if (security.GrantAccess)
			{
				client = new RestClient(Security.Contents[0])
				{
					Timeout = -1,
					UserAgent = Security.Contents[1],
					CookieContainer = new CookieContainer()
				};
				cookie = client.CookieContainer.GetCookieHeader(new Uri(Security.Contents[2]));
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