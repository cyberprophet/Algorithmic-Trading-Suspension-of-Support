using System;
using System.Linq;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy.XingAPI
{
    public class SuperFly : StrategicChoice
    {
        int GetMaxJudge(double price, bool judge)
        {
            int count = 0;

            while ((judge ? API.MaxAmount - API.Quantity - API.BuyOrder.Count : API.Quantity - API.SellOrder.Count - API.MaxAmount) > 1)
                foreach (var kv in API.TradingJudge.OrderBy(o => o.Key))
                {
                    if (judge && ++count > API.Quantity + API.BuyOrder.Count)
                        return (int)((kv.Value - price) * count);

                    if (judge == false && ++count > API.SellOrder.Count - API.Quantity)
                        return (int)((price - kv.Value) * count);
                }
            return int.MinValue;
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(string price, double[] selling)
        {
            var gap = API.MaxAmount - API.Quantity;

            if (0 < gap && gap < 1)
                foreach (var kv in API.Judge.OrderBy(o => o.Key))
                    if (kv.Value < 0)
                    {
                        var index = selling[selling.Length - 1] - API.TradingJudge[kv.Key];

                        return SendNewOrder(selling[index > 0 && index < 5 ? (int)(selling.Length - index) : selling.Length - 1].ToString("F2"), sell);
                    }
            return selling[selling.Length - 1].ToString("F2").Equals(price) ? SendNewOrder(price, sell) : false;
        }
        protected internal override bool ForTheLiquidationOfSellOrder(string price, double[] bid)
        {
            var gap = API.Quantity - API.MaxAmount;

            if (0 < gap && gap < 1)
                foreach (var kv in API.Judge.OrderBy(o => o.Key))
                    if (kv.Value > 0)
                    {
                        var index = API.TradingJudge[kv.Key] - bid[bid.Length - 1];

                        return SendNewOrder(bid[index > 0 && index < 5 ? (int)(bid.Length - index) : bid.Length - 1].ToString("F2"), buy);
                    }
            return bid[bid.Length - 1].ToString("F2").Equals(price) ? SendNewOrder(price, buy) : false;
        }
        protected internal override bool ForTheLiquidationOfBuyOrder(double[] selling)
        {
            var number = API.SellOrder.OrderByDescending(o => o.Value).First().Key;

            return API.SellOrder.TryGetValue(number, out double csp) && selling[API.SellOrder.Count == 1 ? 5 : (selling.Length - 1)] < csp ? SendClearingOrder(number) : false;
        }
        protected internal override bool ForTheLiquidationOfSellOrder(double[] bid)
        {
            var number = API.BuyOrder.OrderBy(o => o.Value).First().Key;

            return API.BuyOrder.TryGetValue(number, out double cbp) && bid[API.BuyOrder.Count == 1 ? 5 : (bid.Length - 1)] > cbp ? SendClearingOrder(number) : false;
        }
        protected internal override bool SendNewOrder(double[] param, string classification)
        {
            var check = classification.Equals(buy);
            var index = GetMaxJudge(param[check ? 5 : 4], check);
            var price = check ? param[5 + index] : param[4 - index];

            return index < 5 && index >= 0 && (check ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)) == false ? SendNewOrder(price.ToString("F2"), classification) : false;
        }
        protected internal override bool SetCorrectionBuyOrder(string avg, double buy)
        {
            var count = 0;

            if (double.TryParse(avg, out double rAvg) && Math.Abs(rAvg - buy) > Const.ErrorRate)
                foreach (var kv in API.Judge.OrderByDescending(o => o.Key))
                    if (API.MaxAmount - API.Quantity > count++ && kv.Value > 0 && API.TradingJudge[kv.Key] < buy && API.BuyOrder.ContainsValue(buy) == false)
                        return SendCorrectionOrder(buy.ToString("F2"), API.BuyOrder.OrderBy(o => o.Value).First().Key);

            return false;
        }
        protected internal override bool SetCorrectionSellOrder(string avg, double sell)
        {
            var count = 0;

            if (double.TryParse(avg, out double rAvg) && Math.Abs(rAvg - sell) > Const.ErrorRate)
                foreach (var kv in API.Judge.OrderByDescending(o => o.Key))
                    if (API.Quantity - API.MaxAmount > count++ && kv.Value < 0 && API.TradingJudge[kv.Key] > sell && API.SellOrder.ContainsValue(sell) == false)
                        return SendCorrectionOrder(sell.ToString("F2"), API.SellOrder.OrderByDescending(o => o.Value).First().Key);

            return false;
        }
        public SuperFly(Catalog.XingAPI.Specify specify) : base(specify) => API.OnReceiveBalance = false;
    }
}