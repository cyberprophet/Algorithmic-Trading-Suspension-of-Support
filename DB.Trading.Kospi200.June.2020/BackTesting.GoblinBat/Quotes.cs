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
                        ScreenNo = string.Concat(strategy ? api.Classification : api.WindingClass, kv.Key.Substring(kv.Key.Length - 3)),
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
                ScreenNo = string.Concat(classification, number.Substring(number.Length - 3)),
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
        private void OnReceiveQuotes(object sender, EventHandler.Quotes e)
        {
            if (strategy && api.Total.Count > 0)
                Temp = api.Total.Dequeue().Split(';');

            if (api.OnReceiveBalance && Temp != null)
            {
                if (strategy == false && e.Price[4] == Sell && e.Price[5] == Buy)
                {
                    Sell = e.Price[4];
                    Buy = e.Price[5];
                    int sell = int.Parse(Temp[0]), buy = int.Parse(Temp[1]);

                    if (sell > buy)
                    {
                        if (api.SellOrder.Count > 0 && api.SellOrder.ContainsValue(e.Price[4]) == false)
                        {
                            var number = api.SellOrder.First(o => o.Value == api.SellOrder.Max(p => p.Value)).Key;

                            if (api.SellOrder.Remove(number))
                                SendCorrectionOrder(api.SellOrder.Min(o => o.Value) - Const.ErrorRate, number, "1");
                        }
                        if (api.BuyOrder.ContainsValue(e.Price[5]))
                        {
                            var number = api.BuyOrder.First(o => o.Value == e.Price[5]).Key;

                            if (api.BuyOrder.Remove(number))
                                SendCorrectionOrder(api.BuyOrder.Min(o => o.Value) - Const.ErrorRate, number, "2");
                        }
                        return;
                    }
                    if (buy > sell)
                    {
                        if (api.BuyOrder.Count > 0 && api.BuyOrder.ContainsValue(e.Price[5]) == false)
                        {
                            var number = api.BuyOrder.First(o => o.Value == api.BuyOrder.Min(p => p.Value)).Key;

                            if (api.BuyOrder.Remove(number))
                                SendCorrectionOrder(api.BuyOrder.Max(o => o.Value) + Const.ErrorRate, number, "2");
                        }
                        if (api.SellOrder.ContainsValue(e.Price[4]))
                        {
                            var number = api.SellOrder.First(o => o.Value == e.Price[4]).Key;

                            if (api.SellOrder.Remove(number))
                                SendCorrectionOrder(api.SellOrder.Max(o => o.Value) + Const.ErrorRate, number, "1");
                        }
                    }
                    return;
                }
                if (api.SellOrder.Count > 0 && (strategy && api.Classification.Equals("1") || strategy == false && api.WindingClass.Equals("1")) || api.BuyOrder.Count > 0 && (strategy && api.Classification.Equals("2") || strategy == false && api.WindingClass.Equals("2")))
                {
                    SendCorrectionOrder(e.Price, e.Quantity);

                    return;
                }
                if (strategy && api.Classification.Equals(string.Empty) == false || strategy == false && api.WindingClass.Equals(string.Empty) == false)
                {
                    SendNewOrder(e.Price, strategy ? api.Difference : api.WindingUp);

                    return;
                }
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
        }
        private void SendCorrectionOrder(double[] param, int[] amount)
        {
            double price = 0;
            string number = string.Empty;
            int trend = 0;

            if (api.SellOrder.Count > 0 && (strategy && api.Classification.Equals("1") || strategy == false && api.WindingClass.Equals("1")))
            {
                if (amount[4] < amount[5] && api.SellOrder.ContainsValue(param[4]))
                {
                    price = api.SellOrder.Max(o => o.Value) + Const.ErrorRate;
                    number = api.SellOrder.First(o => o.Value == api.SellOrder.Min(p => p.Value)).Key;
                }
                else if (amount[4] > amount[5] && api.SellOrder.ContainsValue(param[4]) == false)
                {
                    price = api.SellOrder.Min(o => o.Value) - Const.ErrorRate;
                    number = api.SellOrder.First(o => o.Value == api.SellOrder.Max(p => p.Value)).Key;
                }
                trend = price > 0 && price >= param[4] ? -1 : 0;
            }
            else if (api.BuyOrder.Count > 0 && (strategy && api.Classification.Equals("2") || strategy == false && api.WindingClass.Equals("2")))
            {
                if (amount[4] > amount[5] && api.BuyOrder.ContainsValue(param[5]))
                {
                    price = api.BuyOrder.Min(o => o.Value) - Const.ErrorRate;
                    number = api.BuyOrder.First(o => o.Value == api.BuyOrder.Max(p => p.Value)).Key;
                }
                else if (amount[4] < amount[5] && api.BuyOrder.ContainsValue(param[5]) == false)
                {
                    price = api.BuyOrder.Max(o => o.Value) + Const.ErrorRate;
                    number = api.BuyOrder.First(o => o.Value == api.BuyOrder.Min(p => p.Value)).Key;
                }
                trend = price > 0 && price <= param[5] ? 1 : 0;
            }
            if (trend != 0 && (trend < 0 ? api.SellOrder.Remove(number) : api.BuyOrder.Remove(number)))
            {
                api.OnReceiveBalance = false;
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = price.ToString("F2"),
                    ScreenNo = string.Concat(strategy ? api.Classification : api.WindingClass, number.Substring(number.Length - 3)),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 2,
                    SlbyTP = trend > 0 ? "2" : "1",
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = Math.Round(price, 2).ToString("F2"),
                    OrgOrdNo = number
                });
            }
        }
        private void SendNewOrder(double[] param, double length)
        {
            api.OnReceiveBalance = false;
            var check = strategy ? api.Classification : api.WindingClass;

            for (int i = 1; i < (length > 3 ? 4 : (length < 2 ? 2 : 3)); i++)
            {
                var price = param[check.Equals("2") ? 9 - i : i];

                if (strategy && api.Classification.Equals("2") ? api.BuyOrder.ContainsValue(price) : api.SellOrder.ContainsValue(price))
                    continue;

                else if (strategy == false && api.WindingClass.Equals("2") ? api.BuyOrder.ContainsValue(price) : api.SellOrder.ContainsValue(price))
                    continue;

                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = price.ToString("F2"),
                    ScreenNo = string.Concat(strategy ? api.Classification : api.WindingClass, "00", i),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 1,
                    SlbyTP = strategy ? api.Classification : api.WindingClass,
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