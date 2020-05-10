using System;
using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.Statistics
{
    class Fly : Analysis
    {
        protected internal override bool ForTheLiquidationOfBuyOrder(double[] selling)
        {
            var sell = bt.SellOrder.OrderByDescending(o => o.Key).First();

            return double.TryParse(sell.Key, out double csp) && selling[bt.SellOrder.Count == 1 ? 5 : (selling.Length - 1)] < csp ? bt.SendClearingOrder(sell.Value) : false;
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(string price, double[] selling, int quantity) => double.TryParse(price, out double sAvg) && sAvg < selling[5] ? bt.SendNewOrder(sAvg < selling[selling.Length - 1] ? selling[selling.Length - 1].ToString("F2") : price, sell, quantity) : false;
        protected internal override bool ForTheLiquidationOfSellOrder(double[] bid)
        {
            var buy = bt.BuyOrder.OrderBy(o => o.Key).First();

            return double.TryParse(buy.Key, out double cbp) && bid[bt.BuyOrder.Count == 1 ? 5 : (bid.Length - 1)] > cbp ? bt.SendClearingOrder(buy.Value) : false;
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string price, double[] bid, int quantity) => double.TryParse(price, out double bAvg) && bAvg > bid[5] ? bt.SendNewOrder(bAvg > bid[bid.Length - 1] ? bid[bid.Length - 1].ToString("F2") : price, buy, quantity) : false;
        protected internal override void SendNewOrder(double[] param, string classification, int quantity)
        {
            var check = classification.Equals(buy);
            string key = param[5].ToString("F2"), liquidation = param[param.Length - 1].ToString("F2");

            if (check && bt.Quantity < 0 && param[param.Length - 1] > 0 && bt.BuyOrder.Count < -bt.Quantity && bt.BuyOrder.ContainsKey(liquidation) == false)
                bt.SendNewOrder(liquidation, classification, quantity);

            else if (check == false && bt.Quantity > 0 && param[param.Length - 1] > 0 && bt.SellOrder.Count < bt.Quantity && bt.SellOrder.ContainsKey(liquidation) == false)
                bt.SendNewOrder(liquidation, classification, quantity);

            else if (param[5] > 0 && ((check ? bt.BuyOrder.Count == 0 : bt.SellOrder.Count == 0) || bt.Quantity == 0) && (check ? bt.Quantity + bt.BuyOrder.Count : bt.SellOrder.Count - bt.Quantity) < specify.Assets / (param[5] * Const.TransactionMultiplier * specify.MarginRate) - 1 && (check ? bt.BuyOrder.ContainsKey(key) : bt.SellOrder.ContainsKey(key)) == false)
                bt.SendNewOrder(key, classification, quantity);
        }
        protected internal override bool SetCorrectionBuyOrder(string avg, double buy, int quantity)
        {
            var sb = bt.BuyOrder.OrderByDescending(o => o.Key).First();
            var abscond = bt.Quantity * Const.ErrorRate;

            return double.TryParse(sb.Key, out double price) && double.TryParse(avg, out double sAvg) && buy < sAvg && sAvg - abscond < price ? bt.SendCorrectionOrder((price - abscond).ToString("F2"), sb.Value, quantity) : false;
        }
        protected internal override bool SetCorrectionSellOrder(string avg, double sell, int quantity)
        {
            var sb = bt.SellOrder.OrderBy(o => o.Key).First();
            var abscond = bt.Quantity * Const.ErrorRate;

            return double.TryParse(sb.Key, out double price) && double.TryParse(avg, out double bAvg) && sell > bAvg && bAvg - abscond > price ? bt.SendCorrectionOrder((price - abscond).ToString("F2"), sb.Value, quantity) : false;
        }
        internal Fly(BackTesting bt, Catalog.XingAPI.Specify specify) : base(bt, specify) => Console.WriteLine(specify.Strategy);
    }
}