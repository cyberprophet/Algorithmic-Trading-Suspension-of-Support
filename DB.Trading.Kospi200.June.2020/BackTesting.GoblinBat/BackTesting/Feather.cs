using System;
using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.Statistics
{
    class Feather : Analysis
    {
        protected internal override bool ForTheLiquidationOfBuyOrder(string time, double[] selling)
        {
            var sell = bt.SellOrder.OrderByDescending(o => o.Key).First();

            return double.TryParse(sell.Key, out double csp) && selling[bt.SellOrder.Count == 1 ? 5 : (selling.Length - 1)] < csp ? bt.SendClearingOrder(time, sell.Value) : false;
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(string time, string price, double[] selling, int quantity) => double.TryParse(price, out double sAvg) && sAvg < selling[5] ? bt.SendNewOrder(time, sAvg < selling[selling.Length - 1] ? selling[selling.Length - 1].ToString("F2") : price, sell, quantity) : false;
        protected internal override bool ForTheLiquidationOfSellOrder(string time, double[] bid)
        {
            var buy = bt.BuyOrder.OrderBy(o => o.Key).First();

            return double.TryParse(buy.Key, out double cbp) && bid[bt.BuyOrder.Count == 1 ? 5 : (bid.Length - 1)] > cbp ? bt.SendClearingOrder(time, buy.Value) : false;
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string time, string price, double[] bid, int quantity) => double.TryParse(price, out double bAvg) && bAvg > bid[5] ? bt.SendNewOrder(time, bAvg > bid[bid.Length - 1] ? bid[bid.Length - 1].ToString("F2") : price, buy, quantity) : false;
        protected internal override void SendNewOrder(string time, double[] param, string classification, int quantity)
        {
            var check = classification.Equals(buy);
            var price = param[5];
            var key = price.ToString("F2");

            if (price > 0 && (check ? bt.Quantity + bt.BuyOrder.Count : bt.SellOrder.Count - bt.Quantity) < Max(specify.Assets / (price * Const.TransactionMultiplier * specify.MarginRate), check ? XingAPI.Classification.Buy : XingAPI.Classification.Sell) && (check ? bt.BuyOrder.ContainsKey(key) : bt.SellOrder.ContainsKey(key)) == false)
                bt.SendNewOrder(time, key, classification, quantity);
        }
        protected internal override bool SetCorrectionBuyOrder(string time, string avg, double buy, int quantity)
        {
            var sb = bt.BuyOrder.OrderByDescending(o => o.Key).First();

            return double.TryParse(sb.Key, out double price) && price > buy - Const.ErrorRate * 2 && double.TryParse(bt.BuyOrder.OrderBy(o => o.Key).First().Key, out double oPrice) && double.TryParse(avg, out double sAvg) && Math.Abs(sAvg - price) < Const.ErrorRate * 2 * bt.Quantity ? bt.SendCorrectionOrder(time, (oPrice - Const.ErrorRate * 2).ToString("F2"), sb.Value, quantity) : false;
        }
        protected internal override bool SetCorrectionSellOrder(string time, string avg, double sell, int quantity)
        {
            var sb = bt.SellOrder.OrderBy(o => o.Key).First();

            return double.TryParse(sb.Key, out double price) && price < sell + Const.ErrorRate * 2 && double.TryParse(bt.SellOrder.OrderByDescending(o => o.Key).First().Key, out double oPrice) && double.TryParse(avg, out double bAvg) && Math.Abs(bAvg - price) < Const.ErrorRate * 2 * -bt.Quantity ? bt.SendCorrectionOrder(time, (oPrice + Const.ErrorRate * 2).ToString("F2"), sb.Value, quantity) : false;
        }
        double Max(double max, XingAPI.Classification classification)
        {
            var temp = 0D;

            foreach (var kv in bt.Judge)
                temp += kv.Value < 0 ? 0.085 : -0.085;

            return max * ((classification.Equals(XingAPI.Classification.Buy) ? 0.9 : 0.85) - Math.Abs(temp));
        }
        internal Feather(BackTesting bt, Catalog.XingAPI.Specify specify) : base(bt, specify) => Console.WriteLine(specify.Strategy);
    }
}