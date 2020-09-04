using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;

namespace ShareInvest.Client
{
    public class Disclosure
    {
        public async Task GetDisclosureInformation(string code, string name) => Process.Start(security.GetUrl(await GetDocumentNumber(code), name)); public Disclosure(dynamic key)
        {
            security = new Security(key);

            if (security.GrantAccess)
            {
                client = new RestClient(security.Disclosure)
                {
                    Timeout = -1,
                    UserAgent = security.Agent,
                    CookieContainer = new CookieContainer()
                };
                cookie = client.CookieContainer.GetCookieHeader(new Uri(security.Disclosure));
                source = new CancellationTokenSource();
            }
        }
        async Task<string> GetDocumentNumber(string code) => (await client.ExecuteAsync(new RestRequest(security.RequestSearchExistAll, Method.POST).AddHeader(security.ContentType, security.Form).AddParameter(security.Text, code), source.Token)).Content;
        readonly string cookie;
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly IRestClient client;
    }
}