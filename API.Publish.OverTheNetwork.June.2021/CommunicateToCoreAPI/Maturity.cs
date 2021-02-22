using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Client
{
	public class Maturity
	{
		public async Task<object> PutContextAsync(Codes model)
		{
			try
			{
				var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(model), Method.PUT).AddJsonBody(model, Security.content_type), source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					return (int)response.StatusCode;
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
			}
			return null;
		}
		public Maturity(dynamic param)
		{
			security = new Security(param);
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