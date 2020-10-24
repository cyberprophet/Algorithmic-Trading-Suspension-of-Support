using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

namespace ShareInvest.Client
{
    public class Collect
    {
        public async Task<int> PostContextAsync(StringBuilder data, StringBuilder date, string code)
        {
            try
            {
                string[] key = date.ToString().Split('|'), value = data.ToString().Split('|');
                var param = new Dictionary<string, string>();

                for (int i = 0; i < key.Length; i++)
                    param[key[i]] = value[i];

                var response = await client.ExecuteAsync(new RestRequest(security.RequestToCollect(code), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return param.Count;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return int.MinValue;
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
        void SendMessage(string message) => Console.WriteLine(message);
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly IRestClient client;
    }
}