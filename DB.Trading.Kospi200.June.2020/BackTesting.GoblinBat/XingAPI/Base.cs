using System;
using System.Linq;
using System.Threading.Tasks;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class Base : ReBalancing
    {
        public Base(Catalog.XingAPI.Specify specify) : base(specify)
        {
            API.OnReceiveBalance = false;

            if (specify.Time == 1440)
            {
                ((IEvents<EventHandler.XingAPI.Quotes>)API.reals[0]).Send += OnReceiveQuotes;
            }
        }
        void OnReceiveQuotes(object sender, EventHandler.XingAPI.Quotes e)
        {
            if (int.TryParse(e.Time, out int time) && (time < 090000 && time > 045959) == false && (time > 153459 && time < 180000) == false && string.IsNullOrEmpty(API.Classification) == false)
            {
                string classification = API.Classification, price;
                var check = classification.Equals(buy);
                var max = Max(specify.Assets / ((check ? e.Price[5] : e.Price[4]) * Const.TransactionMultiplier * Const.MarginRate200402), classification);
                double[] sp = new double[10], bp = new double[10];
                API.MaxAmount = max * (classification.Equals(buy) ? 1 : -1);

                for (int i = 0; i < 10; i++)
                    if (double.TryParse((e.Price[5] - Const.ErrorRate * (9 - i)).ToString("F2"), out double bt) && double.TryParse((e.Price[4] + Const.ErrorRate * (9 - i)).ToString("F2"), out double st))
                    {
                        sp[i] = st;
                        bp[i] = bt;
                    }
                if (API.AvgPurchase != null && API.AvgPurchase.Equals(avg) == false)
                {
                    price = GetExactPrice(API.AvgPurchase);

                    switch (classification)
                    {
                        case sell:
                            if (API.OnReceiveBalance && API.Quantity < 0)
                            {
                                if (API.BuyOrder.Count == 0 && max < -API.Quantity && double.TryParse(price, out double bAvg) && bAvg > bp[5])
                                {
                                    SendNewOrder(bAvg > bp[bp.Length - 1] ? bp[bp.Length - 1].ToString("F2") : price, buy);

                                    return;
                                }
                                if (API.BuyOrder.Count > 0)
                                {
                                    var number = API.BuyOrder.OrderBy(o => o.Value).First().Key;

                                    if (API.BuyOrder.TryGetValue(number, out double cbp) && bp[API.BuyOrder.Count == 1 ? 5 : (bp.Length - 1)] > cbp)
                                    {
                                        SendClearingOrder(number);

                                        return;
                                    }
                                }
                                if (API.SellOrder.Count > 0 && SetCorrectionSellOrder(price, sp[sp.Length - 1]))
                                    return;
                            }
                            break;

                        case buy:
                            if (API.OnReceiveBalance && API.Quantity > 0)
                            {
                                if (API.SellOrder.Count == 0 && max < API.Quantity && double.TryParse(price, out double sAvg) && sAvg < sp[5])
                                {
                                    SendNewOrder(sAvg < sp[sp.Length - 1] ? sp[sp.Length - 1].ToString("F2") : price, sell);

                                    return;
                                }
                                if (API.SellOrder.Count > 0)
                                {
                                    var number = API.SellOrder.OrderByDescending(o => o.Value).First().Key;

                                    if (API.SellOrder.TryGetValue(number, out double csp) && sp[API.SellOrder.Count == 1 ? 5 : (sp.Length - 1)] < csp)
                                    {
                                        SendClearingOrder(number);

                                        return;
                                    }
                                }
                                if (API.BuyOrder.Count > 0 && SetCorrectionBuyOrder(price, bp[bp.Length - 1]))
                                    return;
                            }
                            break;
                    }
                }
                foreach (var kv in check ? API.BuyOrder : API.SellOrder)
                    if (Array.Exists(check ? bp : sp, o => o == kv.Value) == false && API.OnReceiveBalance && (check ? API.BuyOrder.ContainsKey(kv.Key) : API.SellOrder.ContainsKey(kv.Key)))
                    {
                        SendClearingOrder(kv.Key);

                        return;
                    }
                if (API.OnReceiveBalance)
                    SendNewOrder(e.Price, classification);
            }
        }
        string GetExactPrice(string avg)
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
        double Max(double max, string classification)
        {
            int num = 9;

            foreach (var kv in API.Judge)
            {
                if (classification.Equals(sell) && kv.Value > 0)
                    num -= 2;

                else if (classification.Equals(buy) && kv.Value < 0)
                    num -= 2;
            }
            return max * num * 0.1;
        }
        double Max(double max, Classification classification)
        {
            int num = 1;

            foreach (var kv in API.Judge)
                switch (classification)
                {
                    case Classification.Sell:
                        if (kv.Value > 0)
                            num += 2;

                        break;

                    case Classification.Buy:
                        if (kv.Value < 0)
                            num += 2;

                        break;
                }
            return max * num * 0.1;
        }
        bool SetCorrectionBuyOrder(string avg, double buy)
        {
            if (double.TryParse(avg, out double sAvg) && double.TryParse(GetExactPrice((((sAvg - Const.ErrorRate) * API.Quantity + buy) / (API.Quantity + 1)).ToString("F2")), out double prospect))
            {
                var order = API.BuyOrder.OrderBy(o => o.Value).First();
                var sb = API.BuyOrder.OrderByDescending(o => o.Value).First();
                double check = prospect - Const.ErrorRate, price = order.Value - Const.ErrorRate;

                if (sb.Value > check && API.BuyOrder.ContainsValue(price) == false && API.OnReceiveBalance)
                {
                    SendCorrectionOrder(price.ToString("F2"), sb.Key);

                    return true;
                }
                if (prospect < sAvg && prospect < buy + Const.ErrorRate && API.BuyOrder.ContainsValue(check) == false && API.OnReceiveBalance)
                {
                    SendCorrectionOrder(check.ToString("F2"), order.Key);

                    return true;
                }
            }
            return false;
        }
        bool SetCorrectionSellOrder(string avg, double sell)
        {
            if (double.TryParse(avg, out double bAvg) && double.TryParse(GetExactPrice(((sell - (bAvg + Const.ErrorRate) * API.Quantity) / (1 - API.Quantity)).ToString("F2")), out double prospect))
            {
                var order = API.SellOrder.OrderByDescending(o => o.Value).First();
                var sb = API.SellOrder.OrderBy(o => o.Value).First();
                double check = prospect + Const.ErrorRate, price = order.Value + Const.ErrorRate;

                if (sb.Value < check && API.SellOrder.ContainsValue(price) == false && API.OnReceiveBalance)
                {
                    SendCorrectionOrder(price.ToString("F2"), sb.Key);

                    return true;
                }
                if (prospect > bAvg && prospect > sell - Const.ErrorRate && API.SellOrder.ContainsValue(check) == false && API.OnReceiveBalance)
                {
                    SendCorrectionOrder(check.ToString("F2"), order.Key);

                    return true;
                }
            }
            return false;
        }
        void SendClearingOrder(string number)
        {
            API.OnReceiveBalance = false;
            new Task(() => API.orders[2].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                OrgOrdNo = number,
                OrdQty = specify.Quantity
            })).Start();
        }
        void SendCorrectionOrder(string price, string number)
        {
            API.OnReceiveBalance = false;
            new Task(() => API.orders[1].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                OrgOrdNo = number,
                FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                OrdPrc = price,
                OrdQty = specify.Quantity
            })).Start();
        }
        void SendNewOrder(string price, string classification)
        {
            API.OnReceiveBalance = false;
            new Task(() => API.orders[0].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                BnsTpCode = classification,
                FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                OrdPrc = price,
                OrdQty = specify.Quantity
            })).Start();
        }
        void SendNewOrder(double[] param, string classification)
        {
            var check = classification.Equals(buy);
            var price = param[check ? param.Length - 1 : 0];

            if (price > 0 && (check ? API.Quantity + API.BuyOrder.Count : API.SellOrder.Count - API.Quantity) < Max(specify.Assets / (price * Const.TransactionMultiplier * Const.MarginRate200402), check ? Classification.Buy : Classification.Sell) && (check ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)) == false)
            {
                API.OnReceiveBalance = false;
                new Task(() => API.orders[0].QueryExcute(new Order
                {
                    FnoIsuNo = ConnectAPI.Code,
                    BnsTpCode = classification,
                    FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                    OrdPrc = price.ToString("F2"),
                    OrdQty = specify.Quantity
                })).Start();
            }
        }
        const string buy = "2";
        const string sell = "1";
        const string avg = "000.00";
    }
}