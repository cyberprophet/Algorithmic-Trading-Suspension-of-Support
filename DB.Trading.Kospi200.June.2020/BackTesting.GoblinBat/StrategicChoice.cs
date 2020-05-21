using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.Strategy.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy
{
    public abstract class StrategicChoice : ReBalancing
    {
        protected internal StrategicChoice(Catalog.XingAPI.Specify specify) : base(specify)
        {
            if (specify.Time == 1440)
            {
                ((IEvents<EventHandler.XingAPI.Quotes>)API.reals[0]).Send += OnReceiveQuotes;
                RollOver = !specify.RollOver;
                ran = new Random();
            }
        }
        protected internal abstract bool SendNewOrder(double[] param, string classification);
        protected internal abstract bool SetCorrectionBuyOrder(string avg, double buy);
        protected internal abstract bool SetCorrectionSellOrder(string avg, double sell);
        protected internal abstract bool ForTheLiquidationOfSellOrder(double[] bid);
        protected internal abstract bool ForTheLiquidationOfSellOrder(string price, double[] bid);
        protected internal abstract bool ForTheLiquidationOfBuyOrder(double[] selling);
        protected internal abstract bool ForTheLiquidationOfBuyOrder(string price, double[] selling);
        void OnReceiveQuotes(object sender, EventHandler.XingAPI.Quotes e)
        {
            if (int.TryParse(e.Time, out int time) && (time < 090007 && time > 045959) == false && (time > 153459 && time < 180000) == false && string.IsNullOrEmpty(API.Classification) == false)
            {
                string classification = API.Classification, price;
                var check = classification.Equals(buy);
                var max = Max(specify.Assets / ((check ? e.Price[5] : e.Price[4]) * Const.TransactionMultiplier * specify.MarginRate), classification);
                double[] sp = new double[10], bp = new double[10];
                API.MaxAmount = max * (classification.Equals(buy) ? 1 : -1);

                for (int i = 0; i < 10; i++)
                    if (double.TryParse((e.Price[5] - Const.ErrorRate * (9 - i)).ToString("F2"), out double bt) && double.TryParse((e.Price[4] + Const.ErrorRate * (9 - i)).ToString("F2"), out double st))
                    {
                        sp[i] = st;
                        bp[i] = bt;
                    }
                if (API.AvgPurchase != null && API.AvgPurchase.Equals(avg) == false && API.OnReceiveBalance && API.Quantity != 0)
                {
                    price = GetExactPrice(API.AvgPurchase);

                    switch (classification)
                    {
                        case sell:
                            if (API.Quantity < 0)
                            {
                                if (API.BuyOrder.Count == 0 && max < 1 - API.Quantity && ForTheLiquidationOfSellOrder(price, bp))
                                    return;

                                if (API.BuyOrder.Count > 0 && ForTheLiquidationOfSellOrder(bp))
                                    return;

                                if (API.SellOrder.Count > 0 && SetCorrectionSellOrder(price, sp[sp.Length - 1]))
                                    return;
                            }
                            else if (API.BuyOrder.Count > 1 && SetBuyDecentralize(bp[bp.Length - 1]))
                                return;

                            break;

                        case buy:
                            if (API.Quantity > 0)
                            {
                                if (API.SellOrder.Count == 0 && max < API.Quantity + 1 && ForTheLiquidationOfBuyOrder(price, sp))
                                    return;

                                if (API.SellOrder.Count > 0 && ForTheLiquidationOfBuyOrder(sp))
                                    return;

                                if (API.BuyOrder.Count > 0 && SetCorrectionBuyOrder(price, bp[bp.Length - 1]))
                                    return;
                            }
                            else if (API.SellOrder.Count > 1 && SetSellDecentralize(sp[sp.Length - 1]))
                                return;

                            break;
                    }
                }
                foreach (var kv in check ? API.BuyOrder : API.SellOrder)
                    if (Array.Exists(check ? bp : sp, o => o == kv.Value) == false && API.OnReceiveBalance && (check ? API.BuyOrder.ContainsKey(kv.Key) : API.SellOrder.ContainsKey(kv.Key)) && SendClearingOrder(kv.Key))
                        return;

                if (API.OnReceiveBalance && SendNewOrder(e.Price, classification))
                    return;
            }
            else if (time > 153559 && time < 154459 && RollOver)
            {
                RollOver = false;
                SendLiquidationOrder();
            }
        }
        double Max(double max, string classification)
        {
            int num = 9;

            foreach (var kv in API.Judge)
            {
                if (classification.Equals(sell) && kv.Value > 0)
                    num--;

                else if (classification.Equals(buy) && kv.Value < 0)
                    num--;
            }
            return max * num * 0.1;
        }
        bool SetBuyDecentralize(double buy)
        {
            var benchmark = API.BuyOrder.OrderBy(o => o.Value).First().Value - Const.ErrorRate * 2;

            return benchmark > buy - Const.ErrorRate * 9 && SendCorrectionOrder(benchmark.ToString("F2"), API.BuyOrder.OrderByDescending(o => o.Value).First().Key);
        }
        bool SetSellDecentralize(double sell)
        {
            var benchmark = API.SellOrder.OrderByDescending(o => o.Value).First().Value + Const.ErrorRate * 2;

            return benchmark < sell + Const.ErrorRate * 9 && SendCorrectionOrder(benchmark.ToString("F2"), API.SellOrder.OrderBy(o => o.Value).First().Key);
        }
        void SendLiquidationOrder()
        {
            foreach (var order in new Dictionary<string, double>[]
            {
                API.SellOrder,
                API.BuyOrder
            })
                foreach (var kv in order)
                    if (SendClearingOrder(kv.Key))
                        Thread.Sleep(ran.Next(999, 5000));

            if (API.Quantity != 0)
                API.orders[0].QueryExcute(new Order
                {
                    FnoIsuNo = ConnectAPI.Code,
                    BnsTpCode = API.Quantity > 0 ? sell : buy,
                    FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.시장가).ToString("D2"),
                    OrdPrc = GetExactPrice(API.AvgPurchase),
                    OrdQty = Math.Abs(API.Quantity).ToString()
                });
        }
        bool RollOver
        {
            get; set;
        }
        protected internal string GetExactPrice(string avg)
        {
            if ((avg.Length < 6 || (avg.Length == 6 && (avg.Substring(5, 1).Equals("0") || avg.Substring(5, 1).Equals("5")))) && double.TryParse(avg, out double price))
                return (API.Quantity > 0 ? price + Const.ErrorRate : price - Const.ErrorRate).ToString("F2");

            if (int.TryParse(avg.Substring(5, 1), out int rest) && double.TryParse(string.Concat(avg.Substring(0, 5), "5"), out double rp))
            {
                if (rest > 0 && rest < 5)
                    return API.Quantity > 0 ? (rp + Const.ErrorRate).ToString("F2") : (rp - Const.ErrorRate * 2).ToString("F2");

                return API.Quantity > 0 ? (rp + Const.ErrorRate * 2).ToString("F2") : (rp - Const.ErrorRate).ToString("F2");
            }
            return avg;
        }
        protected internal bool SendClearingOrder(string number)
        {
            API.OnReceiveBalance = false;
            API.orders[2].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                OrgOrdNo = number,
                OrdQty = sell
            });
            return true;
        }
        protected internal bool SendCorrectionOrder(string price, string number)
        {
            API.OnReceiveBalance = false;
            API.orders[1].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                OrgOrdNo = number,
                FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                OrdPrc = price,
                OrdQty = sell
            });
            return true;
        }
        protected internal bool SendNewOrder(string price, string classification)
        {
            API.OnReceiveBalance = false;
            API.orders[0].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                BnsTpCode = classification,
                FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                OrdPrc = price,
                OrdQty = sell
            });
            return true;
        }
        protected internal const string buy = "2";
        protected internal const string sell = "1";
        protected internal const string avg = "000.00";
        readonly Random ran;
    }
}