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
	public sealed class API
	{
		public static API GetInstance(dynamic key)
		{
			if (Client is null)
				Client = new API(key);

			return Client;
		}
		[SupportedOSPlatform("windows")]
		public async Task<object> GetChartsAsync<T>(T param) where T : struct
		{
			if (param is Charts chart)
				try
				{
					var request = security.RequestCharts(param);

					if (request.Item2)
					{
						if (Array.Exists(chart.Start.ToCharArray(), o => char.IsLetter(o)) && Array.Exists(chart.End.ToCharArray(), o => char.IsLetter(o)))
							return JsonConvert.DeserializeObject<string>(request.Item1);

						else
							return JsonConvert.DeserializeObject<IEnumerable<Catalog.Strategics.Charts>>(request.Item1);
					}
					else if (string.IsNullOrEmpty(request.Item1) is false)
					{
						var response = await client.ExecuteAsync(new RestRequest(request.Item1, Method.GET), source.Token);

						if (chart.End.Length == 6 && chart.End.CompareTo(DateTime.Now.AddDays(-1).ToString(Base.DateFormat)) < 0 || chart.End.Length < 6)
						{
							var save = Security.Save(chart);
							Repository.Save(save.Item1, save.Item2, response.Content);
						}
						if (Array.Exists(chart.Start.ToCharArray(), o => char.IsLetter(o)) && Array.Exists(chart.End.ToCharArray(), o => char.IsLetter(o)))
							return JsonConvert.DeserializeObject<string>(response.Content);

						else
							return JsonConvert.DeserializeObject<IEnumerable<Catalog.Strategics.Charts>>(response.Content);
					}
				}
				catch (Exception ex)
				{
					Base.SendMessage(param.GetType(), chart.Code, chart.Start, ex.StackTrace);
				}
			return null;
		}
		public async Task<object> GetStrategics(string security)
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(this.security.RequestTheIntegratedAddress(security), Method.GET), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					return JsonConvert.DeserializeObject<IEnumerable<BringIn>>(response.Content);
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
			}
			return null;
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
			}
			return null;
		}
		public async Task<object> GetConfirmAsync<T>(T param) where T : struct
		{
			try
			{
				string address;

				switch (param)
				{
					case Tick tick:
						address = System.IO.Path.Combine(tick.Code, tick.Date, tick.Open, tick.Close);
						break;

					case GroupDetail detail:
						address = detail.Code;
						break;

					case Catalog.Dart.Theme:
						address = string.Empty;
						break;

					default:
						return null;
				}
				var response = await client.ExecuteAsync(new RestRequest(Security.RequestTheIntegratedAddress(address, param), Method.GET), source.Token);

				switch (param)
				{
					case GroupDetail:
						return HttpStatusCode.OK.Equals(response.StatusCode) ? JsonConvert.DeserializeObject<string>(response.Content) : string.Empty;

					case Tick when HttpStatusCode.NoContent.Equals(response.StatusCode):
						return param;

					case Catalog.Dart.Theme when HttpStatusCode.OK.Equals(response.StatusCode):
						return JsonConvert.DeserializeObject<List<Catalog.Models.Theme>>(response.Content);
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
			}
			return null;
		}
		public async Task<object> GetContextAsync<T>(T type, string param) where T : struct
		{
			try
			{
				if (string.IsNullOrEmpty(param) is false)
				{
					var response = await client.ExecuteAsync(new RestRequest(Security.RequestTheIntegratedAddress(param, type), Method.GET), source.Token);

					if (HttpStatusCode.OK.Equals(response.StatusCode))
						switch (type)
						{
							case Tick:
								return JsonConvert.DeserializeObject<Stack<Tick>>(response.Content).OrderBy(o => o.Date);
						}
				}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
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
			}
			return new Retention
			{
				Code = null,
				LastDate = null
			};
		}
		[SupportedOSPlatform("windows")]
		public async Task<object> GetContextAsync<T>(T param) where T : struct
		{
			try
			{
				if (param is Charts && Repository.RetrieveSavedMaterial(param) is string file)
					return JsonConvert.DeserializeObject<Stack<Catalog.Strategics.Charts>>(file).OrderBy(o => o.Date);

				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.GET), source.Token);

				switch (param)
				{
					case Catalog.Strategics.Charts when HttpStatusCode.OK.Equals(response.StatusCode) && string.IsNullOrEmpty(response.Content) is false:
						return JsonConvert.DeserializeObject<Tick>(response.Content);

					case Charts charts when HttpStatusCode.OK.Equals(response.StatusCode):
						Repository.KeepOrganizedInStorage(response.Content, charts);

						return JsonConvert.DeserializeObject<Stack<Catalog.Strategics.Charts>>(response.Content).OrderBy(o => o.Date);

					case Catalog.Models.Consensus when response.StatusCode.Equals(HttpStatusCode.OK):
						return JsonConvert.DeserializeObject<List<Catalog.Models.Consensus>>(response.Content);

					case Catalog.Strategics.RevisedStockPrice when response.StatusCode.Equals(HttpStatusCode.OK):
						return JsonConvert.DeserializeObject<Queue<Catalog.Strategics.ConfirmRevisedStockPrice>>(response.Content);

					case Tick when response.StatusCode.Equals(HttpStatusCode.OK) && string.IsNullOrEmpty(response.Content) is false:
						return JsonConvert.DeserializeObject<Tick>(response.Content);

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
			}
			return null;
		}
		public async Task<object> PostConfirmAsync<T>(T param) where T : struct
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.POST).AddHeader(Security.content_type, Security.json).AddParameter(Security.json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

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
				Base.SendMessage(GetType(), ex.StackTrace);
			}
			return null;
		}
		public async Task<object> PostContextAsync(Catalog.Models.Theme theme)
		{
			try
			{
				var now = DateTime.Now;
				var param = new Catalog.Dart.Theme
				{
					Index = theme.Index,
					Name = theme.Name,
					Code = string.Concat(theme.Code[0] ?? string.Empty, ';', theme.Code[^1] ?? string.Empty),
					Rate = theme.Rate * 1e-2,
					Average = theme.Average * 1e-2,
					Date = now.AddDays(now.Hour < 0x12 ? -1 : 0).ToString(Base.DateFormat)
				};
				if (HttpStatusCode.OK.Equals((await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(theme.GetType().Name), Method.POST).AddJsonBody(param, Security.content_type), source.Token)).StatusCode))
					return param;
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
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

						case GroupDetail when string.IsNullOrEmpty(response.Content) is false:
							return JsonConvert.DeserializeObject<int>(response.Content);

						case Retention when string.IsNullOrEmpty(response.Content) is false:
							return JsonConvert.DeserializeObject<Retention>(response.Content);

						case Tick or Message or Account or Catalog.Models.RevisedStockPrice:
							return (int)response.StatusCode;

						case Stocks:
							return JsonConvert.DeserializeObject<string>(response.Content);
					}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
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
			}
			return null;
		}
		public bool GrantAccess => security.GrantAccess;
		public bool IsServer => security.IsServer;
		public bool IsInsider => security.IsInsiders;
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