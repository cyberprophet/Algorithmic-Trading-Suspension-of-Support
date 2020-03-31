using System.Linq;
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
            if (int.TryParse(e.Time, out int time) && (time > 153459 && time < 180000) == false)
            {
                bool accumulate = e.Price[4] == Sell && e.Price[5] == Buy;

                if (API.OnReceiveBalance && API.Classification != null && API.Classification.Equals(string.Empty) == false)
                    SendNewOrder(e.Price, API.Classification);

                if (API.OnReceiveBalance && accumulate)
                {
                    if (e.Price[4] == Sell && e.Price[5] == Buy)
                        OnDetermineTheTrend(AccumulateSell += e.Sell, AccumulateBuy += e.Buy, e.Price[4], e.Price[5]);

                    else if (Sell > e.Price[4] || Buy > e.Price[5])
                        OnDetermineTheTrend(1, 0, e.Price[4], e.Price[5]);

                    else if (Sell < e.Price[4] || Buy < e.Price[5])
                        OnDetermineTheTrend(0, 1, e.Price[4], e.Price[5]);
                }
                if (accumulate == false)
                {
                    AccumulateSell = 0;
                    AccumulateBuy = 0;
                    Sell = e.Price[4];
                    Buy = e.Price[5];
                }
            }
        }
        private void OnDetermineTheTrend(int trendSell, int trendBuy, double priceSell, double priceBuy)
        {
            if (trendSell > trendBuy)
            {
                if (API.SellOrder.Count > 0 && API.SellOrder.ContainsValue(priceSell) == false)
                {
                    var number = API.SellOrder.First(o => o.Value == API.SellOrder.Max(p => p.Value)).Key;
                    var price = API.SellOrder.Min(o => o.Value) - Const.ErrorRate;

                    if (GetTickRevenue(number) && price > priceBuy && API.SellOrder.Remove(number))
                        SendCorrectionOrder(price, number);
                }
                if (API.BuyOrder.ContainsValue(priceBuy))
                {
                    var number = API.BuyOrder.First(o => o.Value == priceBuy).Key;
                    var price = API.BuyOrder.Min(o => o.Value) - Const.ErrorRate;

                    if (GetTickRevenue(number) && API.BuyOrder.Remove(number))
                        SendCorrectionOrder(price, number);
                }
            }
            if (trendBuy > trendSell)
            {
                if (API.BuyOrder.Count > 0 && API.BuyOrder.ContainsValue(priceBuy) == false)
                {
                    var number = API.BuyOrder.First(o => o.Value == API.BuyOrder.Min(p => p.Value)).Key;
                    var price = API.BuyOrder.Max(o => o.Value) + Const.ErrorRate;

                    if (GetTickRevenue(number) && price < priceSell && API.BuyOrder.Remove(number))
                        SendCorrectionOrder(price, number);
                }
                if (API.SellOrder.ContainsValue(priceSell))
                {
                    var number = API.SellOrder.First(o => o.Value == priceSell).Key;
                    var price = API.SellOrder.Max(o => o.Value) + Const.ErrorRate;

                    if (GetTickRevenue(number) && API.SellOrder.Remove(number))
                        SendCorrectionOrder(price, number);
                }
            }
        }
        private bool GetTickRevenue(string number)
        {
            if (API.Quantity > 0 && API.SellOrder.TryGetValue(number, out double sell) && sell == GetExactPrice())
                return false;

            else if (API.Quantity < 0 && API.BuyOrder.TryGetValue(number, out double buy) && buy == GetExactPrice())
                return false;

            return API.Quantity != 0 && (API.Quantity > 0 ? API.SellOrder.Count > 0 : API.BuyOrder.Count > 0) ? false : true;
        }
        private double GetExactPrice()
        {
            int tail = int.Parse(API.AvgPurchase.Substring(5, 1));
            string definite = tail < 5 && tail > 0 ? string.Empty : API.AvgPurchase.Substring(5);

            if (int.TryParse(definite, out int rest))
            {
                definite = rest == 0 || rest == 5 ? API.AvgPurchase.Substring(0, 5) : string.Concat(API.AvgPurchase.Substring(0, 5), "5");

                return API.Quantity > 0 ? double.Parse(definite) + Const.ErrorRate : double.Parse(definite) - Const.ErrorRate;
            }
            else
                return API.Quantity > 0 ? double.Parse(API.AvgPurchase.Substring(0, 5)) + Const.ErrorRate : double.Parse(API.AvgPurchase.Substring(0, 5)) - Const.ErrorRate;
        }
        private void SendCorrectionOrder(double price, string number)
        {
            API.OnReceiveBalance = false;
            API.orders[1].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                OrgOrdNo = number,
                FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                OrdPrc = price.ToString("F2"),
                OrdQty = specify.Quantity
            });
        }
        private void SendNewOrder(double[] param, string classification)
        {
            var price = param[classification.Equals("2") ? 9 : 0];

            if (price == 0 || (classification.Equals("2") ? API.Quantity + API.BuyOrder.Count : API.SellOrder.Count - API.Quantity) > API.Max(specify.Assets / ((classification.Equals("2") ? param[5] : param[4]) * Const.TransactionMultiplier * Const.MarginRate200402)) || (classification.Equals("2") ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)))
                return;

            API.OnReceiveBalance = false;
            API.orders[0].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                BnsTpCode = classification,
                FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                OrdPrc = price.ToString("F2"),
                OrdQty = specify.Quantity
            });
        }
        private double Sell
        {
            get; set;
        }
        private double Buy
        {
            get; set;
        }
        private int AccumulateSell
        {
            get; set;
        }
        private int AccumulateBuy
        {
            get; set;
        }
    }
}