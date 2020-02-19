using System;
using System.Collections.Generic;
using System.Linq;
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
                        ScreenNo = string.Concat(strategy ? api.Classification : api.WindingClass, kv.Value.ToString().Substring(0, 3)),
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
            api.Difference = (api.WindingClass.Equals(string.Empty) ? max : max / 3) - Math.Abs(api.Quantity) - (classification > 0 ? api.BuyOrder.Count : api.SellOrder.Count);

            if (api.Difference > 1)
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
            if (over < 3 && over > -3)
            {
                SendClearingOrder(over);
                SendClearingOrder(-over);
            }
            else
                SendClearingOrder(over);

            if (api.Quantity != 0)
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = "DoNotRollOver",
                    ScreenNo = string.Concat(api.Classification, api.WindingClass, api.Quantity.ToString("D2")),
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
                RQName = price.ToString("F2"),
                ScreenNo = string.Concat(classification, price.ToString().Substring(0, 3)),
                AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                Code = specify.Code,
                OrdKind = 2,
                SlbyTP = classification,
                OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                Qty = 1,
                Price = Math.Round(price, 2).ToString("F2"),
                OrgOrdNo = number
            });
        }
        private void OnDetermineTheTrend(int trendSell, int trendBuy, double priceSell, double priceBuy)
        {
            if (trendSell > trendBuy)
            {
                if (api.SellOrder.Count > 0 && api.SellOrder.ContainsValue(priceSell) == false)
                {
                    var temp = api.SellOrder.First(o => o.Value == api.SellOrder.Max(p => p.Value));
                    var number = temp.Key;

                    if (api.SellOrder.Remove(number))
                        SendCorrectionOrder(temp.Value - Const.ErrorRate, number, "1");
                }
                if (api.BuyOrder.ContainsValue(priceBuy))
                {
                    var temp = api.BuyOrder.First(o => o.Value == priceBuy);
                    var number = temp.Key;

                    if (api.BuyOrder.Remove(number))
                        SendCorrectionOrder(temp.Value - Const.ErrorRate * (api.BuyOrder.Count + 1), number, "2");
                }
            }
            if (trendBuy > trendSell)
            {
                if (api.BuyOrder.Count > 0 && api.BuyOrder.ContainsValue(priceBuy) == false)
                {
                    var temp = api.BuyOrder.First(o => o.Value == api.BuyOrder.Min(p => p.Value));
                    var number = temp.Key;

                    if (api.BuyOrder.Remove(number))
                        SendCorrectionOrder(temp.Value + Const.ErrorRate, number, "2");
                }
                if (api.SellOrder.ContainsValue(priceSell))
                {
                    var temp = api.SellOrder.First(o => o.Value == priceSell);
                    var number = temp.Key;

                    if (api.SellOrder.Remove(number))
                        SendCorrectionOrder(temp.Value + Const.ErrorRate * (api.SellOrder.Count + 1), number, "1");
                }
            }
        }
        private void OnReceiveQuotes(object sender, EventHandler.Quotes e)
        {
            if (strategy && api.Total.Count > 0)
                Temp = api.Total.Dequeue().Split(';');

            if (api.OnReceiveBalance && Temp != null)
            {
                bool check = api.BuyOrder.Count > 0 || api.SellOrder.Count > 0;

                if (check && e.Price[4] == Sell && e.Price[5] == Buy)
                    OnDetermineTheTrend(int.Parse(Temp[0]), int.Parse(Temp[1]), e.Price[4], e.Price[5]);

                else if (check)
                {
                    if (Sell > e.Price[4])
                        OnDetermineTheTrend(1, 0, e.Price[4], e.Price[5]);

                    else if (Sell < e.Price[4])
                        OnDetermineTheTrend(0, 1, e.Price[4], e.Price[5]);
                }
                else if (api.WindingClass.Equals(string.Empty) == false && api.WindingUp > 1)
                    SendNewOrder(e.Price, api.WindingUp, api.WindingClass);

                else if (api.Classification.Equals(string.Empty) == false && api.Difference > 1)
                    SendNewOrder(e.Price, api.Difference, api.Classification);
            }
            if (int.Parse(e.Time) > 154359 && strategy && api.Trend.Count > 0)
            {
                int over = 0;

                foreach (var kv in api.Trend)
                    over += kv.Value.Contains("-") ? -1 : 1;

                api.Trend.Clear();

                if (over != 0)
                    SendRollOverOrder(over);
            }
            Sell = e.Price[4];
            Buy = e.Price[5];
        }
        private void SendNewOrder(double[] param, double length, string classification)
        {
            for (int i = 1; i < (length > 3 ? 4 : (length < 2 ? 2 : 3)); i++)
            {
                var price = param[classification.Equals("2") ? 9 - i : i];

                if (classification.Equals("2") ? api.BuyOrder.ContainsValue(price) : api.SellOrder.ContainsValue(price))
                    continue;

                api.OnReceiveBalance = false;
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = price.ToString("F2"),
                    ScreenNo = string.Concat(classification, "00", i),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 1,
                    SlbyTP = classification,
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = Math.Round(price, 2).ToString("F2"),
                    OrgOrdNo = string.Empty
                });
            }
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