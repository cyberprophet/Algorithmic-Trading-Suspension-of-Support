using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
                Client = new GoblinBatClient(key);

            return Client;
        }
        public static double Coin
        {
            get; private set;
        }
        public async Task<int> EmergencyContext<T>(Codes param)
            => (int)(await client.ExecuteAsync<T>(new RestRequest(string.Concat(security.CoreAPI, param.GetType().Name, "/", param.Code), Method.PUT)
                .AddJsonBody(param, security.ContentType), source.Token)).StatusCode;
        public async Task<Retention> EmergencyContext<T>(Queue<T> param) where T : struct
            => JsonConvert.DeserializeObject<Retention>((await client.ExecuteAsync(new RestRequest(string.Concat(security.CoreAPI, param.GetType().GetGenericArguments()[0].Name), Method.POST)
                .AddHeader(security.ContentType, security.Json)
                .AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token)).Content);
        public async Task<Dictionary<string, int>> GetContextAsync()
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestRanks(), Method.GET), source.Token);

                if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<Queue<Catalog.Request.Collect>> GetContextAsync(string code, string date)
        {
            try
            {
                var file = security.RequestToCollect(code, date);

                if (file.Item1)
                    return JsonConvert.DeserializeObject<Queue<Catalog.Request.Collect>>(file.Item2);

                else
                {
                    var response = await client.ExecuteAsync(new RestRequest(file.Item2, Method.GET), source.Token);

                    if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                    {
                        Coin += security.GetSettleTheFare(response.RawBytes.Length);
                        SendMessage(Coin);
                    }
                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                        switch (baseDate.CompareTo(date))
                        {
                            case int baseDate when baseDate < 0:
                                var collect = JsonConvert.DeserializeObject<string>(response.Content);
                                await security.AskForStorageAsync(code, collect, date);

                                return JsonConvert.DeserializeObject<Queue<Catalog.Request.Collect>>(collect);

                            case 0:
                                var queue = new Queue<Catalog.Request.Collect>();

                                foreach (var param in JArray.Parse(JsonConvert.DeserializeObject<string>(response.Content)))
                                    queue.Enqueue(new Catalog.Request.Collect
                                    {
                                        Date = param.Value<string>("Key"),
                                        Datum = param.Value<string>("Value")
                                    });
                                await security.AskForStorageAsync(code, JsonConvert.SerializeObject(queue), date);

                                return queue;
                        }
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<object> GetContext(Catalog.Request.SatisfyConditions conditions)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestConditions(conditions), Method.GET), source.Token);

                if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<Catalog.Request.SatisfyConditions>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<object> GetContext(Catalog.Request.Charts chart)
        {
            try
            {
                var request = security.RequestCharts(chart);

                if (request.Item2)
                {
                    if (Array.Exists(chart.Start.ToCharArray(), o => char.IsLetter(o)) && Array.Exists(chart.End.ToCharArray(), o => char.IsLetter(o)))
                        return JsonConvert.DeserializeObject<string>(request.Item1);

                    else
                    {
                        var enumerable = JsonConvert.DeserializeObject<IEnumerable<Charts>>(request.Item1);

                        if (string.IsNullOrEmpty(chart.Start) == false && string.IsNullOrEmpty(chart.End) == false)
                        {
                            if (Environment.Is64BitProcess && enumerable.First().Volume > 0)
                            {
                                var file = security.RequestDaysExists(chart);

                                if (file.Item1 == false && await security.RequestDaysAsync(file.Item2, enumerable) is int count)
                                    SendMessage(count);
                            }
                            else if (enumerable.First().Volume > 0)
                            {
                                var file = security.RequestDaysExists(chart);

                                if (file.Item1 && await security.RequestDaysAsync(file.Item2) is string contents)
                                    return JsonConvert.DeserializeObject<IEnumerable<Charts>>(contents);
                            }
                        }
                        return enumerable;
                    }
                }
                else
                {
                    var response = await client.ExecuteAsync(new RestRequest(request.Item1, Method.GET), source.Token);

                    if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                    {
                        if (chart.End.Length == 6 && chart.End.CompareTo(DateTime.Now.AddDays(-1).ToString("yyMMdd")) < 0 || chart.End.Length < 6)
                            await security.Save(chart, response.Content);

                        Coin += security.GetSettleTheFare(response.RawBytes.Length);
                        SendMessage(Coin);
                    }
                    if (Array.Exists(chart.Start.ToCharArray(), o => char.IsLetter(o)) && Array.Exists(chart.End.ToCharArray(), o => char.IsLetter(o)))
                        return JsonConvert.DeserializeObject<string>(response.Content);

                    else
                        return JsonConvert.DeserializeObject<IEnumerable<Charts>>(response.Content);
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
                SendMessage(chart);
            }
            return null;
        }
        public async Task<object> GetContext(Catalog.Request.IncorporatedStocks stocks)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestCount(stocks), Method.GET), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                if (string.IsNullOrEmpty(stocks.Date))
                {
                    var page = JsonConvert.DeserializeObject<int>(response.Content);

                    if (response.StatusCode.Equals(HttpStatusCode.OK) && page < 0x16)
                        return page;
                }
                else
                    return JsonConvert.DeserializeObject<List<Catalog.Request.IncorporatedStocks>>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<List<Catalog.Request.FinancialStatement>> GetContext(Catalog.Request.FinancialStatement param)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestCode(param), Method.GET), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                var list = JsonConvert.DeserializeObject<List<Catalog.Request.FinancialStatement>>(response.Content);
                var remove = new Queue<Catalog.Request.FinancialStatement>();
                var str = string.Empty;

                foreach (var fs in list.OrderBy(o => o.Date))
                {
                    var date = fs.Date.Substring(0, 5);

                    if (date.Equals(str))
                        remove.Enqueue(fs);

                    str = date;
                }
                while (remove.Count > 0)
                {
                    var fs = remove.Dequeue();

                    if (list.Remove(fs))
                        SendMessage(string.Concat("Count_" + list.Count + "\tDate_" + fs.Date));
                }
                return list;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<List<Catalog.Request.Consensus>> GetContext(Catalog.Request.Consensus consensus)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestConsensus(consensus), Method.GET), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<List<Catalog.Request.Consensus>>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<byte[]> GetContext(Catalog.Request.FileOfGoblinBat param)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestFile(param), Method.GET), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<byte[]>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<Codes> GetContext(Codes codes)
        {
            var response = await client.ExecuteAsync(new RestRequest(security.RequestCodes(codes), Method.GET), source.Token);

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
            var response = await client.ExecuteAsync(new RestRequest(security.RequestCodes(param, length), Method.GET), source.Token);

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
            var response = await client.ExecuteAsync<T>(new RestRequest(security.RequestCodes(param), Method.GET), source.Token);

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
            var response = await client.ExecuteAsync(new RestRequest(security.Request(param.GetType().Name), Method.GET), source.Token);

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
        public async Task<Queue<Catalog.Request.ConfirmRevisedStockPrice>> GetContext(Catalog.OpenAPI.RevisedStockPrice param)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestCode(param), Method.GET), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                    return JsonConvert.DeserializeObject<Queue<Catalog.Request.ConfirmRevisedStockPrice>>(response.Content);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
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
                    case TrendToCashflow _:
                        foreach (var content in JArray.Parse(response.Content))
                            if (int.TryParse(content.Values().ToArray()[0].ToString(), out int analysis))
                                stack.Push(new TrendToCashflow
                                {
                                    Code = string.Empty,
                                    Short = 5,
                                    Long = 0x3C,
                                    Trend = 0x14,
                                    Unit = 1,
                                    ReservationQuantity = 0,
                                    ReservationRevenue = 0xA,
                                    Addition = 0xB,
                                    Interval = 1,
                                    TradingQuantity = 1,
                                    PositionRevenue = 5.25e-3,
                                    PositionAddition = 7.25e-3,
                                    AnalysisType = Enum.GetName(typeof(AnalysisType), analysis)
                                });
                        break;

                    case ScenarioAccordingToTrend _:
                        foreach (var content in JArray.Parse(response.Content))
                        {
                            var param = content.Values().ToArray();

                            if (double.TryParse(param[7].ToString(), out double net)
                                && double.TryParse(param[5].ToString(), out double profit)
                                && int.TryParse(param[3].ToString(), out int sales)
                                && int.TryParse(param[1].ToString(), out int sTrend))
                                stack.Push(new ScenarioAccordingToTrend
                                {
                                    Code = string.Empty,
                                    Calendar = param[0].ToString(),
                                    Trend = sTrend,
                                    CheckSales = param[2].ToString().Equals("T"),
                                    Sales = sales * 0.01,
                                    CheckOperatingProfit = param[4].ToString().Equals("T"),
                                    OperatingProfit = profit,
                                    CheckNetIncome = param[6].ToString().Equals("T"),
                                    NetIncome = net,
                                    Short = 5,
                                    Long = 0x78,
                                    Quantity = 1,
                                    IntervalInSeconds = 0,
                                    ErrorRange = 0D
                                });
                        }
                        break;

                    case TrendsInStockPrices _:
                        foreach (var content in JArray.Parse(response.Content))
                        {
                            var param = content.Values().ToArray();

                            if (int.TryParse(param[5].ToString(), out int unit)
                                && double.TryParse(param[4].ToString(), out double purchase)
                                && double.TryParse(param[3].ToString(), out double profit)
                                && int.TryParse(param[2].ToString(), out int trend)
                                && int.TryParse(param[1].ToString(), out int jLong)
                                && int.TryParse(param[0].ToString(), out int jShort)
                                && char.TryParse(param[8].ToString(), out char setting)
                                && char.TryParse(param[6].ToString(), out char ls)
                                && char.TryParse(param[7].ToString(), out char type))
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

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return stack.OrderBy(o => random.Next(stack.Count));
        }
        public async Task<IEnumerable<ConvertConsensus>> GetContext(ConvertConsensus consensus)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestConsensus(consensus), Method.GET), source.Token);

                if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                if (response.Content != null && response.Content.Length > 0)
                {
                    var list = JsonConvert.DeserializeObject<List<ConvertConsensus>>(response.Content);

                    if (list != null && list.Count > 0)
                        return list.OrderBy(o => o.Quarter);
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return null;
        }
        public async Task<int> PostContext(Catalog.Request.SatisfyConditions conditions)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestConditions(conditions.GetType().Name), Method.POST)
                    .AddJsonBody(conditions, security.ContentType), source.Token);

                if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return int.MinValue;
        }
        public async Task<int> PostContext(Stack<Catalog.Request.IncorporatedStocks> stocks)
        {
            var status = int.MaxValue;

            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestIncorporatedStocks(stocks.Peek()), Method.POST)
                    .AddHeader(security.ContentType, security.Json)
                    .AddParameter(security.Json, JsonConvert.SerializeObject(stocks), ParameterType.RequestBody), source.Token);

                if (response != null && (int)response.StatusCode == 0xC8 && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                status = (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return status;
        }
        public async Task<bool> PostContext(ConfirmStrategics confirm)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestConfirm(confirm), Method.POST)
                    .AddHeader(security.ContentType, security.Json)
                    .AddParameter(security.Json, JsonConvert.SerializeObject(confirm), ParameterType.RequestBody), source.Token);

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
                var response = await client.ExecuteAsync(new RestRequest(security.RequestCode(), Method.POST)
                    .AddHeader(security.ContentType, security.Json)
                    .AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

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
                var response = await client.ExecuteAsync(new RestRequest(security.Request(param.GetType().GetGenericArguments()[0].Name), Method.POST)
                    .AddHeader(security.ContentType, security.Json)
                    .AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

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
            var response = await client.ExecuteAsync(new RestRequest(security.Request(param.GetType().Name), Method.POST)
                .AddJsonBody(param, security.ContentType), source.Token);

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
        public async Task<int> PostContext<T>(IEnumerable<T> param)
        {
            try
            {
                var index = 0;

                switch (param)
                {
                    case IEnumerable<Codes> _:
                    case IEnumerable<Catalog.Request.FinancialStatement> _:
                        index = 0;
                        break;

                    case IEnumerable<ConvertConsensus> _:
                        index = 7;
                        break;

                    default:
                        return int.MinValue;
                }
                var response = await client.ExecuteAsync(new RestRequest(security.Request(param.GetType().GetGenericArguments()[0].Name.Substring(index)), Method.POST)
                    .AddHeader(security.ContentType, security.Json)
                    .AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

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
            }
            return int.MinValue;
        }
        public async Task<int> PostContext(Catalog.OpenAPI.RevisedStockPrice param)
        {
            int code = int.MinValue;

            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.Request(param.GetType().Name), Method.POST)
                    .AddJsonBody(param, security.ContentType), source.Token);

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
        public async Task<int> PostContext(Catalog.Request.FileOfGoblinBat param)
        {
            int code = int.MinValue;

            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.Request(param.GetType().Name), Method.POST)
                    .AddHeader(security.ContentType, security.Json)
                    .AddParameter(security.Json, JsonConvert.SerializeObject(param), ParameterType.RequestBody), source.Token);

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
        public async Task<double> PutContext(Catalog.Request.Consensus consensus)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestConsensus(consensus), Method.PUT)
                    .AddJsonBody(consensus, security.ContentType), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                SendMessage((int)response.StatusCode);
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return Coin;
        }
        public async Task<double> PutContext(Catalog.Request.StocksStrategics param)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest(security.RequestStrategics(param), Method.PUT)
                    .AddJsonBody(param, security.ContentType), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                SendMessage((int)response.StatusCode);

                return JsonConvert.DeserializeObject<double>(response.Content) + Coin;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return Coin;
        }
        public async Task<int> PutContext<T>(Codes param)
        {
            var response = await client.ExecuteAsync<T>(new RestRequest(security.RequestCodes(param), Method.PUT)
                .AddJsonBody(param, security.ContentType), source.Token);

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
                var response = await client.ExecuteAsync(new RestRequest(security.RequestCodes(param), Method.PUT, DataFormat.Json)
                    .AddJsonBody(param, security.ContentType), source.Token);

                if (response != null && response.RawBytes != null && response.RawBytes.Length > 0)
                {
                    Coin += security.GetSettleTheFare(response.RawBytes.Length);
                    SendMessage(Coin);
                }
                return JsonConvert.DeserializeObject<double>(response.Content) + Coin;
            }
            catch (Exception ex)
            {
                SendMessage(ex.StackTrace);
            }
            return Coin;
        }
        public async Task<int> DeleteContext<T>(IParameters param)
        {
            var response = await client.ExecuteAsync<T>(new RestRequest(security.RequestCodes(param), Method.DELETE), source.Token);

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
        public bool InternalAccess => security.InternalAccess;
        static GoblinBatClient Client
        {
            get; set;
        }
        [Conditional("DEBUG")]
        void SendMessage(object code)
        {
            if (code is double coin && Show < coin)
            {
                Show = (int)coin + 1;
                Console.WriteLine(coin);
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
        int Show
        {
            get; set;
        }
        const string baseDate = "20201103";
        readonly CancellationTokenSource source;
        readonly Security security;
        readonly Random random;
        readonly IRestClient client;
    }
}