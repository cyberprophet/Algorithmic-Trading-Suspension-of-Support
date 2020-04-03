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
                ((IEvents<EventHandler.XingAPI.Quotes>)API.reals[0]).Send += OnReceiveQuotes;
        }
        private void OnReceiveQuotes(object sender, EventHandler.XingAPI.Quotes e)
        {
            if (int.TryParse(e.Time, out int time) && (time > 153459 && time < 180000) == false && string.IsNullOrEmpty(API.Classification) == false)
            {
                string classification = API.Classification;
                var check = classification.Equals(buy);
                var max = Max(specify.Assets / ((check ? e.Price[5] : e.Price[4]) * Const.TransactionMultiplier * Const.MarginRate200402), classification);
                int i, being = (int)max;
                double[] sp = new double[5], bp = new double[5];
                API.MaxAmount = max * (classification.Equals(buy) ? 1 : -1);

                for (i = 0; i < 5; i++)
                {
                    sp[i] = e.Price[i];
                    bp[i] = e.Price[9 - i];
                }
                switch (classification)
                {
                    case sell:
                        if (API.OnReceiveBalance && API.Quantity < 0 && API.AvgPurchase != null && API.AvgPurchase.Equals(avg) == false)
                        {
                            var price = GetExactPrice(API.AvgPurchase);

                            switch (API.BuyOrder.Count)
                            {
                                case 0:
                                    if (API.OnReceiveBalance && price.Equals(Price) == false)
                                    {
                                        SendNewOrder(price, buy);
                                        Price = price;

                                        return;
                                    }
                                    else if (API.OnReceiveBalance && max < Math.Abs(API.Quantity))
                                    {
                                        SendNewOrder(e.Price, max, buy);

                                        return;
                                    }
                                    break;

                                case 1:
                                    var number = API.BuyOrder.First().Key;

                                    if (API.BuyOrder.TryGetValue(number, out double cbp))
                                        if (cbp.ToString("F2").Equals(price) == false && API.OnReceiveBalance)
                                        {
                                            SendCorrectionOrder(price, number);

                                            return;
                                        }
                                        else if (Array.Exists(bp, o => o == cbp) == false && API.OnReceiveBalance)
                                        {
                                            SendClearingOrder(number);

                                            return;
                                        }
                                    break;

                                default:
                                    var order = API.BuyOrder.First(f => f.Value == API.BuyOrder.Max(o => o.Value)).Key;

                                    if (API.OnReceiveBalance && API.BuyOrder.ContainsKey(order))
                                    {
                                        SendClearingOrder(order);

                                        return;
                                    }
                                    break;
                            }
                        }
                        for (i = 4; i > -1; i--)
                            if (being + API.Quantity <= i && API.SellOrder.ContainsValue(sp[i]))
                            {
                                var number = API.SellOrder.First(o => o.Value == API.SellOrder.Min(m => m.Value)).Key;
                                var price = API.SellOrder.Max(o => o.Value) + Const.ErrorRate;

                                if (API.OnReceiveBalance && API.SellOrder.ContainsKey(number))
                                {
                                    SendCorrectionOrder(price.ToString("F2"), number);

                                    return;
                                }
                            }
                        break;

                    case buy:
                        if (API.OnReceiveBalance && API.Quantity > 0 && API.AvgPurchase != null && API.AvgPurchase.Equals(avg) == false)
                        {
                            var price = GetExactPrice(API.AvgPurchase);

                            switch (API.SellOrder.Count)
                            {
                                case 0:
                                    if (API.OnReceiveBalance && (price.Equals(Price) == false))
                                    {
                                        SendNewOrder(price, sell);
                                        Price = price;

                                        return;
                                    }
                                    else if (API.OnReceiveBalance && max < API.Quantity)
                                    {
                                        SendNewOrder(e.Price, max, sell);

                                        return;
                                    }
                                    break;

                                case 1:
                                    var number = API.SellOrder.First().Key;

                                    if (API.SellOrder.TryGetValue(number, out double csp))
                                        if (csp.ToString("F2").Equals(price) == false && API.OnReceiveBalance)
                                        {
                                            SendCorrectionOrder(price, number);

                                            return;
                                        }
                                        else if (Array.Exists(sp, o => o == csp) == false && API.OnReceiveBalance)
                                        {
                                            SendClearingOrder(number);

                                            return;
                                        }
                                    break;

                                default:
                                    var order = API.SellOrder.First(f => f.Value == API.SellOrder.Min(o => o.Value)).Key;

                                    if (API.OnReceiveBalance && API.SellOrder.ContainsKey(order))
                                    {
                                        SendClearingOrder(order);

                                        return;
                                    }
                                    break;
                            }
                        }
                        for (i = 4; i > -1; i--)
                            if (being - API.Quantity <= i && API.BuyOrder.ContainsValue(bp[i]))
                            {
                                var number = API.BuyOrder.First(o => o.Value == API.BuyOrder.Max(m => m.Value)).Key;
                                var price = API.BuyOrder.Min(o => o.Value) - Const.ErrorRate;

                                if (API.OnReceiveBalance && API.BuyOrder.ContainsKey(number))
                                {
                                    SendCorrectionOrder(price.ToString("F2"), number);

                                    return;
                                }
                            }
                        break;
                }
                foreach (var kv in check ? API.BuyOrder : API.SellOrder)
                    if (Array.Exists(check ? bp : sp, o => o == kv.Value) == false && API.OnReceiveBalance && (check ? API.BuyOrder.ContainsKey(kv.Key) : API.SellOrder.ContainsKey(kv.Key)))
                    {
                        SendClearingOrder(kv.Key);

                        return;
                    }
                if (API.OnReceiveBalance)
                    SendNewOrder(e.Price, max, classification);
            }
        }
        private string GetExactPrice(string avg)
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
        private double Max(double max, string classification)
        {
            int num = 5;

            foreach (var kv in API.Judge)
            {
                if (classification.Equals(sell) && kv.Value > 0)
                    num--;

                else if (classification.Equals(buy) && kv.Value < 0)
                    num--;
            }
            return max * num * 0.2;
        }
        private void SendClearingOrder(string number)
        {
            API.OnReceiveBalance = false;
            new Task(() => API.orders[2].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                OrgOrdNo = number,
                OrdQty = specify.Quantity
            })).Start();
        }
        private void SendCorrectionOrder(string price, string number)
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
        private void SendNewOrder(string price, string classification)
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
        private void SendNewOrder(double[] param, double max, string classification)
        {
            var price = param[classification.Equals(buy) ? 5 : 4];

            if (price > 0 && (classification.Equals(buy) ? API.Quantity + API.BuyOrder.Count : API.SellOrder.Count - API.Quantity) < max && (classification.Equals(buy) ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)) == false)
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
        private string Price
        {
            get; set;
        }
        private const string buy = "2";
        private const string sell = "1";
        private const string avg = "000.00";
    }
}