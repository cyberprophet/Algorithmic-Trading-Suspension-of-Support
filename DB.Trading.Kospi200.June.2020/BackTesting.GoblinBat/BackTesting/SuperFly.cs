using System;
using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.Statistics
{
    class SuperFly : Analysis
    {
        protected internal override bool ForTheLiquidationOfBuyOrder(string time, string price, double[] selling, int quantity)
        {
            var gap = bt.MaxAmount - bt.Quantity;

            if (0 < gap && gap < 1)
                foreach (var kv in bt.Judge.OrderBy(o => o.Key))
                    if (kv.Value < 0)
                    {
                        var index = selling[selling.Length - 1] - bt.TradingJudge[kv.Key];

                        return bt.SendNewOrder(time, selling[index > 0 && index < 5 ? (int)(selling.Length - index) : selling.Length - 1].ToString("F2"), sell, quantity);
                    }
            return selling[selling.Length - 1].ToString("F2").Equals(price) && bt.SendNewOrder(time, price, sell, quantity);
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string time, string price, double[] bid, int quantity)
        {
            var gap = bt.Quantity - bt.MaxAmount;

            if (0 < gap && gap < 1)
                foreach (var kv in bt.Judge.OrderBy(o => o.Key))
                    if (kv.Value > 0)
                    {
                        var index = bt.TradingJudge[kv.Key] - bid[bid.Length - 1];

                        return bt.SendNewOrder(time, bid[index > 0 && index < 5 ? (int)(bid.Length - index) : bid.Length - 1].ToString("F2"), buy, quantity);
                    }
            return bid[bid.Length - 1].ToString("F2").Equals(price) && bt.SendNewOrder(time, price, buy, quantity);
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(string time, double[] selling)
        {
            var sell = bt.SellOrder.OrderByDescending(o => o.Key).First();

            return double.TryParse(sell.Key, out double csp) && selling[bt.SellOrder.Count == 1 ? 5 : (selling.Length - 1)] < csp && bt.SendClearingOrder(time, sell.Value);
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string time, double[] bid)
        {
            var buy = bt.BuyOrder.OrderBy(o => o.Key).First();

            return double.TryParse(buy.Key, out double cbp) && bid[bt.BuyOrder.Count == 1 ? 5 : (bid.Length - 1)] > cbp && bt.SendClearingOrder(time, buy.Value);
        }
        protected internal override bool SetCorrectionBuyOrder(string time, string avg, double buy, int quantity)
        {
            if (double.TryParse(avg, out double rAvg) && Math.Abs(rAvg - buy) < Const.ErrorRate * bt.Quantity)
            {
                var price = double.MaxValue;
                var order = bt.BuyOrder.OrderByDescending(o => o.Key);

                foreach (var kv in order)
                {
                    if (double.TryParse(kv.Key, out double oPrice) && price - oPrice == Const.ErrorRate && bt.BuyOrder.ContainsKey((oPrice - Const.ErrorRate).ToString("F2")) == false)
                        return bt.SendCorrectionOrder(time, (oPrice - Const.ErrorRate).ToString("F2"), order.First().Value, quantity);

                    price = oPrice;
                }
            }
            return false;
        }
        protected internal override bool SetCorrectionSellOrder(string time, string avg, double sell, int quantity)
        {
            if (double.TryParse(avg, out double rAvg) && Math.Abs(rAvg - sell) < Const.ErrorRate * -bt.Quantity)
            {
                var price = double.MinValue;
                var order = bt.SellOrder.OrderBy(o => o.Key);

                foreach (var kv in order)
                {
                    if (double.TryParse(kv.Key, out double oPrice) && oPrice - price == Const.ErrorRate && bt.SellOrder.ContainsKey((oPrice + Const.ErrorRate).ToString("F2")) == false)
                        return bt.SendCorrectionOrder(time, (oPrice + Const.ErrorRate).ToString("F2"), order.First().Value, quantity);

                    price = oPrice;
                }
            }
            return false;
        }
        protected internal override void SendNewOrder(string time, double[] param, string classification, int quantity)
        {
            var check = classification.Equals(buy);
            var index = GetMaxJudge(param[param.Length - 1], check);

            if (index < 3 && index >= 0)
            {
                var price = param[7 - index].ToString("F2");

                if ((check ? bt.BuyOrder.ContainsKey(price) : bt.SellOrder.ContainsKey(price)) == false)
                    bt.SendNewOrder(time, price, classification, quantity);
            }
        }
        int GetMaxJudge(double price, bool judge)
        {
            int count = 0;

            while ((judge ? bt.MaxAmount - bt.Quantity - bt.BuyOrder.Count : bt.Quantity - bt.SellOrder.Count - bt.MaxAmount) > 1)
                foreach (var kv in bt.TradingJudge.OrderBy(o => o.Key))
                {
                    if (judge && ++count > bt.Quantity + bt.BuyOrder.Count)
                        return (int)((kv.Value - price) * count);

                    if (judge == false && ++count > bt.SellOrder.Count - bt.Quantity)
                        return (int)((price - kv.Value) * count);
                }
            return int.MinValue;
        }
        internal SuperFly(BackTesting bt, Catalog.XingAPI.Specify specify) : base(bt, specify) => Console.WriteLine(specify.Strategy);
    }
}