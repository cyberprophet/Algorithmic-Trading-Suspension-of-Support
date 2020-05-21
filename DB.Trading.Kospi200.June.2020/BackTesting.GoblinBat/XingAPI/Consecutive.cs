using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class Consecutive
    {
        public Consecutive(Catalog.XingAPI.Specify specify)
        {
            this.specify = specify;
            this.judge = specify.Strategy.Length == 2 && int.TryParse(specify.Strategy, out int judge) ? judge : int.MaxValue;
            Short = new Stack<double>(256);
            Long = new Stack<double>(256);

            foreach (var kv in Retrieve.Charts.OrderBy(o => o.Key))
                foreach (var chart in kv.Value.OrderBy(o => o.Date))
                    Analysize(chart);

            if (specify.Time == 1440)
            {
                RollOver = specify.RollOver == false || Array.Exists(Information.RemainingDay, o => o.Equals(DateTime.Now.ToString(remaining)));
                ran = new Random();
                OnTime = true;
                API.OnReceiveBalance = false;
                ((IEvents<EventHandler.XingAPI.Quotes>)API.reals[0]).Send += OnReceiveQuotes;
            }
            else
                Check = string.Empty;

            ((IEvents<Datum>)API.reals[1]).Send += Analysize;
        }
        void Analysize(Chart chart)
        {
            if (GetCheckOnTime(chart.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, chart.Price, Short.Peek()) : EMA.Make(chart.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, chart.Price, Long.Peek()) : EMA.Make(chart.Price));
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
            API.Max(popShort - popLong - (Short.Peek() - Long.Peek()), specify.Time, Check);
            Short.Push(popShort);
            Long.Push(popLong);

            if (specify.Time == 1440 && string.IsNullOrEmpty(API.Classification) == false && API.OnReceiveBalance)
            {
                var judge = API.Judge.OrderBy(o => o.Key);
                var trend = judge.First().Value;

                if ((API.Classification.Equals(buy) ? e.Volume + trend > e.Volume : e.Volume + trend < e.Volume) && GetJudgeTheTrading(e.Volume))
                {
                    if (API.Judge.Count > 2)
                    {
                        var num = 0;

                        foreach (var kv in judge)
                        {
                            if (kv.Key == judge.First().Key)
                                continue;

                            if (kv.Key == 1440)
                                break;

                            if (API.Classification.Equals(buy) && kv.Value < 0 || API.Classification.Equals(sell) && kv.Value > 0)
                                num++;
                        }
                        if (num == 8 && API.OnReceiveBalance && SendNewOrder(e.Price, API.Classification))
                        {
                            API.Volume = e.Volume;

                            return;
                        }
                    }
                    if (API.Judge.Count == 2 && API.OnReceiveBalance && SendNewOrder(e.Price, API.Classification))
                    {
                        API.Volume = e.Volume;

                        return;
                    }
                }
                if (API.Judge.Count > 2 && this.judge % 2 == 0)
                {
                    var num = 0;

                    if (API.Quantity > 0 && e.Volume < -this.judge && e.Volume + trend < e.Volume)
                    {
                        foreach (var kv in judge)
                        {
                            if (kv.Key == judge.First().Key)
                                continue;

                            if (kv.Key == 1440)
                                break;

                            if (API.Classification.Equals(buy) && kv.Value > 0)
                                num++;
                        }
                        if (num == 8 && API.OnReceiveBalance && SendNewOrder(e.Price, sell))
                        {
                            API.Volume = e.Volume;

                            return;
                        }
                    }
                    else if (API.Quantity < 0 && e.Volume > this.judge && e.Volume + trend > e.Volume)
                    {
                        foreach (var kv in judge)
                        {
                            if (kv.Key == judge.First().Key)
                                continue;

                            if (kv.Key == 1440)
                                break;

                            if (API.Classification.Equals(sell) && kv.Value < 0)
                                num++;
                        }
                        if (num == 8 && API.OnReceiveBalance && SendNewOrder(e.Price, buy))
                        {
                            API.Volume = e.Volume;

                            return;
                        }
                    }
                }
            }
        }
        void OnReceiveQuotes(object sender, EventHandler.XingAPI.Quotes e)
        {
            if (int.TryParse(e.Time, out int time) && (time < 090007 && time > 045959) == false && (time > 153459 && time < 180000) == false && string.IsNullOrEmpty(API.Classification) == false)
            {
                var check = API.Classification.Equals(buy);
                double[] bp = new double[] { e.Price[e.Price.Length - 3], e.Price[e.Price.Length - 4], e.Price[e.Price.Length - 5] }, sp = new double[] { e.Price[2], e.Price[3], e.Price[4] };
                API.MaxAmount = specify.Assets / ((check ? bp[2] : -sp[4]) * Const.TransactionMultiplier * specify.MarginRate);

                foreach (var kv in check ? API.BuyOrder : API.SellOrder)
                    if (Array.Exists(check ? bp : sp, o => o == kv.Value) == false && API.OnReceiveBalance && (check ? API.BuyOrder.ContainsKey(kv.Key) : API.SellOrder.ContainsKey(kv.Key)) && SendClearingOrder(kv.Key))
                        return;
            }
            else if (time > 153559 && time < 154459 && RollOver)
            {
                RollOver = false;
                SendLiquidationOrder();
            }
        }
        void SendLiquidationOrder()
        {
            foreach (var order in new Dictionary<string, double>[] { API.SellOrder, API.BuyOrder })
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
        bool SendNewOrder(double price, string classification)
        {
            API.OnReceiveBalance = false;
            API.orders[0].QueryExcute(new Order
            {
                FnoIsuNo = ConnectAPI.Code,
                BnsTpCode = classification,
                FnoOrdprcPtnCode = ((int)FnoOrdprcPtnCode.지정가).ToString("D2"),
                OrdPrc = price.ToString("F2"),
                OrdQty = sell
            });
            return true;
        }
        bool SendClearingOrder(string number)
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
        bool GetJudgeTheTrading(int quantity)
        {
            if (API.MaxAmount > 0 && API.MaxAmount - API.Quantity > 1)
                return judge < quantity;

            if (API.MaxAmount < 0 && API.Quantity - API.MaxAmount > 1)
                return -judge > quantity;

            return false;
        }
        bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals(onTime) == false;

            return false;
        }
        bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if ((onTime.Substring(0, 4).Equals(Check) || string.IsNullOrEmpty(Check) || onTime.Equals(end) || time.Substring(6).Equals(Consecutive.onTime) || time.Substring(6).Equals(nTime)) && DateTime.TryParseExact(onTime, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
            {
                Check = (date + TimeSpan.FromMinutes(specify.Time)).ToString(hm);

                return false;
            }
            return true;
        }
        bool GetCheckOnTimeByAPI(string time)
        {
            if (specify.Time > 0 && specify.Time < 1440 && (time.Substring(0, 4).Equals(Check) || string.IsNullOrEmpty(Check)) && DateTime.TryParseExact(time, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
            {
                Check = (date + TimeSpan.FromMinutes(specify.Time)).ToString(hm);

                return false;
            }
            else if (OnTime && specify.Time == 1440)
            {
                API.OnReceiveBalance = true;
                OnTime = false;

                if (time.Equals(start))
                    return false;
            }
            return true;
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
        Stack<double> Short
        {
            get;
        }
        Stack<double> Long
        {
            get;
        }
        EMA EMA
        {
            get;
        }
        string Check
        {
            get; set;
        }
        bool RollOver
        {
            get; set;
        }
        bool OnTime
        {
            get; set;
        }
        ConnectAPI API => ConnectAPI.GetInstance();
        readonly Catalog.XingAPI.Specify specify;
        readonly int judge;
        readonly Random ran;
        const string buy = "2";
        const string sell = "1";
        const string avg = "000.00";
        const string nTime = "180000000";
        const string onTime = "090000000";
        const string remaining = "yyMMdd";
        const string format = "HHmmss";
        const string hm = "HHmm";
        const string start = "090000";
        const string end = "154500";
    }
}