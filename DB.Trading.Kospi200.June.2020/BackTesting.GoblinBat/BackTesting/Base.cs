using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.Statistics
{
    class Base : Analysis
    {
        protected internal override bool SetCorrectionBuyOrder(string avg, double buy, int quantity)
        {
            if (double.TryParse(avg, out double sAvg) && double.TryParse(GetExactPrice((((sAvg - Const.ErrorRate) * bt.Quantity + buy) / (bt.Quantity + 1)).ToString("F2")), out double prospect))
            {
                var order = bt.BuyOrder.OrderBy(o => o.Key).First();
                var sb = bt.BuyOrder.OrderByDescending(o => o.Key).First();
                double check = prospect - Const.ErrorRate;

                if (double.TryParse(order.Key, out double ok) && double.TryParse(sb.Key, out double sk) && sk > check && bt.BuyOrder.ContainsKey((ok - Const.ErrorRate).ToString("F2")) == false)
                {
                    bt.SendCorrectionOrder((ok - Const.ErrorRate).ToString("F2"), sb.Value, quantity);

                    return true;
                }
                if (prospect < sAvg && prospect < buy + Const.ErrorRate && bt.BuyOrder.ContainsKey(check.ToString("F2")) == false)
                {
                    bt.SendCorrectionOrder(check.ToString("F2"), order.Value, quantity);

                    return true;
                }
            }
            return false;
        }
        protected internal override bool SetCorrectionSellOrder(string avg, double sell, int quantity)
        {
            if (double.TryParse(avg, out double bAvg) && double.TryParse(GetExactPrice(((sell - (bAvg + Const.ErrorRate) * bt.Quantity) / (1 - bt.Quantity)).ToString("F2")), out double prospect))
            {
                var order = bt.SellOrder.OrderByDescending(o => o.Key).First();
                var sb = bt.SellOrder.OrderBy(o => o.Key).First();
                double check = prospect + Const.ErrorRate;

                if (double.TryParse(order.Key, out double ok) && double.TryParse(sb.Key, out double sk) && sk < check && bt.SellOrder.ContainsKey((ok + Const.ErrorRate).ToString("F2")) == false)
                {
                    bt.SendCorrectionOrder((ok + Const.ErrorRate).ToString("F2"), sb.Value, quantity);

                    return true;
                }
                if (prospect > bAvg && prospect > sell - Const.ErrorRate && bt.SellOrder.ContainsKey(check.ToString("F2")) == false)
                {
                    bt.SendCorrectionOrder(check.ToString("F2"), order.Value, quantity);

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

            if (price > 0 && (check ? bt.Quantity + bt.BuyOrder.Count : bt.SellOrder.Count - bt.Quantity) < Max(specify.Assets / (price * Const.TransactionMultiplier * Const.MarginRate200402), check ? XingAPI.Classification.Buy : XingAPI.Classification.Sell) && (check ? bt.BuyOrder.ContainsKey(key) : bt.SellOrder.ContainsKey(key)) == false)
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
                            num += 2;

                        break;

                    case XingAPI.Classification.Buy:
                        if (kv.Value < 0)
                            num += 2;

                        break;
                }
            return max * num * 0.1;
        }
        internal Base(BackTesting bt, Catalog.XingAPI.Specify specify) : base(bt, specify)
        {

        }
    }
}