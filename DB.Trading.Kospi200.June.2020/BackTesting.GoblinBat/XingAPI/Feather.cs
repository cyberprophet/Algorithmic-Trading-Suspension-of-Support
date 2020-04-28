using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.XingAPI
{
    public class Feather : StrategicChoice
    {
        double Max(double max, Classification classification)
        {
            var num = 5.5D;

            foreach (var kv in API.Judge)
                switch (classification)
                {
                    case Classification.Sell:
                        if (kv.Value > 0)
                            num += 0.5;

                        break;

                    case Classification.Buy:
                        if (kv.Value < 0)
                            num += 0.5;

                        break;
                }
            return max * num * 0.1;
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(double[] selling)
        {
            if (API.Quantity < -1)
                return SendNewOrder(selling[selling.Length - 1]);

            var number = API.SellOrder.OrderByDescending(o => o.Value).First().Key;

            if (API.SellOrder.TryGetValue(number, out double csp) && selling[API.SellOrder.Count == 1 ? 5 : (selling.Length - 1)] < csp)
                return SendClearingOrder(number);

            return false;
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(string price, double[] selling)
        {
            if (double.TryParse(price, out double sAvg) && sAvg < selling[5])
                return SendNewOrder(sAvg < selling[selling.Length - 1] ? selling[selling.Length - 1].ToString("F2") : price, sell);

            return false;
        }
        protected internal override bool ForTheLiquidationOfSellOrder(double[] bid)
        {
            if (API.Quantity > 1)
                return SendNewOrder(bid[bid.Length - 1]);

            var number = API.BuyOrder.OrderBy(o => o.Value).First().Key;

            if (API.BuyOrder.TryGetValue(number, out double cbp) && bid[API.BuyOrder.Count == 1 ? 5 : (bid.Length - 1)] > cbp)
                return SendClearingOrder(number);

            return false;
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string price, double[] bid)
        {
            if (double.TryParse(price, out double bAvg) && bAvg > bid[5])
                return SendNewOrder(bAvg > bid[bid.Length - 1] ? bid[bid.Length - 1].ToString("F2") : price, buy);

            return false;
        }
        protected internal override bool SendNewOrder(double[] param, string classification)
        {
            var check = classification.Equals(buy);
            var price = param[check ? param.Length - 1 : 0];

            if (price > 0 && (check ? API.Quantity + API.BuyOrder.Count : API.SellOrder.Count - API.Quantity) < Max(specify.Assets / (price * Const.TransactionMultiplier * specify.MarginRate), check ? Classification.Buy : Classification.Sell) && (check ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)) == false)
                return SendNewOrder(price.ToString("F2"), classification);

            return false;
        }
        protected internal override bool SetCorrectionBuyOrder(string avg, double buy)
        {
            var order = API.BuyOrder.OrderBy(o => o.Value).First();
            var sb = API.BuyOrder.OrderByDescending(o => o.Value).First();

            if (API.SellOrder.Count > 1)
            {
                var benchmark = API.SellOrder.OrderByDescending(o => o.Value).First().Value + Const.ErrorRate * 2;

                if (benchmark < buy + Const.ErrorRate * 11)
                    return SendCorrectionOrder(benchmark.ToString("F2"), API.SellOrder.OrderBy(o => o.Value).First().Key);
            }
            if (double.TryParse(avg, out double sAvg) && double.TryParse(GetExactPrice((((sAvg - Const.ErrorRate) * API.Quantity + sb.Value) / (API.Quantity + 1)).ToString("F2")), out double prospect))
            {
                double check = prospect - Const.ErrorRate, abscond = order.Value - Const.ErrorRate, chase = sb.Value + Const.ErrorRate, confirm = check - buy;

                if (0 < confirm && confirm < 0.2 * API.Quantity && sAvg > check && API.BuyOrder.ContainsValue(abscond) == false && sb.Value > buy - Const.ErrorRate * 2 && API.OnReceiveBalance)
                    return SendCorrectionOrder(abscond.ToString("F2"), sb.Key);

                if (buy > check && buy < sAvg && API.BuyOrder.ContainsValue(chase) == false && sb.Value < buy - Const.ErrorRate * 5 && API.OnReceiveBalance)
                    return SendCorrectionOrder(chase.ToString("F2"), order.Key);
            }
            return false;
        }
        protected internal override bool SetCorrectionSellOrder(string avg, double sell)
        {
            var order = API.SellOrder.OrderByDescending(o => o.Value).First();
            var sb = API.SellOrder.OrderBy(o => o.Value).First();

            if (API.BuyOrder.Count > 1)
            {
                var benchmark = API.BuyOrder.OrderBy(o => o.Value).First().Value - Const.ErrorRate * 2;

                if (benchmark > sell * Const.ErrorRate * 11)
                    return SendCorrectionOrder(benchmark.ToString("F2"), API.BuyOrder.OrderByDescending(o => o.Value).First().Key);
            }
            if (double.TryParse(avg, out double bAvg) && double.TryParse(GetExactPrice(((sb.Value - (bAvg + Const.ErrorRate) * API.Quantity) / (1 - API.Quantity)).ToString("F2")), out double prospect))
            {
                double check = prospect + Const.ErrorRate, abscond = order.Value + Const.ErrorRate, chase = sb.Value - Const.ErrorRate, confirm = sell - check;

                if (0 < confirm && confirm < 0.2 * -API.Quantity && bAvg < check && API.SellOrder.ContainsValue(abscond) == false && sb.Value < sell + Const.ErrorRate * 2 && API.OnReceiveBalance)
                    return SendCorrectionOrder(abscond.ToString("F2"), sb.Key);

                if (sell < check && sell > bAvg && API.SellOrder.ContainsValue(chase) == false && sb.Value > sell + Const.ErrorRate * 5 && API.OnReceiveBalance)
                    return SendCorrectionOrder(chase.ToString("F2"), order.Key);
            }
            return false;
        }
        public Feather(Catalog.XingAPI.Specify specify) : base(specify) => API.OnReceiveBalance = false;
    }
}