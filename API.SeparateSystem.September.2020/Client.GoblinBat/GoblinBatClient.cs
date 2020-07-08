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
        public static object GetContext<T>(IParameters param) => client.Execute<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.GET)).Data;
        public static int PostContext<T>(IParameters param) => (int)client.Execute(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name), Method.POST).AddJsonBody(param, security.ContentType)).StatusCode;
        public static int PutContext<T>(IParameters param) => (int)client.Execute<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.PUT).AddJsonBody(param, security.ContentType)).StatusCode;
        public static int DeleteContext<T>(IParameters param) => (int)client.Execute<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.DELETE)).StatusCode;
    }
}