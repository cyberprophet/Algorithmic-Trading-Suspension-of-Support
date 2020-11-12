using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

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
            }
            return null;
        }
        public async Task<object> PostContextAsync<T>(Type type, T param) where T : struct
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(Security.RequestTheIntegratedAddress(param.GetType()), Method.POST).AddJsonBody(param, Security.content_type), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    switch (type.Namespace.Split('.')[1])
                    {
                        case "OpenAPI":
                            return JsonConvert.DeserializeObject<Catalog.OpenAPI.Order>(response.Content);
                    }
            }
            catch (Exception ex)
            {
                Base.SendMessage(GetType(), ex.StackTrace);
            }
            return null;
        }
        public async Task PutContextAsync<T>(T param) where T : struct
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.GrantAccess ? security.RequestTheIntegratedAddress(param) : Security.RequestTheIntegratedAddress(param.GetType()), Method.PUT).AddJsonBody(param, Security.content_type), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    Base.SendMessage(GetType(), JsonConvert.DeserializeObject<string>(response.Content));
            }
            catch (Exception ex)
            {
                Base.SendMessage(GetType(), ex.StackTrace);
            }
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