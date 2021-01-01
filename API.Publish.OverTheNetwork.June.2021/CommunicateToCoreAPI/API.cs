using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

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
		public async Task<object> PostContextAsync<T>(T param) where T : struct
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param, Method.POST))
					.AddJsonBody(param, Security.content_type), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					switch (param)
					{
						case Privacies:
							return response.StatusCode;

						case Retention when string.IsNullOrEmpty(response.Content) is false:
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
		public async Task<object> PutContextAsync<T>(T param) where T : struct
		{
			try
			{
				if (param is Privacies)
					return (await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param, Method.PUT), Method.PUT, DataFormat.Json)
					   .AddJsonBody(param, Security.content_type), source.Token)).StatusCode;

				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.PUT)
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
		public bool GrantAccess => security.GrantAccess;
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