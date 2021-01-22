using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using ShareInvest.Catalog;
using ShareInvest.Catalog.Models;

namespace ShareInvest.Client
{
	public sealed class API
	{
		public static API GetInstance(dynamic key)
		{
			if (Client == null)
				Client = new API(key);

			return Client;
		}
		public async Task<object> GetSecurityAsync(string security)
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(this.security.RequestTheIntegratedAddress(new Privacies { Security = security }), Method.GET), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					return JsonConvert.DeserializeObject<Privacies>(response.Content);
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return null;
		}
		public async Task<Retention> GetContextAsync(string param)
		{
			try
			{
				if (string.IsNullOrEmpty(param) is false)
				{
					var retention = JsonConvert.DeserializeObject<Retention>((await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.GET), source.Token)).Content);

					if (string.IsNullOrEmpty(retention.Code) is false && string.IsNullOrEmpty(retention.FirstDate) && string.IsNullOrEmpty(retention.LastDate))
						return new Retention
						{
							Code = param,
							FirstDate = string.Empty,
							LastDate = string.Empty
						};
					return retention;
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return new Retention
			{
				Code = null,
				LastDate = null
			};
		}
		public async Task<object> GetContextAsync<T>(T param) where T : struct
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.GET), source.Token);

				switch (param)
				{
					case Catalog.Models.Consensus when response.StatusCode.Equals(HttpStatusCode.OK):
						return JsonConvert.DeserializeObject<List<Catalog.Models.Consensus>>(response.Content);

					case Catalog.Strategics.RevisedStockPrice when response.StatusCode.Equals(HttpStatusCode.OK):
						return JsonConvert.DeserializeObject<Queue<Catalog.Strategics.ConfirmRevisedStockPrice>>(response.Content);

					case FinancialStatement:
						var list = JsonConvert.DeserializeObject<List<FinancialStatement>>(response.Content);
						var remove = new Queue<FinancialStatement>();
						var str = string.Empty;

						foreach (var fs in list.OrderBy(o => o.Date))
						{
							var date = fs.Date.Substring(0, 5);

							if (date.Equals(str))
								remove.Enqueue(fs);

							str = date;
						}
						while (remove.Count > 0)
						{
							var fs = remove.Dequeue();

							if (list.Remove(fs))
								Base.SendMessage(fs.Date, list.Count, param.GetType());
						}
						return list;

					case TrendsToCashflow:
						var stack = new Stack<Interface.IStrategics>();

						foreach (var content in JArray.Parse(response.Content))
							if (int.TryParse(content.Values().ToArray()[0].ToString(), out int analysis))
								stack.Push(new TrendsToCashflow
								{
									Code = string.Empty,
									Short = 5,
									Long = 0x3C,
									Trend = 0x14,
									Unit = 1,
									ReservationQuantity = 0,
									ReservationRevenue = 0xA,
									Addition = 0xB,
									Interval = 1,
									TradingQuantity = 1,
									PositionRevenue = 5.25e-3,
									PositionAddition = 7.25e-3,
									AnalysisType = Enum.GetName(typeof(AnalysisType), analysis)
								});
						return stack.OrderBy(o => Guid.NewGuid());

					case Catalog.IncorporatedStocks stocks:
						if (string.IsNullOrEmpty(stocks.Date))
						{
							var page = JsonConvert.DeserializeObject<int>(response.Content);

							if (response.StatusCode.Equals(HttpStatusCode.OK) && page < 0x16)
								return page;
						}
						else
							return JsonConvert.DeserializeObject<List<Catalog.IncorporatedStocks>>(response.Content);

						break;
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return null;
		}
		public async Task<object> GetContextAsync(Codes param, int length)
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param, length), Method.GET), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					return JsonConvert.DeserializeObject<List<Codes>>(response.Content);
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return null;
		}
		public async Task<int> PostContextAsync<T>(IEnumerable<T> param) where T : struct
		{
			try
			{
				string resource = string.Empty;

				switch (param)
				{
					case IEnumerable<Stocks>:
					case IEnumerable<Options>:
					case IEnumerable<Futures>:
						resource = security.RequestTheChangeOfAddress(param.GetType().GetGenericArguments()[0]);
						break;

					case IEnumerable<FinancialStatement>:
					case IEnumerable<Catalog.IncorporatedStocks>:
						resource = security.RequestTheIntegratedAddress(param.GetType().GetGenericArguments()[0].Name);
						break;

					case IEnumerable<ConvertConsensus>:
						resource = security.RequestTheIntegratedAddress(param.GetType().GetGenericArguments()[0].Name[7..]);
						break;

					default:
						return int.MinValue;
				}
				return (int)(await client.ExecuteAsync(new RestRequest(resource, Method.POST).AddHeader(Security.content_type, Security.json).AddParameter(Security.json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token)).StatusCode;
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return int.MinValue;
		}
		public async Task<object> PostContextAsync<T>(T param) where T : struct
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.POST).AddJsonBody(param, Security.content_type), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					switch (param)
					{
						case Privacies:
							return response.StatusCode;

						case Retention when string.IsNullOrEmpty(response.Content) is false:
							return JsonConvert.DeserializeObject<Retention>(response.Content);

						case Message:
						case Account:
						case Catalog.Models.RevisedStockPrice:
							return (int)response.StatusCode;

						case Stocks:
							return JsonConvert.DeserializeObject<string>(response.Content);
					}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return null;
		}
		public async Task<object> PutContextAsync<T>(T param) where T : struct
		{
			try
			{
				if (param is Privacies)
					return (await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param, Method.PUT), Method.PUT, DataFormat.Json).AddJsonBody(param, Security.content_type), source.Token)).StatusCode;

				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.PUT).AddJsonBody(param, Security.content_type), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					return param switch
					{
						StocksStrategics => JsonConvert.DeserializeObject<double>(response.Content),
						Catalog.Models.Consensus => (int)response.StatusCode,
						_ => JsonConvert.DeserializeObject<string>(response.Content),
					};
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return null;
		}
		public bool GrantAccess => security.GrantAccess;
		public bool IsAdministrator => security.IsAdministrator;
		static API Client
		{
			get; set;
		}
		API(dynamic key)
		{
			security = new Security(key);
			client = new RestClient(security.Url)
			{
				Timeout = -1
			};
			source = new CancellationTokenSource();
		}
		readonly CancellationTokenSource source;
		readonly Security security;
		readonly IRestClient client;
	}
}
///	<summary>
/// Base.IsDebug ? @"http://localhost:5528/" :
///	</summary>