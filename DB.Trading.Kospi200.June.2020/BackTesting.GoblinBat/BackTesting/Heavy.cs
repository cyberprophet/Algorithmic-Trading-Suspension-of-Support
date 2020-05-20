using System;
using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.Statistics
{
    class Heavy : Analysis
    {
        protected internal override bool ForTheLiquidationOfBuyOrder(string time, string price, double[] selling, int quantity)
        {
            if (double.TryParse(price, out double sAvg) && sAvg == selling[selling.Length - 1] && bt.SendNewOrder(time, price, sell, quantity))
            {
                var cPrice = sAvg - Const.ErrorRate;
                var oPrice = cPrice.ToString("F2");

                if (bt.BuyOrder.Count == 0 && bt.Quantity + 1 < specify.Assets / (cPrice * Const.TransactionMultiplier * specify.MarginRate))
                    return bt.SendNewOrder(time, oPrice, buy, quantity);

                if (bt.BuyOrder.Count > 0 && bt.BuyOrder.ContainsKey(oPrice) == false)
                    return bt.SendCorrectionOrder(time, oPrice, bt.BuyOrder.OrderBy(o => o.Key).First().Value, quantity);
            }
            return false;
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string time, string price, double[] bid, int quantity)
        {
            if (double.TryParse(price, out double bAvg) && bAvg == bid[bid.Length - 1] && bt.SendNewOrder(time, price, buy, quantity))
            {
                var cPrice = bAvg + Const.ErrorRate;
                var oPrice = cPrice.ToString("F2");

                if (bt.SellOrder.Count == 0 && Math.Abs(bt.Quantity - 1) < specify.Assets / (cPrice * Const.TransactionMultiplier * specify.MarginRate))
                    return bt.SendNewOrder(time, oPrice, sell, quantity);

                if (bt.SellOrder.Count > 0 && bt.SellOrder.ContainsKey(oPrice) == false)
                    return bt.SendCorrectionOrder(time, oPrice, bt.SellOrder.OrderByDescending(o => o.Key).First().Value, quantity);
            }
            return false;
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(string time, double[] selling)
        {
            var sell = bt.SellOrder.OrderBy(o => o.Key).First();

            return double.TryParse(sell.Key, out double csp) && selling[bt.SellOrder.Count == 1 ? 3 : (selling.Length - 1)] < csp && bt.SendClearingOrder(time, sell.Value);
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string time, double[] bid)
        {
            var buy = bt.BuyOrder.OrderByDescending(o => o.Key).First();

            return double.TryParse(buy.Key, out double cbp) && bid[bt.BuyOrder.Count == 1 ? 3 : (bid.Length - 1)] > cbp && bt.SendClearingOrder(time, buy.Value);
        }
        protected internal override void SendNewOrder(string time, double[] param, string classification, int quantity)
        {
            var check = classification.Equals(buy);
            var price = param[5];
            string key = price.ToString("F2"), oPrice = param[param.Length - 1].ToString("F2");

            if (check && bt.Quantity < 0 && param[param.Length - 1] > 0 && bt.BuyOrder.Count < -bt.Quantity && bt.BuyOrder.ContainsKey(oPrice) == false && bt.SendNewOrder(time, oPrice, buy, quantity))
                return;

            else if (check == false && bt.Quantity > 0 && param[param.Length - 1] > 0 && bt.SellOrder.Count < bt.Quantity && bt.SellOrder.ContainsKey(oPrice) == false && bt.SendNewOrder(time, oPrice, sell, quantity))
                return;

            else if (price > 0 && GetPermission(price) && (check ? bt.Quantity + bt.BuyOrder.Count : bt.SellOrder.Count - bt.Quantity) < Max(specify.Assets / (price * Const.TransactionMultiplier * specify.MarginRate), check ? XingAPI.Classification.Buy : XingAPI.Classification.Sell) && (check ? bt.BuyOrder.ContainsKey(key) : bt.SellOrder.ContainsKey(key)) == false && bt.SendNewOrder(time, key, classification, quantity))
                return;
        }
        protected internal override bool SetCorrectionBuyOrder(string time, string avg, double buy, int quantity)
        {
            string price = string.Empty, bPrice = bt.BuyOrder.OrderBy(o => o.Key).First().Key;
            var gap = Const.ErrorRate * bt.Quantity;

            foreach (var kv in bt.BuyOrder.OrderByDescending(o => o.Key))
            {
                if (string.IsNullOrEmpty(price) == false && double.TryParse(kv.Key, out double oPrice) && (oPrice + Const.ErrorRate).ToString("F2").Equals(price) && double.TryParse(bPrice, out double cPrice) && cPrice - gap > buy - Const.ErrorRate * 9)
                {
                    var sPrice = (cPrice - gap).ToString("F2");

                    if (bt.BuyOrder.ContainsKey(sPrice) == false)
                        return bt.SendCorrectionOrder(time, sPrice, kv.Value, quantity);
                }
                price = kv.Key;
            }
            return false;
        }
        protected internal override bool SetCorrectionSellOrder(string time, string avg, double sell, int quantity)
        {
            string price = string.Empty, sPrice = bt.SellOrder.OrderByDescending(o => o.Key).First().Key;
            var gap = Const.ErrorRate * bt.Quantity;

            foreach (var kv in bt.SellOrder.OrderBy(o => o.Key))
            {
                if (string.IsNullOrEmpty(price) == false && double.TryParse(kv.Key, out double oPrice) && (oPrice - Const.ErrorRate).ToString("F2").Equals(price) && double.TryParse(sPrice, out double cPrice) && cPrice - gap < sell + Const.ErrorRate * 9)
                {
                    var bPrice = (cPrice - gap).ToString("F2");

                    if (bt.SellOrder.ContainsKey(bPrice) == false)
                        return bt.SendCorrectionOrder(time, bPrice, kv.Value, quantity);
                }
                price = kv.Key;
            }
            return false;
        }
        bool GetPermission(double price)
        {
            if (bt.BuyOrder.Count > 0 && bt.Quantity > 0)
            {
                var cPrice = (price - Const.ErrorRate).ToString("F2");

                if (double.TryParse(cPrice, out double bPrice) && double.TryParse(bt.BuyOrder.OrderBy(o => o.Key).First().Key, out double bKey) && bPrice > bKey && bt.BuyOrder.ContainsKey(cPrice))
                    return false;
            }
            if (bt.SellOrder.Count > 0 && bt.Quantity < 0)
            {
                var cPrice = (price + Const.ErrorRate).ToString("F2");

                if (double.TryParse(cPrice, out double sPrice) && double.TryParse(bt.SellOrder.OrderByDescending(o => o.Key).First().Key, out double sKey) && sPrice < sKey && bt.SellOrder.ContainsKey(cPrice))
                    return false;
            }
            return true;
        }
        double Max(double max, XingAPI.Classification classification)
        {
            var num = 4.5D;

            foreach (var kv in bt.Judge)
                switch (classification)
                {
                    case XingAPI.Classification.Sell:
                        if (kv.Value > 0)
                            num += 0.5;

                        break;

                    case XingAPI.Classification.Buy:
                        if (kv.Value < 0)
                            num += 0.5;

                        break;
                }
            return max * num * 0.1;
        }
        internal Heavy(BackTesting bt, Catalog.XingAPI.Specify specify) : base(bt, specify) => Console.WriteLine(specify.Strategy);
    }
}