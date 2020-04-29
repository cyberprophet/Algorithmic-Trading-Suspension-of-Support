using System;
using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.XingAPI
{
    public class Feather : StrategicChoice
    {
        double Max(double max, Classification classification)
        {
            var temp = 0D;

            foreach (var kv in API.Judge)
                temp += kv.Value < 0 ? 0.085 : -0.085;

            return max * ((classification.Equals(Classification.Buy) ? 1 : 0.85) - Math.Abs(temp));
        }
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

            return price > 0 && (check ? API.Quantity + API.BuyOrder.Count : API.SellOrder.Count - API.Quantity) < Max(specify.Assets / (price * Const.TransactionMultiplier * specify.MarginRate), check ? Classification.Buy : Classification.Sell) && (check ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)) == false ? SendNewOrder(price.ToString("F2"), classification) : false;
        }
        protected internal override bool SetCorrectionBuyOrder(string avg, double buy)
        {
            var sb = API.BuyOrder.OrderByDescending(o => o.Value).First();

            return double.TryParse(avg, out double sAvg) && API.OnReceiveBalance && Math.Abs(sAvg - sb.Value) < Const.ErrorRate * 2 * API.Quantity ? SendCorrectionOrder((API.BuyOrder.OrderBy(o => o.Value).First().Value - Const.ErrorRate * 2).ToString("F2"), sb.Key) : false;
        }
        protected internal override bool SetCorrectionSellOrder(string avg, double sell)
        {
            var sb = API.SellOrder.OrderBy(o => o.Value).First();

            return double.TryParse(avg, out double bAvg) && API.OnReceiveBalance && Math.Abs(bAvg - sb.Value) < Const.ErrorRate * 2 * API.Quantity ? SendCorrectionOrder((API.SellOrder.OrderByDescending(o => o.Value).First().Value + Const.ErrorRate * 2).ToString("F2"), sb.Key) : false;
        }
        public Feather(Catalog.XingAPI.Specify specify) : base(specify) => API.OnReceiveBalance = false;
    }
}