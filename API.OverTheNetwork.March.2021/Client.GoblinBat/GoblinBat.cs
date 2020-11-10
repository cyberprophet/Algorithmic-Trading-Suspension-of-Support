using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

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
        public async Task<int> PutContextAsync<T>(T param) where T : struct
        {
            var status = int.MaxValue;

            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.GrantAccess ? security.RequestTheIntegratedAddress(param) : security.RequestTheIntegratedAddress(param.GetType()), Method.PUT).AddJsonBody(param, security.ContentType), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    SendMessage(JsonConvert.DeserializeObject<string>(response.Content));

                    return int.MaxValue;
                }
                status = (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return status;
        }
        [Conditional("DEBUG")]
        void SendMessage(object message)
        {
            switch (message)
            {
                case string _:
                    Debug.WriteLine(message);
                    return;
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