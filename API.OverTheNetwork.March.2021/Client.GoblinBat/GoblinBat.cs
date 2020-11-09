using System.Threading;

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

        }
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly IRestClient client;
    }
}