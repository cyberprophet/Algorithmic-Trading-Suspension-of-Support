using System;
using ShareInvest.Catalog;
using ShareInvest.Strategy.XingAPI;

namespace ShareInvest.Strategy.Statistics
{
    abstract class Analysis : TF
    {
        protected internal abstract void SendNewOrder(double[] param, string classification, int quantity);
        protected internal abstract bool SetCorrectionBuyOrder(string avg, double buy, int quantity);
        protected internal abstract bool SetCorrectionSellOrder(string avg, double sell, int quantity);
        protected internal abstract bool ForTheLiquidationOfSellOrder(double[] bid);
        protected internal abstract bool ForTheLiquidationOfSellOrder(string price, double[] bid, int quantity);
        protected internal abstract bool ForTheLiquidationOfBuyOrder(double[] selling);
        protected internal abstract bool ForTheLiquidationOfBuyOrder(string price, double[] selling, int quantity);
        protected internal override void OnReceiveTrend(int volume) => Volume += volume;
        protected internal Analysis(BackTesting bt, Catalog.XingAPI.Specify specify) : base(specify)
        {
            this.bt = bt;
            bt.SendDatum += Analysize;

            if (specify.Time == 1440)
            {
                bt.SendQuotes += OnReceiveQuotes;
                RollOver = specify.RollOver ? false : true;
            }
        }
        protected internal string GetExactPrice(string avg)
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
        double Max(double max, string classification)
        {
            int num = 9;

            foreach (var kv in bt.Judge)
            {
                if (classification.Equals(sell) && kv.Value > 0)
                    num--;

                else if (classification.Equals(buy) && kv.Value < 0)
                    num--;
            }
            return max * num * 0.1;
        }
        void OnReceiveQuotes(object sender, EventHandler.BackTesting.Quotes e)
        {
            if (int.TryParse(e.Time.Substring(6, 6), out int time) && (time < 090000 && time > 045959) == false && (time > 153459 && time < 180000) == false && string.IsNullOrEmpty(bt.Classification) == false)
            {
                string classification = bt.Classification, price;
                var check = classification.Equals(buy);
                var max = Max(specify.Assets / ((check ? e.BuyPrice : e.SellPrice) * Const.TransactionMultiplier * specify.MarginRate), classification);
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
                                if (bt.BuyOrder.Count == 0 && max < -bt.Quantity && ForTheLiquidationOfSellOrder(price, bp, e.BuyQuantity))
                                    return;

                                if (bt.BuyOrder.Count > 0 && ForTheLiquidationOfSellOrder(bp))
                                    return;

                                if (bt.SellOrder.Count > 0 && SetCorrectionSellOrder(price, sp[sp.Length - 1], e.SellQuantity))
                                    return;
                            }
                            break;

                        case buy:
                            if (bt.Quantity > 0)
                            {
                                if (bt.SellOrder.Count == 0 && max < bt.Quantity && ForTheLiquidationOfBuyOrder(price, sp, e.SellQuantity))
                                    return;

                                if (bt.SellOrder.Count > 0 && ForTheLiquidationOfBuyOrder(sp))
                                    return;

                                if (bt.BuyOrder.Count > 0 && SetCorrectionBuyOrder(price, bp[bp.Length - 1], e.BuyQuantity))
                                    return;
                            }
                            break;
                    }
                }
                foreach (var kv in check ? bt.BuyOrder : bt.SellOrder)
                    if (double.TryParse(kv.Key, out double key) && Array.Exists(check ? bp : sp, o => o == key) == false && (check ? bt.BuyOrder.ContainsKey(kv.Key) : bt.SellOrder.ContainsKey(kv.Key)))
                    {
                        bt.SendClearingOrder(kv.Value);

                        return;
                    }
                SendNewOrder(check ? bp : sp, classification, check ? e.BuyQuantity : e.SellQuantity);
            }
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

            if (specify.Time == 1440)
            {
                switch (GetCheckTime(e.Date.ToString()))
                {
                    case true:
                        OnReceiveTrend(e.Volume);
                        break;

                    case false:
                        bt.SellOrder.Clear();
                        bt.BuyOrder.Clear();
                        break;
                }
                if (e.Date.ToString().Substring(6, 6).Equals(end))
                    bt.SetStatisticalStorage(e.Date.ToString().Substring(0, 6), e.Price, RollOver);
            }
        }
        bool RollOver
        {
            get; set;
        }
        EMA EMA
        {
            get;
        }
        internal const string buy = "2";
        internal const string sell = "1";
        internal readonly BackTesting bt;
    }
}