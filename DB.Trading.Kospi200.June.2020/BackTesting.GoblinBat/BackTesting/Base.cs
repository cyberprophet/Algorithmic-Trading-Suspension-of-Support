using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.Statistics
{
    class Base : Analysis
    {
        protected internal override bool SetCorrectionBuyOrder(string avg, double buy, int quantity)
        {
            var order = bt.BuyOrder.OrderBy(o => o.Key).First();
            var sb = bt.BuyOrder.OrderByDescending(o => o.Key).First();

            if (double.TryParse(order.Key, out double oPrice) && double.TryParse(sb.Key, out double sPrice) && double.TryParse(avg, out double sAvg) && double.TryParse(GetExactPrice((((sAvg - Const.ErrorRate) * bt.Quantity + sPrice) / (bt.Quantity + 1)).ToString("F2")), out double prospect))
            {
                double check = prospect - Const.ErrorRate, abscond = oPrice - Const.ErrorRate, chase = sPrice + Const.ErrorRate;

                if (buy < check && sAvg > check && bt.BuyOrder.ContainsKey(abscond.ToString("F2")) == false && sPrice > buy - Const.ErrorRate * 2)
                {
                    bt.SendCorrectionOrder(abscond.ToString("F2"), sb.Value, quantity);

                    return true;
                }
                if (buy > check && buy < sAvg && bt.BuyOrder.ContainsKey(chase.ToString("F2")) == false && sPrice < buy - Const.ErrorRate * 5)
                {
                    bt.SendCorrectionOrder(chase.ToString("F2"), order.Value, quantity);

                    return true;
                }
            }
            return false;
        }
        protected internal override bool SetCorrectionSellOrder(string avg, double sell, int quantity)
        {
            var order = bt.SellOrder.OrderByDescending(o => o.Key).First();
            var sb = bt.SellOrder.OrderBy(o => o.Key).First();

            if (double.TryParse(order.Key, out double oPrice) && double.TryParse(sb.Key, out double sPrice) && double.TryParse(avg, out double bAvg) && double.TryParse(GetExactPrice(((sPrice - (bAvg + Const.ErrorRate) * bt.Quantity) / (1 - bt.Quantity)).ToString("F2")), out double prospect))
            {
                double check = prospect + Const.ErrorRate, abscond = oPrice + Const.ErrorRate, chase = sPrice - Const.ErrorRate;

                if (sell > check && bAvg < check && bt.SellOrder.ContainsKey(abscond.ToString("F2")) == false && sPrice < sell + Const.ErrorRate * 2)
                {
                    bt.SendCorrectionOrder(abscond.ToString("F2"), sb.Value, quantity);

                    return true;
                }
                if (sell < check && sell > bAvg && bt.SellOrder.ContainsKey(chase.ToString("F2")) == false && sPrice > sell + Const.ErrorRate * 5)
                {
                    bt.SendCorrectionOrder(chase.ToString("F2"), order.Value, quantity);

                    return true;
                }
            }
            return false;
        }
        protected internal override void SendNewOrder(double[] param, string classification, int residue)
        {
            var check = classification.Equals(buy);
            var price = param[5];
            var key = price.ToString("F2");

            if (price > 0 && (check ? bt.Quantity + bt.BuyOrder.Count : bt.SellOrder.Count - bt.Quantity) < Max(specify.Assets / (price * Const.TransactionMultiplier * specify.MarginRate), check ? XingAPI.Classification.Buy : XingAPI.Classification.Sell) && (check ? bt.BuyOrder.ContainsKey(key) : bt.SellOrder.ContainsKey(key)) == false)
                bt.SendNewOrder(key, classification, residue);
        }
        double Max(double max, XingAPI.Classification classification)
        {
            int num = 1;

            foreach (var kv in bt.Judge)
                switch (classification)
                {
                    case XingAPI.Classification.Sell:
                        if (kv.Value > 0)
                            num++;

                        break;

                    case XingAPI.Classification.Buy:
                        if (kv.Value < 0)
                            num++;

                        break;
                }
            return max * num * 0.1;
        }
        internal Base(BackTesting bt, Catalog.XingAPI.Specify specify) : base(bt, specify)
        {
            
        }
    }
}