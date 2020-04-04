using System;
using System.Linq;
using ShareInvest.Catalog;
using ShareInvest.Strategy.XingAPI;

namespace ShareInvest.Strategy.Statistics
{
    class Analysis : TF
    {
        internal Analysis(BackTesting bt, Catalog.XingAPI.Specify specify) : base(specify)
        {
            this.bt = bt;
            bt.SendDatum += Analysize;

            if (specify.Time == 1440)
                bt.SendQuotes += OnReceiveQuotes;
        }
        double Max(double max, string classification)
        {
            int num = 5;

            foreach (var kv in bt.Judge)
            {
                if (classification.Equals(sell) && kv.Value > 0)
                    num--;

                else if (classification.Equals(buy) && kv.Value < 0)
                    num--;
            }
            return max * num * 0.2;
        }
        string GetExactPrice(string avg)
        {
            if ((avg.Length < 6 || (avg.Length == 6 && (avg.Substring(5, 1).Equals("0") || avg.Substring(5, 1).Equals("5")))) && double.TryParse(avg, out double price))
                return (bt.Quantity > 0 ? price + Const.ErrorRate : price - Const.ErrorRate).ToString("F2");

            if (int.TryParse(avg.Substring(5, 1), out int rest) && double.TryParse(string.Concat(avg.Substring(0, 5), "5"), out double rp))
            {
                if (rest > 0 && rest < 5)
                    return bt.Quantity > 0 ? (rp + Const.ErrorRate).ToString("F2") : (rp - Const.ErrorRate * 2).ToString("F2");

                return bt.Quantity > 0 ? (rp + Const.ErrorRate * 2).ToString("F2") : (rp - Const.ErrorRate).ToString("F2");
            }
            return avg;
        }
        void OnReceiveQuotes(object sender, EventHandler.BackTesting.Quotes e)
        {
            if (int.TryParse(e.Time, out int time) && (time > 153459 && time < 180000) == false && string.IsNullOrEmpty(bt.Classification) == false)
            {
                string classification = bt.Classification;
                var check = classification.Equals(buy);
                var max = Max(specify.Assets / ((check ? e.BuyPrice : e.SellPrice) * Const.TransactionMultiplier * Const.MarginRate200402), classification);
                int i, being = (int)max;
                double[] sp = new double[5], bp = new double[5];
                bt.MaxAmount = max * (classification.Equals(buy) ? 1 : -1);

                for (i = 4; i > -1; i--)
                {
                    sp[i] = e.SellPrice + Const.ErrorRate * (4 - i);
                    bp[i] = e.BuyPrice - Const.ErrorRate * (4 - i);
                }
                switch (classification)
                {
                    case sell:
                        if (bt.Quantity < 0 && string.IsNullOrEmpty(bt.AvgPurchase) == false)
                        {
                            var price = GetExactPrice(bt.AvgPurchase);

                            switch (bt.BuyOrder.Count)
                            {
                                case 0:
                                    if (price.Equals(Price) == false)
                                    {
                                        bt.SendNewOrder(price, buy);
                                        Price = price;

                                        return;
                                    }
                                    else if (max < Math.Abs(API.Quantity))
                                    {
                                        bt.SendNewOrder(e.BuyPrice, max, buy);

                                        return;
                                    }
                                    break;

                                case 1:
                                    var number = bt.BuyOrder.First().Key;

                                    if (bt.BuyOrder.TryGetValue(number, out double cbp))
                                        if (cbp.ToString("F2").Equals(price) == false)
                                        {
                                            bt.SendCorrectionOrder(price, number);

                                            return;
                                        }
                                        else if (Array.Exists(bp, o => o == cbp) == false)
                                        {
                                            bt.SendClearingOrder(number);

                                            return;
                                        }
                                    break;

                                default:
                                    var order = bt.BuyOrder.First(f => f.Value == bt.BuyOrder.Max(o => o.Value)).Key;

                                    if (bt.BuyOrder.ContainsKey(order))
                                    {
                                        bt.SendClearingOrder(order);

                                        return;
                                    }
                                    break;
                            }
                        }
                        for (i = 4; i > -1; i--)
                            if (being + bt.Quantity <= i && bt.SellOrder.ContainsValue(sp[i]))
                            {
                                var number = bt.SellOrder.First(o => o.Value == bt.SellOrder.Min(m => m.Value)).Key;
                                var price = bt.SellOrder.Max(o => o.Value) + Const.ErrorRate;

                                if (bt.SellOrder.ContainsKey(number))
                                {
                                    bt.SendCorrectionOrder(price.ToString("F2"), number);

                                    return;
                                }
                            }
                        break;

                    case buy:
                        if (bt.Quantity > 0 && string.IsNullOrEmpty(bt.AvgPurchase) == false)
                        {
                            var price = GetExactPrice(bt.AvgPurchase);

                            switch (bt.SellOrder.Count)
                            {
                                case 0:
                                    if (price.Equals(Price) == false)
                                    {
                                        bt.SendNewOrder(price, sell);
                                        Price = price;

                                        return;
                                    }
                                    else if (max < API.Quantity)
                                    {
                                        bt.SendNewOrder(e.SellPrice, max, sell);

                                        return;
                                    }
                                    break;

                                case 1:
                                    var number = bt.SellOrder.First().Key;

                                    if (bt.SellOrder.TryGetValue(number, out double csp))
                                        if (csp.ToString("F2").Equals(price) == false)
                                        {
                                            bt.SendCorrectionOrder(price, number);

                                            return;
                                        }
                                        else if (Array.Exists(sp, o => o == csp) == false)
                                        {
                                            bt.SendClearingOrder(number);

                                            return;
                                        }
                                    break;

                                default:
                                    var order = bt.SellOrder.First(f => f.Value == bt.SellOrder.Min(o => o.Value)).Key;

                                    if (bt.SellOrder.ContainsKey(order))
                                    {
                                        bt.SendClearingOrder(order);

                                        return;
                                    }
                                    break;
                            }
                        }
                        for (i = 4; i > -1; i--)
                            if (being - bt.Quantity <= i && bt.BuyOrder.ContainsValue(bp[i]))
                            {
                                var number = bt.BuyOrder.First(o => o.Value == bt.BuyOrder.Max(m => m.Value)).Key;
                                var price = bt.BuyOrder.Min(o => o.Value) - Const.ErrorRate;

                                if (bt.BuyOrder.ContainsKey(number))
                                {
                                    bt.SendCorrectionOrder(price.ToString("F2"), number);

                                    return;
                                }
                            }
                        break;
                }
                foreach (var kv in check ? bt.BuyOrder : bt.SellOrder)
                    if (Array.Exists(check ? bp : sp, o => o == kv.Value) == false && (check ? bt.BuyOrder.ContainsKey(kv.Key) : bt.SellOrder.ContainsKey(kv.Key)))
                    {
                        bt.SendClearingOrder(kv.Key);

                        return;
                    }
                bt.SendNewOrder(check ? e.BuyPrice : e.SellPrice, max, classification);
            }
        }
        void Analysize(object sender, EventHandler.BackTesting.Datum e)
        {
            if (GetCheckOnTime(e.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(EMA.Make(specify.Short, Short.Count, e.Price, Short.Peek()));
            Long.Push(EMA.Make(specify.Long, Long.Count, e.Price, Long.Peek()));
            double popShort = Short.Pop(), popLong = Long.Pop();
            bt.Max(popShort - popLong - (Short.Peek() - Long.Peek()), specify);
            Short.Push(popShort);
            Long.Push(popLong);
        }
        EMA EMA
        {
            get;
        }
        string Price
        {
            get; set;
        }
        internal const string buy = "2";
        internal const string sell = "1";
        readonly BackTesting bt;
    }
}