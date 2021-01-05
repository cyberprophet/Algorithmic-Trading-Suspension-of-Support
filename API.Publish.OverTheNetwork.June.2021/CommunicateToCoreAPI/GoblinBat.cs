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
		static GoblinBat Client
		{
			get; set;
		}
		public async Task<object> GetContextAsync(string security)
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
					else if (string.IsNullOrEmpty(request.Item1) == false)
					{
						var response = await client.ExecuteAsync(new RestRequest(request.Item1, Method.GET), source.Token);

						if (chart.End.Length == 6 && chart.End.CompareTo(DateTime.Now.AddDays(-1).ToString("yyMMdd")) < 0 || chart.End.Length < 6)
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
					Base.SendMessage(chart.Code, ex.StackTrace, param.GetType());
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
				var response = await client.ExecuteAsync(new RestRequest(Security.RequestTheIntegratedAddress(param.GetType()), Method.POST).AddJsonBody(param, Security.content_type), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					switch (param)
					{
						case Privacies:
							return response.StatusCode;

						case Retention when string.IsNullOrEmpty(response.Content) is false:
							return JsonConvert.DeserializeObject<Retention>(response.Content);

						case Message:
						case Account:
							if (Base.IsDebug)
								Base.SendMessage(GetType(), response.Content);

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
		public string Url => security.Uri;
		GoblinBat(dynamic key)
		{
			security = new Security(key);
			client = new RestClient(security.Uri)
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