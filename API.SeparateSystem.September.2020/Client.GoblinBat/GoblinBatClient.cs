using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using ShareInvest.Catalog;
using ShareInvest.Interface;

namespace ShareInvest.Client
{
    public sealed class GoblinBatClient
    {
        public static GoblinBatClient GetInstance() => Client;
        public static GoblinBatClient GetInstance(dynamic key)
        {
            if (Client == null && new Security(key).GrantAccess)
            {
                Client = new GoblinBatClient(key);
            }
            return Client;
        }
        public static double Coin
        {
            get; private set;
        }
        public async Task<int> EmergencyContext<T>(Codes param) => (int)(await client.ExecuteAsync<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Code), Method.PUT).AddJsonBody(param, security.ContentType), source.Token)).StatusCode;
        public async Task<Retention> EmergencyContext<T>(Queue<T> param) where T : struct => JsonConvert.DeserializeObject<Retention>((await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().GetGenericArguments()[0].Name), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token)).Content);
        public async Task<object> GetContext(Catalog.Request.Charts chart)
        {
            var response = await client.ExecuteAsync(new RestRequest(security.RequestCharts(chart), Method.GET), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                if (Array.Exists(chart.Start.ToCharArray(), o => char.IsLetter(o)) && Array.Exists(chart.End.ToCharArray(), o => char.IsLetter(o)))
                    return JsonConvert.DeserializeObject<string>(response.Content);

                else
                    return JsonConvert.DeserializeObject<IEnumerable<Charts>>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
                SendMessage(chart);
            }
            return null;
        }
        public async Task<Codes> GetContext(Codes codes)
        {
            var response = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, codes.GetType().Name, "/", codes.Code), Method.GET), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return JsonConvert.DeserializeObject<Codes>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
            }
            return codes;
        }
        public async Task<object> GetContext(Codes param, int length)
        {
            var response = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", length), Method.GET), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return JsonConvert.DeserializeObject<List<Codes>>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
            }
            return null;
        }
        public async Task<long> GetContext()
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestCount(), Method.GET), source.Token);

                if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return JsonConvert.DeserializeObject<long>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return long.MinValue;
        }
        public async Task<object> GetContext<T>(IParameters param)
        {
            var response = await client.ExecuteAsync<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.GET), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                switch (param)
                {
                    case Privacies _:
                        return JsonConvert.DeserializeObject<Privacies>(response.Content);
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
            }
            return null;
        }
        public async Task<Retention> GetContext<T>(T param) where T : struct, ICharts
        {
            var response = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name), Method.GET), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return JsonConvert.DeserializeObject<Retention>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
            }
            return new Retention
            {
                Code = string.Empty,
                LastDate = string.Empty
            };
        }
        public async Task<Retention> GetContext(Retention retention)
        {
            try
            {
                if (string.IsNullOrEmpty(retention.Code) == false)
                {
                    var response = await client.ExecuteAsync(new RestRequest(security.RequestCode(retention), Method.GET), source.Token);

                    if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                    {
                        Coin += security.GetSettleTheFare(response.RawBytes.Length);
                        SendMessage(Coin);
                    }
                    return JsonConvert.DeserializeObject<Retention>(response.Content);
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return new Retention
            {
                Code = null,
                LastDate = null
            };
        }
        public async Task<Retention> GetContext(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code) == false)
                {
                    var response = await client.ExecuteAsync(new RestRequest(security.RequestCode(code), Method.GET), source.Token);

                    if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                    {
                        Coin += security.GetSettleTheFare(response.RawBytes.Length);
                        SendMessage(Coin);
                    }
                    return JsonConvert.DeserializeObject<Retention>(response.Content);
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return new Retention
            {
                Code = null,
                LastDate = null
            };
        }
        public async Task<IEnumerable<IStrategics>> GetContext(IStrategics strategics)
        {
            var stack = new Stack<IStrategics>();

            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestStrategics(strategics), Method.GET), source.Token);

                if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                switch (strategics)
                {
                    case TrendsInStockPrices _:
                        foreach (var content in JArray.Parse(response.Content))
                        {
                            var param = content.Values().ToArray();

                            if (int.TryParse(param[5].ToString(), out int unit) && double.TryParse(param[4].ToString(), out double purchase) && double.TryParse(param[3].ToString(), out double profit) && int.TryParse(param[2].ToString(), out int trend) && int.TryParse(param[1].ToString(), out int jLong) && int.TryParse(param[0].ToString(), out int jShort) && char.TryParse(param[8].ToString(), out char setting) && char.TryParse(param[6].ToString(), out char ls) && char.TryParse(param[7].ToString(), out char type))
                                stack.Push(new TrendsInStockPrices
                                {
                                    Code = string.Empty,
                                    Short = jShort,
                                    Long = jLong,
                                    Trend = trend,
                                    RealizeProfit = profit,
                                    AdditionalPurchase = purchase,
                                    Quantity = 1,
                                    QuoteUnit = unit,
                                    LongShort = (LongShort)ls,
                                    TrendType = (Trend)type,
                                    Setting = (Setting)setting
                                });
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return stack.OrderBy(o => random.Next(stack.Count));
        }
        public async Task<bool> PostContext(ConfirmStrategics confirm)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestConfirm(confirm), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(confirm), ParameterType.RequestBody), source.Token);

                if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return JsonConvert.DeserializeObject<bool>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return false;
        }
        public async Task<int> PostContext(Stack<Catalog.OpenAPI.Days> param)
        {
            int code = int.MinValue;

            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestCode(), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                code = (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return code;
        }
        public async Task<Retention> PostContext<T>(string code, Queue<T> param) where T : struct, ICharts
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().GetGenericArguments()[0].Name), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return await GetContext(code);
        }
        public async Task<Tuple<int, double>> PostContext<T>(IParameters param)
        {
            var response = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name), Method.POST).AddJsonBody(param, security.ContentType), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                switch (param)
                {
                    case Privacies _:
                        return new Tuple<int, double>((int)response.StatusCode, JsonConvert.DeserializeObject<double>(response.Content));
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
            }
            return new Tuple<int, double>((int)response.StatusCode, Coin);
        }
        public async Task<object> PostContext<T>(IEnumerable<T> param)
        {
            var response = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().GetGenericArguments()[0].Name), Method.POST).AddHeader(security.ContentType, security.Json).AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                switch (param)
                {
                    case IEnumerable<Codes> _:
                        return response.IsSuccessful;
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
            }
            return null;
        }
        public async Task<double> PutContext(Catalog.Request.StocksStrategics param)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestStrategics(param), Method.PUT).AddJsonBody(param, security.ContentType), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                SendMessage((int)response.StatusCode);

                return JsonConvert.DeserializeObject<double>(response.Content) - Coin;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return Coin;
        }
        public async Task<int> PutContext<T>(Codes param)
        {
            var response = await client.ExecuteAsync<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Code), Method.PUT).AddJsonBody(param, security.ContentType), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
            }
            return int.MinValue;
        }
        public async Task<double> PutContext(Privacies param)
        {
            try
            {
                Coin = 0;
                var response = await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.PUT, DataFormat.Json).AddJsonBody(param, security.ContentType), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return JsonConvert.DeserializeObject<double>(response.Content) - Coin;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return Coin;
        }
        public async Task<int> DeleteContext<T>(IParameters param)
        {
            var response = await client.ExecuteAsync<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Security), Method.DELETE), source.Token);

            try
            {
                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage((int)response.StatusCode);
            }
            return int.MinValue;
        }
        static GoblinBatClient Client
        {
            get; set;
        }
        [Conditional("DEBUG")]
        void SendMessage(object code)
        {
            if (code is double? && Temp < Coin)
            {
                Temp = (int)(Coin + 1);
                Console.WriteLine(((double)Coin).ToString("C0"));
            }
            else if (code is Catalog.Request.Charts chart)
                Console.WriteLine("Code_" + chart.Code + " Start_" + chart.Start + " End_" + chart.End);

            else if (code is int response && response > 200)
                Console.WriteLine(response);

            else if (code is string str)
                Console.WriteLine(str);
        }
        GoblinBatClient(dynamic key)
        {
            security = new Security(key);
            client = new RestClient(security.Url)
            {
                Timeout = -1
            };
            source = new CancellationTokenSource();
            random = new Random();
        }
        int Temp
        {
            get; set;
        }
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly Random random;
        readonly IRestClient client;
    }
}