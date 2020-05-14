using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.XingAPI
{
    public class SuperFly : StrategicChoice
    {
        protected internal override bool ForTheLiquidationOfBuyOrder(double[] selling)
        {
            var number = API.SellOrder.OrderByDescending(o => o.Value).First().Key;

            return API.SellOrder.TryGetValue(number, out double csp) && selling[API.SellOrder.Count == 1 ? 5 : (selling.Length - 1)] < csp ? SendClearingOrder(number) : false;
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(string price, double[] selling) => double.TryParse(price, out double sAvg) && sAvg < selling[5] ? SendNewOrder(sAvg < selling[selling.Length - 1] ? selling[selling.Length - 1].ToString("F2") : price, sell) : false;
        protected internal override bool ForTheLiquidationOfSellOrder(double[] bid)
        {
            var number = API.BuyOrder.OrderBy(o => o.Value).First().Key;

            return API.BuyOrder.TryGetValue(number, out double cbp) && bid[API.BuyOrder.Count == 1 ? 5 : (bid.Length - 1)] > cbp ? SendClearingOrder(number) : false;
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string price, double[] bid) => double.TryParse(price, out double bAvg) && bAvg > bid[5] ? SendNewOrder(bAvg > bid[bid.Length - 1] ? bid[bid.Length - 1].ToString("F2") : price, buy) : false;
        protected internal override bool SendNewOrder(double[] param, string classification)
        {
            var check = classification.Equals(buy);
            var price = param[check ? param.Length - 1 : 0];

            if (check && API.Quantity < 0 && param[5] > 0 && API.BuyOrder.Count < -API.Quantity && API.BuyOrder.ContainsValue(param[5]) == false)
                return SendNewOrder(param[5].ToString("F2"), classification);

            else if (check == false && API.Quantity > 0 && param[4] > 0 && API.SellOrder.Count < API.Quantity && API.SellOrder.ContainsValue(param[4]) == false)
                return SendNewOrder(param[4].ToString("F2"), classification);

            return price > 0 && ((check ? API.BuyOrder.Count == 0 : API.SellOrder.Count == 0) || API.Quantity == 0) && (check ? API.Quantity + API.BuyOrder.Count : API.SellOrder.Count - API.Quantity) < specify.Assets / (price * Const.TransactionMultiplier * specify.MarginRate) - 1 && (check ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)) == false ? SendNewOrder(price.ToString("F2"), classification) : false;
        }
        protected internal override bool SetCorrectionBuyOrder(string avg, double buy)
        {
            var sb = API.BuyOrder.OrderByDescending(o => o.Value).First();
            double abscond = API.Quantity * Const.ErrorRate, oPrice = sb.Value - abscond;

            return oPrice > buy - Const.ErrorRate * 9 && double.TryParse(avg, out double sAvg) && buy < sAvg && sAvg - abscond < sb.Value + Const.ErrorRate && API.BuyOrder.ContainsValue(oPrice) == false ? SendCorrectionOrder(oPrice.ToString("F2"), sb.Key) : false;
        }
        protected internal override bool SetCorrectionSellOrder(string avg, double sell)
        {
            var sb = API.SellOrder.OrderBy(o => o.Value).First();
            double abscond = API.Quantity * Const.ErrorRate, oPrice = sb.Value - abscond;

            return oPrice < sell + Const.ErrorRate * 9 && double.TryParse(avg, out double bAvg) && sell > bAvg && bAvg - abscond > sb.Value - Const.ErrorRate && API.SellOrder.ContainsValue(oPrice) == false ? SendCorrectionOrder(oPrice.ToString("F2"), sb.Key) : false;
        }
        public SuperFly(Catalog.XingAPI.Specify specify) : base(specify) => API.OnReceiveBalance = false;
    }
}