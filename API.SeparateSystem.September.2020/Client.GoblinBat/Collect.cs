using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

namespace ShareInvest.Client
{
    public class Collect
    {
        public async Task<string> PostContextAsync(Queue<Catalog.Request.Collect> storage, string code)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestToCollect(storage.Peek(), code), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(storage), ParameterType.RequestBody), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<string>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<string> PostContextAsync(string code, Stack<Catalog.Request.Collect> storage)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestToCollect(code, storage.Peek()), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(storage), ParameterType.RequestBody), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<string>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<string> GetContextAsync(Catalog.Request.Collect storage)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestToCollect(storage), Method.GET), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<string>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public Collect(string access)
        {
            security = new Security(0);
            client = new RestClient(access)
            {
                Timeout = -1
            };
            source = new CancellationTokenSource();
        }
        [Conditional("DEBUG")]
        void SendMessage(object message) => Console.WriteLine(message);
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly IRestClient client;
    }
}