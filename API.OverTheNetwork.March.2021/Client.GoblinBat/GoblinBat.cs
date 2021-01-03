using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using ShareInvest.Catalog;
using ShareInvest.Catalog.Models;

namespace ShareInvest.Client
{
	public sealed class GoblinBat
	{
		public static GoblinBat GetInstance(dynamic key)
		{
			if (Client == null)
				Client = new GoblinBat(key);

			return Client;
		}
		public static GoblinBat GetInstance()
		{
			if (Client == null)
				Client = new GoblinBat();

			return Client;
		}
		static GoblinBat Client
		{
			get; set;
		}
		public async Task<Retention> GetContextAsync(string param)
		{
			try
			{
				if (string.IsNullOrEmpty(param) == false)
				{
					var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.GET), source.Token);
					var retention = JsonConvert.DeserializeObject<Retention>(response.Content);

					if (string.IsNullOrEmpty(retention.Code) == false && string.IsNullOrEmpty(retention.FirstDate) && string.IsNullOrEmpty(retention.LastDate))
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
		public async Task<object> GetContextAsync(string[] security)
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(this.security.RequestTheIntegratedAddress(new Privacies { Security = security[0] }), Method.GET), source.Token);

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
		[SupportedOSPlatform("windows")]
		public async Task<object> GetChartsAsync<T>(T param) where T : struct
		{
			if (param is Charts chart)
				try
				{
					var request = security.RequestCharts(param);
					object result = null;

					if (request.Item2)
					{
						if (Array.Exists(chart.Start.ToCharArray(), o => char.IsLetter(o)) && Array.Exists(chart.End.ToCharArray(), o => char.IsLetter(o)))
							result = JsonConvert.DeserializeObject<string>(request.Item1);

						else
							result = JsonConvert.DeserializeObject<IEnumerable<Catalog.Strategics.Charts>>(request.Item1);
					}
					else if (string.IsNullOrEmpty(request.Item1) == false)
					{
						var response = await client.ExecuteAsync(new RestRequest(request.Item1, Method.GET), source.Token);

						if (Array.Exists(chart.Start.ToCharArray(), o => char.IsLetter(o)) && Array.Exists(chart.End.ToCharArray(), o => char.IsLetter(o)))
							result = JsonConvert.DeserializeObject<string>(response.Content);

						else
							result = JsonConvert.DeserializeObject<IEnumerable<Catalog.Strategics.Charts>>(response.Content);

						if (chart.End.Length == 6 && chart.End.CompareTo(DateTime.Now.AddDays(-1).ToString("yyMMdd")) < 0 || chart.End.Length < 6)
						{
							var save = Security.Save(chart);
							Repository.Save(save.Item1, save.Item2, response.Content);
						}
					}
					return result;
				}
				catch (Exception ex)
				{
					Base.SendMessage(chart.Code, ex.StackTrace, param.GetType());
					Repository.Delete(Security.Save(chart).Item2);
				}
			return null;
		}
		public async Task<object> PostConfirmAsync<T>(T param) where T : struct
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.POST)
					.AddHeader(Security.content_type, Security.json)
					.AddParameter(Security.json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					switch (param)
					{
						case ConfirmStrategics:
							return JsonConvert.DeserializeObject<bool>(response.Content);

						case Files:
							if (response.RawBytes != null && response.RawBytes.Length > 0)
								return JsonConvert.DeserializeObject<Files>(response.Content);

							break;
					}
			}
			catch (Exception ex)
			{
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return null;
		}
		public async Task<object> PostContextAsync<T>(T param) where T : struct
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(security.GrantAccess ? security.RequestTheIntegratedAddress(param, Method.POST) : Security.RequestTheIntegratedAddress(param.GetType()), Method.POST)
					.AddJsonBody(param, Security.content_type), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					switch (param)
					{
						case Privacies:
							return response.StatusCode;

						case Retention when string.IsNullOrEmpty(response.Content) == false:
							return JsonConvert.DeserializeObject<Retention>(response.Content);

						case Message:
						case Account:
							return (int)response.StatusCode;
					}
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
						var address = param.GetType().GetGenericArguments()[0];
						resource = security.GrantAccess ? security.RequestTheChangeOfAddress(address) : Security.RequestTheIntegratedAddress(address);
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
				return (int)(await client.ExecuteAsync(new RestRequest(resource, Method.POST)
					.AddHeader(Security.content_type, Security.json)
					.AddParameter(Security.json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token)).StatusCode;
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
				Base.SendMessage(ex.StackTrace, GetType());
			}
			return int.MinValue;
		}
		public async Task<object> PutContextAsync(Type type, Dictionary<string, string> param)
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(Security.RequestTheIntegratedAddress(type), Method.PUT)
					.AddHeader(Security.content_type, Security.json)
					.AddParameter(Security.json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
					return (int)response.StatusCode;
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
					return (await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param, Method.PUT), Method.PUT, DataFormat.Json)
					   .AddJsonBody(param, Security.content_type), source.Token)).StatusCode;

				var response = await client.ExecuteAsync(new RestRequest(security.GrantAccess ?
					security.RequestTheIntegratedAddress(param) : Security.RequestTheIntegratedAddress(param.GetType()), Method.PUT)
					.AddJsonBody(param, Security.content_type), source.Token);

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
		GoblinBat()
		{
			security = new Security(int.MinValue);
			client = new RestClient(security.Url)
			{
				Timeout = -1
			};
			source = new CancellationTokenSource();
		}
		GoblinBat(dynamic key)
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