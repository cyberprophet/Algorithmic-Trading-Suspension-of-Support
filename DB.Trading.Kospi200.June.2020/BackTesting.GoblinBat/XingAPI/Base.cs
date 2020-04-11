using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.XingAPI
{
    public class Base : StrategicChoice
    {
        double Max(double max, Classification classification)
        {
            int num = 1;

            foreach (var kv in API.Judge)
                switch (classification)
                {
                    case Classification.Sell:
                        if (kv.Value > 0)
                            num++;

                        break;

                    case Classification.Buy:
                        if (kv.Value < 0)
                            num++;

                        break;
                }
            return max * num * 0.1;
        }
        protected internal override bool SetCorrectionBuyOrder(string avg, double buy)
        {
            var order = API.BuyOrder.OrderBy(o => o.Value).First();
            var sb = API.BuyOrder.OrderByDescending(o => o.Value).First();

            if (double.TryParse(avg, out double sAvg) && double.TryParse(GetExactPrice((((sAvg - Const.ErrorRate) * API.Quantity + sb.Value) / (API.Quantity + 1)).ToString("F2")), out double prospect))
            {
                double check = prospect - Const.ErrorRate, abscond = order.Value - Const.ErrorRate, chase = sb.Value + Const.ErrorRate;

                if (buy < check && sAvg > check && API.BuyOrder.ContainsValue(abscond) == false && sb.Value > buy - Const.ErrorRate * 2 && API.OnReceiveBalance)
                {
                    SendCorrectionOrder(abscond.ToString("F2"), sb.Key);

                    return true;
                }
                if (buy > check && buy < sAvg && API.BuyOrder.ContainsValue(chase) == false && sb.Value < buy - Const.ErrorRate * 5 && API.OnReceiveBalance)
                {
                    SendCorrectionOrder(chase.ToString("F2"), order.Key);

                    return true;
                }
            }
            return false;
        }
        protected internal override bool SetCorrectionSellOrder(string avg, double sell)
        {
            var order = API.SellOrder.OrderByDescending(o => o.Value).First();
            var sb = API.SellOrder.OrderBy(o => o.Value).First();

            if (double.TryParse(avg, out double bAvg) && double.TryParse(GetExactPrice(((sb.Value - (bAvg + Const.ErrorRate) * API.Quantity) / (1 - API.Quantity)).ToString("F2")), out double prospect))
            {
                double check = prospect + Const.ErrorRate, abscond = order.Value + Const.ErrorRate, chase = sb.Value - Const.ErrorRate;

                if (sell > check && bAvg < check && API.SellOrder.ContainsValue(abscond) == false && sb.Value < sell + Const.ErrorRate * 2 && API.OnReceiveBalance)
                {
                    SendCorrectionOrder(abscond.ToString("F2"), sb.Key);

                    return true;
                }
                if (sell < check && sell > bAvg && API.SellOrder.ContainsValue(chase) == false && sb.Value > sell + Const.ErrorRate * 5 && API.OnReceiveBalance)
                {
                    SendCorrectionOrder(chase.ToString("F2"), order.Key);

                    return true;
                }
            }
            return false;
        }
        protected internal override void SendNewOrder(double[] param, string classification)
        {
            var check = classification.Equals(buy);
            var price = param[check ? param.Length - 1 : 0];

            if (price > 0 && (check ? API.Quantity + API.BuyOrder.Count : API.SellOrder.Count - API.Quantity) < Max(specify.Assets / (price * Const.TransactionMultiplier * Const.MarginRate200402), check ? Classification.Buy : Classification.Sell) && (check ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)) == false)
                SendNewOrder(price.ToString("F2"), classification);
        }
        public Base(Catalog.XingAPI.Specify specify) : base(specify) => API.OnReceiveBalance = false;
    }
}