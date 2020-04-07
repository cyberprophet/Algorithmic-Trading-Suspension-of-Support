using System;
using System.Linq;
using ShareInvest.Catalog;
using ShareInvest.Strategy.XingAPI;

namespace ShareInvest.Strategy.Statistics
{
    class Analysis : TF
    {
        protected internal override void OnReceiveTrend(int volume) => Volume += volume;
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
                string classification = bt.Classification, price;
                var check = classification.Equals(buy);
                var max = Max(specify.Assets / ((check ? e.BuyPrice : e.SellPrice) * Const.TransactionMultiplier * Const.MarginRate200402), classification);
                double[] sp = new double[10], bp = new double[10];

                for (int i = 0; i < 10; i++)
                    if (double.TryParse((e.BuyPrice - Const.ErrorRate * (9 - i)).ToString("F2"), out double bt) && double.TryParse((e.SellPrice + Const.ErrorRate * (9 - i)).ToString("F2"), out double st))
                    {
                        sp[i] = st;
                        bp[i] = bt;
                    }
                if (string.IsNullOrEmpty(bt.AvgPurchase) == false)
                {
                    price = GetExactPrice(bt.AvgPurchase);

                    switch (classification)
                    {
                        case sell:
                            if (bt.Quantity < 0)
                            {
                                if (bt.BuyOrder.Count == 0 && max < -bt.Quantity && double.TryParse(price, out double bAvg) && bAvg > bp[0])
                                {
                                    bt.SendNewOrder(bAvg > bp[bp.Length - 1] ? bp[bp.Length - 1].ToString("F2") : price, buy);

                                    return;
                                }
                                if (bt.BuyOrder.Count > 0)
                                {
                                    var order = bt.BuyOrder.OrderBy(o => o.Key).First();

                                    if (order.Key < bp[5])
                                    {
                                        bt.SendClearingOrder(order.Value);

                                        return;
                                    }
                                }
                                if (bt.SellOrder.Count > 0 && SetCorrectionSellOrder(price, bp))
                                    return;
                            }
                            break;

                        case buy:
                            if (bt.Quantity > 0)
                            {
                                if (bt.SellOrder.Count == 0 && max < bt.Quantity && double.TryParse(price, out double sAvg) && sAvg < sp[0])
                                {
                                    bt.SendNewOrder(sAvg < sp[sp.Length - 1] ? sp[sp.Length - 1].ToString("F2") : price, sell);

                                    return;
                                }
                                if (bt.SellOrder.Count > 0)
                                {
                                    var order = bt.SellOrder.OrderByDescending(o => o.Key).First();

                                    if (sp[5] < order.Key)
                                    {
                                        bt.SendClearingOrder(order.Value);

                                        return;
                                    }
                                }
                                if (bt.BuyOrder.Count > 0 && SetCorrectionBuyOrder(price, sp))
                                    return;
                            }
                            break;
                    }
                }
                foreach (var kv in check ? bt.BuyOrder : bt.SellOrder)
                    if (Array.Exists(check ? bp : sp, o => o == kv.Key) == false && (check ? bt.BuyOrder.ContainsKey(kv.Key) : bt.SellOrder.ContainsKey(kv.Key)))
                    {
                        bt.SendClearingOrder(kv.Value);

                        return;
                    }
                bt.SendNewOrder(check ? bp : sp, classification, check ? e.BuyQuantity : e.SellQuantity);
            }
        }
        bool SetCorrectionSellOrder(string avg, double[] buy)
        {
            if (double.TryParse(avg, out double bAvg) && double.TryParse(((buy[buy.Length - 1] - bAvg * bt.Quantity) / (1 - bt.Quantity)).ToString("F2"), out double prospect))
            {
                double price = bt.SellOrder.Min(o => o.Key) - Const.ErrorRate, abscond = bt.SellOrder.Max(o => o.Key) + Const.ErrorRate;

                if (prospect > bAvg && price > buy[buy.Length - 1])
                {
                    bt.SendCorrectionOrder(price.ToString("F2"), bt.SellOrder.OrderByDescending(o => o.Key).First().Value);

                    return true;
                }
                else if (bAvg < prospect && abscond < buy[buy.Length - 1] + Const.ErrorRate * 9)
                {
                    bt.SendCorrectionOrder(abscond.ToString("F2"), bt.SellOrder.OrderBy(o => o.Key).First().Value);

                    return true;
                }
            }
            return false;
        }
        bool SetCorrectionBuyOrder(string avg, double[] sell)
        {
            if (double.TryParse(avg, out double sAvg) && double.TryParse(((sAvg * bt.Quantity + sell[sell.Length - 1]) / (bt.Quantity + 1)).ToString("F2"), out double prospect))
            {
                double price = bt.BuyOrder.Max(o => o.Key) + Const.ErrorRate, abscond = bt.BuyOrder.Min(o => o.Key) - Const.ErrorRate;

                if (prospect < sAvg && price < sell[sell.Length - 1])
                {
                    bt.SendCorrectionOrder(price.ToString("F2"), bt.BuyOrder.OrderBy(o => o.Key).First().Value);

                    return true;
                }
                else if (sAvg > prospect && abscond > sell[sell.Length - 1] - Const.ErrorRate * 9)
                {
                    bt.SendCorrectionOrder(abscond.ToString("F2"), bt.BuyOrder.OrderByDescending(o => o.Key).First().Value);

                    return true;
                }
            }
            return false;
        }
        void Analysize(object sender, EventHandler.BackTesting.Datum e)
        {
            if (bt.SellOrder.Count > 0 && e.Volume > 0)
                bt.SetSellConclusion(e.Price, e.Volume);

            if (bt.BuyOrder.Count > 0 && e.Volume < 0)
                bt.SetBuyConclusion(e.Price, e.Volume);

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

            if (specify.Time == 1440 && GetCheckTime(e.Date.ToString()))
                OnReceiveTrend(e.Volume);
        }
        EMA EMA
        {
            get;
        }
        internal const string buy = "2";
        internal const string sell = "1";
        readonly BackTesting bt;
    }
}