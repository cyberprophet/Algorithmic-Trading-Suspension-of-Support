using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

using ShareInvest.Catalog;
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
        public async Task<Retention> GetContextAsync(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code) == false)
                {
                    var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(code), Method.GET), source.Token);
                    var retention = JsonConvert.DeserializeObject<Retention>(response.Content);

                    if (string.IsNullOrEmpty(retention.Code) == false && string.IsNullOrEmpty(retention.FirstDate) && string.IsNullOrEmpty(retention.LastDate))
                        return new Retention
                        {
                            Code = code,
                            FirstDate = string.Empty,
                            LastDate = string.Empty
                        };
                    return retention;
                }
            }
            catch (Exception ex)
            {
                Base.SendMessage(GetType(), ex.StackTrace);
            }
            return new Retention
            {
                Code = null,
                LastDate = null
            };
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
        public async Task<object> GetContextAsync<T>(T param) where T : struct
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestTheIntegratedAddress(param), Method.GET), source.Token);

                switch (param)
                {
                    case Catalog.IncorporatedStocks stocks:
                        if (string.IsNullOrEmpty(stocks.Date))
                        {
                            var page = JsonConvert.DeserializeObject<int>(response.Content);

                            if (response.StatusCode.Equals(HttpStatusCode.OK) && page < 0x16)
                                return page;
                        }
                        else
                            return JsonConvert.DeserializeObject<List<Catalog.IncorporatedStocks>>(response.Content);

                        break;
                }
            }
            catch (Exception ex)
            {
                Base.SendMessage(GetType(), ex.StackTrace);
            }
            return null;
        }
        public async Task<object> PostContextAsync<T>(T param) where T : struct
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(Security.RequestTheIntegratedAddress(param.GetType()), Method.POST).AddJsonBody(param, Security.content_type), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    switch (param)
                    {
                        case Retention:
                            return JsonConvert.DeserializeObject<Retention>(response.Content);
                    }
            }
            catch (Exception ex)
            {
                Base.SendMessage(GetType(), ex.StackTrace);
            }
            return null;
        }
        public async Task<int> PostContextAsync<T>(IEnumerable<T> param) where T : struct
        {
            try
            {
                string resource = string.Empty;

                switch (param)
                {
                    case IEnumerable<Stocks>:
                    case IEnumerable<Options>:
                    case IEnumerable<Futures>:
                        var address = param.GetType().GetGenericArguments()[0];
                        resource = security.GrantAccess ? security.RequestTheChangeOfAddress(address) : Security.RequestTheIntegratedAddress(address);
                        break;

                    case IEnumerable<FinancialStatement>:
                    case IEnumerable<Catalog.IncorporatedStocks>:
                        resource = security.RequestTheIntegratedAddress(param.GetType().GetGenericArguments()[0].Name);
                        break;

                    case IEnumerable<ConvertConsensus>:
                        resource = security.RequestTheIntegratedAddress(param.GetType().GetGenericArguments()[0].Name[7..]);
                        break;

                    default:
                        return int.MinValue;
                }
                return (int)(await client.ExecuteAsync(new RestRequest(resource, Method.POST)
                    .AddHeader(Security.content_type, Security.json)
                    .AddParameter(Security.json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token)).StatusCode;
            }
            catch (Exception ex)
            {
                Base.SendMessage(GetType(), ex.StackTrace);
            }
            return int.MinValue;
        }
        public async Task<object> PutContextAsync<T>(T param) where T : struct
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.GrantAccess ? security.RequestTheIntegratedAddress(param) : Security.RequestTheIntegratedAddress(param.GetType()), Method.PUT).AddJsonBody(param, Security.content_type), source.Token);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<string>(response.Content);
            }
            catch (Exception ex)
            {
                Base.SendMessage(GetType(), ex.StackTrace);
            }
            return null;
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