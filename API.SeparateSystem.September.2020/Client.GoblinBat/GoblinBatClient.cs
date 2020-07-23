using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

using ShareInvest.Catalog;

namespace ShareInvest.Client
{
    public sealed class GoblinBatClient
    {
        public static GoblinBatClient GetInstance() => Client;
        public static GoblinBatClient GetInstance(dynamic key)
        {
            if (Client == null && new Security(key).GrantAccess)
                Client = new GoblinBatClient(key);

            return Client;
        }
        public async Task<object> GetContext<T>(IParameters param)
        {
            var temp = await client.ExecuteAsync<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.GET), source.Token);

            switch (param)
            {
                case Privacies _:
                    return JsonConvert.DeserializeObject<Privacies>(temp.Content);
            }
            return null;
        }
        public async Task<object> GetContext(Codes param, int length) => JsonConvert.DeserializeObject<List<Codes>>((await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", length), Method.GET), source.Token)).Content);
        public async Task<Retention> GetContext<T>(T param) where T : struct, ICharts => JsonConvert.DeserializeObject<Retention>((await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name), Method.GET), source.Token)).Content);
        public async Task<Retention> PostContext<T>(Queue<T> param) where T : struct, ICharts => JsonConvert.DeserializeObject<Retention>((await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().GetGenericArguments()[0].Name), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token)).Content);
        public async Task<object> PostContext<T>(IParameters param)
        {
            var temp = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name), Method.POST).AddJsonBody(param, security.ContentType), source.Token);

            switch (param)
            {
                case Privacies _:
                    return (int)temp.StatusCode;
            }
            return null;
        }
        public async Task<object> PostContext<T>(IEnumerable<T> param)
        {
            var temp = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().GetGenericArguments()[0].Name), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

            switch (param)
            {
                case IEnumerable<Codes> _:
                    return temp.IsSuccessful;
            }
            return null;
        }
        public async Task<int> PutContext<T>(Codes param) => (int)(await client.ExecuteAsync<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Code), Method.PUT).AddJsonBody(param, security.ContentType), source.Token)).StatusCode;
        public async Task<int> PutContext<T>(IParameters param) => (int)(await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.PUT, DataFormat.Json).AddJsonBody(param, security.ContentType), source.Token)).StatusCode;
        public async Task<int> DeleteContext<T>(IParameters param) => (int)(await client.ExecuteAsync<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.DELETE), source.Token)).StatusCode;
        static GoblinBatClient Client
        {
            get; set;
        }
        GoblinBatClient(dynamic key)
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