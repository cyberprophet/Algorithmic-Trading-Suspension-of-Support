using System;
using System.Collections.Generic;
using System.Linq;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class Trading : Trend
    {
        public Trading(Catalog.Specify specify) : base(specify)
        {
            foreach (Catalog.XingAPI.Quotes quotes in Retrieve.Quotes)
                if (quotes.Price != null)
                    Analysize(new Chart
                    {
                        Date = long.Parse(quotes.Time),
                        Price = double.Parse(quotes.Price),
                        Volume = int.Parse(quotes.Volume)
                    });
            if (specify.Time == 1440)
                OnTime = true;

            else
                Check = string.Empty;

            ((IEvents<Datum>)API.reals[1]).Send += Analysize;
        }
        protected internal bool GetTickRevenue(string number)
        {
            if (API.Quantity > 0 && API.SellOrder.TryGetValue(number, out double sell) && sell == GetExactPrice())
                return false;

            else if (API.Quantity < 0 && API.BuyOrder.TryGetValue(number, out double buy) && buy == GetExactPrice())
                return false;

            return !specify.Strategy.Equals(number) || API.Quantity == 0 || !(API.Quantity > 0 ? API.SellOrder.Count > 0 : API.BuyOrder.Count > 0);
        }
        protected internal void SendNewOrder(string classification, double price)
        {
            API.OnReceiveBalance = false;
            API.orders[0].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                BnsTpCode = classification,
                FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                OrdPrc = price.ToString("F2"),
                OrdQty = "1"
            });
        }
        protected internal void SendClearingOrder(double trend)
        {
            foreach (KeyValuePair<string, double> kv in trend > 0 ? API.SellOrder.OrderBy(o => o.Value) : API.BuyOrder.OrderByDescending(o => o.Value))
                if (trend > 0 ? API.SellOrder.Remove(kv.Key) : API.BuyOrder.Remove(kv.Key))
                {
                    API.OnReceiveBalance = false;
                    API.orders[2].QueryExcute(new Order
                    {
                        FnoIsuNo = ConnectAPI.Code,
                        OrgOrdNo = kv.Key
                    });
                }
        }
        void SetTrendFollowing(double max, double classification)
        {
            API.Difference = max - Math.Abs(API.Quantity) - (classification > 0 ? API.BuyOrder.Count : API.SellOrder.Count);

            if (API.Difference > 1)
                API.Classification = classification > 0 ? "2" : "1";

            else
                API.Classification = string.Empty;
        }
        void SetWindingUp(double classification)
        {
            API.WindingUp = Math.Abs(API.Quantity) - (classification > 0 ? API.BuyOrder.Count : API.SellOrder.Count);

            if (API.WindingUp > 0 && classification > 0 && API.Quantity < 0 && API.Classification.Equals("2") == false)
                API.WindingClass = "2";

            else if (API.WindingUp > 0 && classification < 0 && API.Quantity > 0 && API.Classification.Equals("1") == false)
                API.WindingClass = "1";

            else
                API.WindingClass = string.Empty;
        }
        double GetExactPrice()
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
        void Analysize(object sender, Datum e)
        {
            if (GetCheckOnTimeByAPI(e.Time))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(EMA.Make(specify.Short, Short.Count, e.Price, Short.Peek()));
            Long.Push(EMA.Make(specify.Long, Long.Count, e.Price, Long.Peek()));
            double popShort = Short.Pop(), popLong = Long.Pop();
            var trend = popShort - popLong - (Short.Peek() - Long.Peek());
            Short.Push(popShort);
            Long.Push(popLong);

            API.Trend[specify.Strategy] = string.Concat(trend.ToString("F2"), " (", specify.Time == 1440 ? "Day" : Check, ")");

            switch (specify.Strategy)
            {
                case "TF":
                    SetTrendFollowing(specify.Assets / (specify.Code.Length == 8 ? e.Price * Const.TransactionMultiplier * Const.MarginRate200402 : e.Price), trend);
                    break;

                case "WU":
                    SetWindingUp(trend);

                    if (API.Quantity != 0 && API.OnReceiveBalance && GetTickRevenue(specify.Strategy))
                    {
                        var price = GetExactPrice();

                        if (API.Quantity > 0 ? price > e.Price : price < e.Price)
                            SendNewOrder(API.Quantity > 0 ? "1" : "2", price);
                    }
                    break;
            }
            if (GetTrend(trend.ToString()))
                SendClearingOrder(trend);

            if (specify.Reaction > 0)
            {
                API.Volume += e.Volume;

                if (API.OnReceiveBalance && (trend > 0 ? API.BuyOrder.Count == 0 : API.SellOrder.Count == 0) && GetJudgeTheReaction(trend, e.Price) && GetJudgeTheReaction(API.Volume, trend))
                {
                    var price = e.Price + (trend > 0 ? -Const.ErrorRate : Const.ErrorRate);
                    SendNewOrder(trend > 0 ? "2" : "1", price);
                    API.Volume = 0;
                }
            }
        }
        bool GetCheckOnTimeByAPI(string time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
            {
                if (time.Substring(2, 2).Equals(Check) || Check.Equals(string.Empty))
                {
                    Check = (new TimeSpan(int.Parse(time.Substring(0, 2)), int.Parse(time.Substring(2, 2)), int.Parse(time.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                    return false;
                }
                return true;
            }
            else if (OnTime && specify.Time == 1440 && time.Equals(onTime))
            {
                API.OnReceiveBalance = true;

                return OnTime = false;
            }
            return true;
        }
        bool GetTrend(string trend)
        {
            int check = trend.Contains("-") ? -1 : 1;

            if (check == Trend)
                return false;

            Trend = check;

            return true;
        }
        bool GetJudgeTheReaction(double trend, double price)
        {
            var max = specify.Assets / (price * Const.TransactionMultiplier * Const.MarginRate200402);

            if (trend > 0)
                return max - API.Quantity - API.BuyOrder.Count > 1;

            else if (trend < 0)
                return max + API.Quantity - API.SellOrder.Count > 1;

            return false;
        }
        bool GetJudgeTheReaction(int volume, double trend)
        {
            if (trend < 0 && specify.Reaction < volume || trend > 0 && -specify.Reaction > volume)
                return true;

            return false;
        }
        int Trend
        {
            get; set;
        }
        EMA EMA
        {
            get;
        }
        const string onTime = "090000";
    }
}