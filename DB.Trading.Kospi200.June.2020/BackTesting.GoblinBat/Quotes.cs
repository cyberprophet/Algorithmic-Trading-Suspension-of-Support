using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShareInvest.Interface.Struct;
using ShareInvest.OpenAPI;

namespace ShareInvest.Strategy
{
    public class Quotes
    {
        public Quotes(Specify specify, ConnectAPI api)
        {
            this.specify = specify;
            this.api = api;
            strategy = specify.Strategy.Equals("TF");
            api.OnReceiveBalance = false;
            api.SendQuotes += OnReceiveQuotes;
            api.WindingClass = string.Empty;
        }
        internal void SendClearingOrder(double trend)
        {
            foreach (KeyValuePair<string, double> kv in trend > 0 ? api.SellOrder.OrderBy(o => o.Value) : api.BuyOrder.OrderByDescending(o => o.Value))
                if (trend > 0 ? api.SellOrder.Remove(kv.Key) : api.BuyOrder.Remove(kv.Key))
                {
                    api.OnReceiveBalance = false;
                    api.OnReceiveOrder(new PurchaseInformation
                    {
                        RQName = string.Concat(kv.Key, ";", kv.Value),
                        ScreenNo = string.Concat(trend > 0 ? "1" : "2", GetScreenNumber().ToString("D3")),
                        AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                        Code = specify.Code,
                        OrdKind = 3,
                        SlbyTP = string.Empty,
                        OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                        Qty = 1,
                        Price = kv.Value.ToString("F2"),
                        OrgOrdNo = kv.Key
                    });
                }
        }
        internal void SetTrendFollowing(double max, double classification)
        {
            api.Difference = max - Math.Abs(api.Quantity) - (classification > 0 ? api.BuyOrder.Count : api.SellOrder.Count);

            if (api.Difference > 2)
                api.Classification = classification > 0 ? "2" : "1";

            else
                api.Classification = string.Empty;
        }
        internal void SetWindingUp(double classification)
        {
            api.WindingUp = Math.Abs(api.Quantity) - (classification > 0 ? api.BuyOrder.Count : api.SellOrder.Count);

            if (api.WindingUp > 0 && classification > 0 && api.Quantity < 0 && api.Classification.Equals("2") == false)
                api.WindingClass = "2";

            else if (api.WindingUp > 0 && classification < 0 && api.Quantity > 0 && api.Classification.Equals("1") == false)
                api.WindingClass = "1";

            else
                api.WindingClass = string.Empty;
        }
        private void SendRollOverOrder(int over)
        {
            SendClearingOrder(over);
            SendClearingOrder(-over);

            if (api.Quantity != 0)
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = "DoNotRollOver",
                    ScreenNo = string.Concat(Math.Abs(over), GetScreenNumber().ToString("D3")),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 1,
                    SlbyTP = api.Quantity > 0 ? "1" : "2",
                    OrdTp = ((int)PurchaseInformation.OrderType.시장가).ToString(),
                    Qty = Math.Abs(api.Quantity) / (over < 3 && over > -3 ? 1 : 2),
                    Price = string.Empty,
                    OrgOrdNo = string.Empty
                });
        }
        private void SendCorrectionOrder(double price, string number, string classification)
        {
            api.OnReceiveBalance = false;
            api.OnReceiveOrder(new PurchaseInformation
            {
                RQName = string.Concat(price, ';', number),
                ScreenNo = string.Concat(classification, GetScreenNumber().ToString("D3")),
                AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                Code = specify.Code,
                OrdKind = 2,
                SlbyTP = classification,
                OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                Qty = 1,
                Price = price.ToString("F2"),
                OrgOrdNo = number
            });
        }
        private void OnDetermineTheTrend(int trendSell, int trendBuy, double priceSell, double priceBuy)
        {
            if (trendSell > trendBuy)
            {
                if (api.SellOrder.Count > 0 && api.SellOrder.ContainsValue(priceSell) == false)
                {
                    var number = api.SellOrder.First(o => o.Value == api.SellOrder.Max(p => p.Value)).Key;
                    var price = api.SellOrder[number] - Const.ErrorRate;

                    if (api.SellOrder.Count > 1 && price > priceBuy && api.SellOrder.Remove(number))
                        SendCorrectionOrder(price, number, "1");
                }
                if (api.BuyOrder.ContainsValue(priceBuy))
                {
                    var number = api.BuyOrder.First(o => o.Value == priceBuy).Key;
                    var price = api.BuyOrder.Min(o => o.Value) - Const.ErrorRate;

                    if (api.BuyOrder.Remove(number))
                        SendCorrectionOrder(price, number, "2");
                }
            }
            if (trendBuy > trendSell)
            {
                if (api.BuyOrder.Count > 0 && api.BuyOrder.ContainsValue(priceBuy) == false)
                {
                    var number = api.BuyOrder.First(o => o.Value == api.BuyOrder.Min(p => p.Value)).Key;
                    var price = api.BuyOrder[number] + Const.ErrorRate;

                    if (api.BuyOrder.Count > 1 && price < priceSell && api.BuyOrder.Remove(number))
                        SendCorrectionOrder(price, number, "2");
                }
                if (api.SellOrder.ContainsValue(priceSell))
                {
                    var number = api.SellOrder.First(o => o.Value == priceSell).Key;
                    var price = api.SellOrder.Max(o => o.Value) + Const.ErrorRate;

                    if (api.SellOrder.Remove(number))
                        SendCorrectionOrder(price, number, "1");
                }
            }
        }
        private void SendNewOrder(double[] param, string classification)
        {
            var price = param[classification.Equals("2") ? 9 : 0];

            if (classification.Equals("2") ? api.BuyOrder.ContainsValue(price) : api.SellOrder.ContainsValue(price))
                return;

            api.OnReceiveBalance = false;
            api.OnReceiveOrder(new PurchaseInformation
            {
                RQName = string.Concat(price, ';'),
                ScreenNo = string.Concat(classification, GetScreenNumber().ToString("D3")),
                AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                Code = specify.Code,
                OrdKind = 1,
                SlbyTP = classification,
                OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                Qty = 1,
                Price = price.ToString("F2"),
                OrgOrdNo = string.Empty
            });
        }
        private void OnReceiveQuotes(object sender, EventHandler.Quotes e)
        {
            if (strategy && api.Total.Count > 0)
                Temp = api.Total.Dequeue().Split(';');

            if (Temp != null)
            {
                bool accumulate = e.Price[4] == Sell && e.Price[5] == Buy;

                if (api.OnReceiveBalance && api.WindingClass.Equals(string.Empty) == false)
                    SendNewOrder(e.Price, api.WindingClass);

                if (api.OnReceiveBalance && api.Classification.Equals(string.Empty) == false)
                    SendNewOrder(e.Price, api.Classification);

                if (api.OnReceiveBalance && accumulate)
                {
                    if (e.Price[4] == Sell && e.Price[5] == Buy)
                        OnDetermineTheTrend(AccumulateSell += int.Parse(Temp[0]), AccumulateBuy += int.Parse(Temp[1]), e.Price[4], e.Price[5]);

                    else if (Sell > e.Price[4] || Buy > e.Price[5])
                        OnDetermineTheTrend(1, 0, e.Price[4], e.Price[5]);

                    else if (Sell < e.Price[4] || Buy < e.Price[5])
                        OnDetermineTheTrend(0, 1, e.Price[4], e.Price[5]);
                }
                if (accumulate == false)
                {
                    AccumulateSell = 0;
                    AccumulateBuy = 0;
                    Sell = e.Price[4];
                    Buy = e.Price[5];
                }
            }
            if (int.Parse(e.Time) > 154259 && strategy && api.Trend.Count > 0)
            {
                int over = 0;

                foreach (var kv in api.Trend)
                    over += kv.Value.Contains("-") ? -1 : 1;

                api.Trend.Clear();

                if (over != 0)
                    SendRollOverOrder(over);
            }
        }
        private uint GetScreenNumber()
        {
            if (Accumulate++ > 99)
                Accumulate = 0;

            if (Accumulate == 0)
                api.ScreenNumber++;

            if (api.ScreenNumber == 99)
            {
                new Task(() => api.SetScreenNumber(1000, 1100)).Start();
                new Task(() => api.SetScreenNumber(2000, 2100)).Start();
                api.ScreenNumber = 0;
            }
            return api.ScreenNumber;
        }
        private int AccumulateSell
        {
            get; set;
        }
        private int AccumulateBuy
        {
            get; set;
        }
        private uint Accumulate
        {
            get; set;
        }
        private double Sell
        {
            get; set;
        }
        private double Buy
        {
            get; set;
        }
        private string[] Temp
        {
            get; set;
        }
        private readonly bool strategy;
        private readonly Specify specify;
        private readonly ConnectAPI api;
    }
}