using System;
using System.Collections.Generic;
using ShareInvest.Catalog;
using ShareInvest.EventHandler.OpenAPI;
using ShareInvest.OpenAPI;

namespace ShareInvest.Strategy
{
    public class Trading
    {
        public Trading(ConnectAPI api, Specify specify, Quotes quotes, Queue<Chart> charts)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);
            SendDatum += Analysize;

            foreach (Chart chart in charts)
                SendDatum?.Invoke(this, new Datum(chart));

            SendDatum -= Analysize;
            this.api = api;
            this.quotes = quotes;
            Check = string.Empty;
            api.SendDatum += Analysize;
        }
        public Trading(ConnectAPI api, Specify specify, Queue<Chart> charts)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);
            SendDatum += Analysize;

            foreach (Chart chart in charts)
                SendDatum?.Invoke(this, new Datum(chart));

            SendDatum -= Analysize;
            this.api = api;
            OnTime = true;
            api.SendDatum += Analysize;
        }
        private void Analysize(object sender, Datum e)
        {
            if (api == null)
            {
                if (GetCheckOnTime(e.Chart.Date))
                {
                    Short.Pop();
                    Long.Pop();
                }
                Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, e.Chart.Price, Short.Peek()) : EMA.Make(e.Chart.Price));
                Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, e.Chart.Price, Long.Peek()) : EMA.Make(e.Chart.Price));

                return;
            }
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
            api.Trend[specify.Strategy] = string.Concat(trend.ToString("F2"), " (", specify.Time == 1440 ? "Day" : Check, ")");

            if (quotes != null)
            {
                switch (specify.Strategy)
                {
                    case "TF":
                        quotes.SetTrendFollowing(specify.Assets / (specify.Code.Length == 8 ? e.Price * Const.TransactionMultiplier * Const.MarginRate : e.Price), trend);
                        break;

                    case "WU":
                        quotes.SetWindingUp(trend);

                        if (api.Quantity != 0 && api.OnReceiveBalance && quotes.GetTickRevenue(specify.Strategy))
                        {
                            var price = quotes.GetExactPrice();

                            if (api.Quantity > 0 ? price > e.Price : price < e.Price)
                            {
                                api.OnReceiveBalance = false;
                                api.OnReceiveOrder(new PurchaseInformation
                                {
                                    RQName = string.Concat(price, ';'),
                                    ScreenNo = string.Concat(api.Quantity > 0 ? "1" : "2", new Random().Next(1, 76).ToString("D3")),
                                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                                    Code = api.Code,
                                    OrdKind = 1,
                                    SlbyTP = api.Quantity > 0 ? "1" : "2",
                                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                                    Qty = 1,
                                    Price = price.ToString("F2"),
                                    OrgOrdNo = string.Empty
                                });
                            }
                        }
                        break;
                }
                if (GetTrend(trend.ToString()))
                    quotes.SendClearingOrder(trend);
            }
            else if (specify.Reaction > 0)
            {
                api.Volume += e.Volume;

                if (api.OnReceiveBalance && (trend > 0 ? api.BuyOrder.Count == 0 : api.SellOrder.Count == 0) && GetJudgeTheReaction(trend, e.Price) && GetJudgeTheReaction(api.Volume, trend))
                {
                    api.OnReceiveBalance = false;
                    var price = e.Price + (trend > 0 ? -Const.ErrorRate : Const.ErrorRate);
                    api.OnReceiveOrder(new PurchaseInformation
                    {
                        RQName = string.Concat(price, ';'),
                        ScreenNo = string.Concat(trend > 0 ? "2" : "1", new Random().Next(1, 76).ToString("D3")),
                        AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                        Code = api.Code,
                        OrdKind = 1,
                        SlbyTP = trend > 0 ? "2" : "1",
                        OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                        Qty = 1,
                        Price = price.ToString("F2"),
                        OrgOrdNo = string.Empty
                    });
                    api.Volume = 0;
                }
            }
        }
        private bool GetJudgeTheReaction(double trend, double price)
        {
            var max = specify.Assets / (price * Const.TransactionMultiplier * Const.MarginRate);

            if (trend > 0)
                return max - api.Quantity - api.BuyOrder.Count > 1;

            else if (trend < 0)
                return max + api.Quantity - api.SellOrder.Count > 1;

            return false;
        }
        private bool GetJudgeTheReaction(int volume, double trend)
        {
            if (trend < 0 && specify.Reaction < volume || trend > 0 && -specify.Reaction > volume)
                return true;

            return false;
        }
        private bool GetTrend(string trend)
        {
            int check = trend.Contains("-") ? -1 : 1;

            if (check == Trend)
                return false;

            Trend = check;

            return true;
        }
        private bool GetCheckOnTimeByAPI(string time)
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
            else if (OnTime && specify.Time == 1440 && time.Equals("090000"))
            {
                api.OnReceiveBalance = true;

                return OnTime = false;
            }
            return true;
        }
        private bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals("090000000") == false;

            return false;
        }
        private bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if (onTime.Substring(2, 2).Equals(Check) || Check == null || onTime.Equals("154500") || onTime.Equals("090000"))
            {
                Check = (new TimeSpan(int.Parse(onTime.Substring(0, 2)), int.Parse(onTime.Substring(2, 2)), int.Parse(onTime.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                return false;
            }
            return true;
        }
        private int Trend
        {
            get; set;
        }
        private bool OnTime
        {
            get; set;
        }
        private string Check
        {
            get; set;
        }
        private EMA EMA
        {
            get;
        }
        private Stack<double> Short
        {
            get;
        }
        private Stack<double> Long
        {
            get;
        }
        private readonly Specify specify;
        private readonly Quotes quotes;
        private readonly ConnectAPI api;
        public event EventHandler<Datum> SendDatum;
    }
}