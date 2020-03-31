using System;
using System.Linq;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class Quotes : Trading
    {
        public Quotes(Catalog.Specify specify) : base(specify)
        {
            strategy = specify.Strategy.Equals("TF");
            API.OnReceiveBalance = false;
            API.WindingClass = string.Empty;
            ((IEvents<EventHandler.XingAPI.Quotes>)API.reals[0]).Send += OnReceiveQuotes;
        }
        private void SendRollOverOrder(int over)
        {
            SendClearingOrder(over);
            SendClearingOrder(-over);

            if (API.Quantity != 0)
                API.orders[0].QueryExcute(new Order
                {
                    FnoIsuNo = ConnectAPI.Code,
                    BnsTpCode = API.Quantity > 0 ? "1" : "2",
                    FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.시장가).ToString("D2"),
                    OrdPrc = string.Empty,
                    OrdQty = ((int)(Math.Abs(API.Quantity) / (over < 3 && over > -3 ? 1 : 1.5))).ToString()
                });
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
                OrdQty = "1"
            });
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
        private void SendNewOrder(double[] param, string classification)
        {
            var price = param[classification.Equals("2") ? 9 : 0];

            if ((classification.Equals("2") ? API.Quantity + API.BuyOrder.Count : API.SellOrder.Count - API.Quantity) > specify.Assets / ((classification.Equals("2") ? param[5] : param[4]) * Const.TransactionMultiplier * Const.MarginRate200402) || (classification.Equals("2") ? API.BuyOrder.ContainsValue(price) : API.SellOrder.ContainsValue(price)))
                return;

            SendNewOrder(classification, price);
        }
        private void OnReceiveQuotes(object sender, EventHandler.XingAPI.Quotes e)
        {
            if (strategy && int.TryParse(e.Time, out int time) && (time > 153459 && time < 180000) == false)
            {
                bool accumulate = e.Price[4] == Sell && e.Price[5] == Buy;

                if (API.OnReceiveBalance && API.WindingClass.Equals(string.Empty) == false)
                    SendNewOrder(e.Price, API.WindingClass);

                if (API.OnReceiveBalance && API.Classification.Equals(string.Empty) == false)
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
            else if (strategy && API.Trend.Count > 0 && int.TryParse(e.Time, out int roll) && roll > 153459 && roll < 154500)
            {
                int over = 0;

                foreach (var kv in API.Trend)
                    over += kv.Value.Contains("-") ? -1 : 1;

                API.Trend.Clear();

                if (over != 0)
                    SendRollOverOrder(over);
            }
        }
        private int AccumulateSell
        {
            get; set;
        }
        private int AccumulateBuy
        {
            get; set;
        }
        private double Sell
        {
            get; set;
        }
        private double Buy
        {
            get; set;
        }
        private readonly bool strategy;
    }
}