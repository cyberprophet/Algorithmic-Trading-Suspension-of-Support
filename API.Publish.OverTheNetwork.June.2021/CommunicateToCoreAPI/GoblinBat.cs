using Newtonsoft.Json;

using RestSharp;

using ShareInvest.Catalog;
using ShareInvest.Catalog.Models;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ShareInvest.Client
{
	public sealed class GoblinBat
	{
		public static GoblinBat GetInstance(dynamic key)
		{
			if (Client is null)
				Client = new GoblinBat(key);

			return Client;
		}
		static GoblinBat Client
		{
			get; set;
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
							if (response.RawBytes is not null && response.RawBytes.Length > 0)
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
		public async Task<object> PostContextAsync<T>(T param) where T : struct
		{
			try
			{
				var request = new RestRequest(Security.RequestTheIntegratedAddress(param.GetType()), Method.POST);
				var body = param is Balance or Files ? request.AddHeader(Security.content_type, Security.json).AddParameter(Security.json, JsonConvert.SerializeObject(param), ParameterType.RequestBody) : request.AddJsonBody(param, Security.content_type);
				var response = await client.ExecuteAsync(body, source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					switch (param)
					{
						case Privacies:
							return response.StatusCode;

						case Retention when string.IsNullOrEmpty(response.Content) is false:
							return JsonConvert.DeserializeObject<Retention>(response.Content);

						case Message or Account or Balance or Files:
							if (Base.IsDebug)
								Base.SendMessage(GetType(), response.Content);

							return (int)response.StatusCode;
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
				var response = await client.ExecuteAsync(new RestRequest(Security.RequestTheIntegratedAddress(param.GetType()), Method.PUT).AddHeader(Security.content_type, Security.json).AddParameter(Security.json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

				if (HttpStatusCode.OK.Equals(response.StatusCode))
					switch (param)
					{
						case Files:
							return (int)response.StatusCode;
					}
			}
			catch (Exception ex)
			{
				Base.SendMessage(param.GetType(), ex.StackTrace);
			}
			return null;
		}
		///	<summary>
		/// Base.IsDebug ? @"https://localhost:44393/" :
		///	</summary>
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