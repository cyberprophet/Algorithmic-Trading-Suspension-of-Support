using System.Collections.Generic;

using Newtonsoft.Json;

using RestSharp;

using ShareInvest.Catalog;

namespace ShareInvest.Client
{
    public static class GoblinBatClient
    {
        static readonly Security security = new Security();
        static readonly RestClient client = new RestClient(security.Url)
        {
            Timeout = -1
        };
        public static object GetContext<T>(IParameters param)
        {
            var temp = client.Execute<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.GET));

            switch (param)
            {
                case Privacies _:
                    return JsonConvert.DeserializeObject<Privacies>(temp.Content);
            }
            return null;
        }
        public static object PostContext<T>(IParameters param)
        {
            var temp = client.Execute(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name), Method.POST).AddJsonBody(param, security.ContentType));

            switch (param)
            {
                case Privacies _:
                    return (int)temp.StatusCode;
            }
            return null;
        }
        public static object PostContext<T>(IEnumerable<T> param)
        {
            var temp = client.Execute(new RestRequest(string.Concat(security.CoreAPI, param.GetType().GetGenericArguments()[0].Name), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody));

            switch (param)
            {
                case IEnumerable<Charts> _:
                    return temp.IsSuccessful;
            }
            return null;
        }
        public static int PutContext<T>(IParameters param) => (int)client.Execute<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.PUT).AddJsonBody(param, security.ContentType)).StatusCode;
        public static int DeleteContext<T>(IParameters param) => (int)client.Execute<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.DELETE)).StatusCode;
    }
}