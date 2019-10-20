using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ShareInvest.BackTest;
using ShareInvest.Chart;
using ShareInvest.Communicate;
using ShareInvest.Const;
using ShareInvest.EventHandler;
using ShareInvest.Publish;
using ShareInvest.SecondaryIndicators;
using ShareInvest.SelectableMessageBox;

namespace ShareInvest.Analysize
{
    public class Strategy
    {
        public Strategy(IStrategy st)
        {
            ema = new EMA();
            shortEMA = new List<double>(32768);
            longEMA = new List<double>(32768);
            shortDay = new List<double>(512);
            longDay = new List<double>(512);
            Send += Analysis;
            this.st = st;

            if (st.Division)
                info = new Information(st);

            GetChart();

            if (st.Division)
            {
                info.Log();

                return;
            }
            Send -= Analysis;
            api = PublicFutures.Get();
            api.Retention = Retention;
            result = Choose.Show("Decide How to Order. . .", "Notice", "MarketPrice", "SpecifyPrice", "DotHighPrice");
            om = order[result];
            api.Send += Analysis;
        }
        private int Analysis(string time, double price)
        {
            bool check = time.Length == 6 && !time.Equals("090000") ? false : time.Length == 2 ? true : ConfirmDate(time.Substring(0, 6));
            int sc = shortDay.Count, lc = longDay.Count;

            if (check == false)
            {
                shortDay[sc - 1] = ema.Make(st.ShortDayPeriod, sc, price, sc > 1 ? shortDay[sc - 2] : 0);
                longDay[lc - 1] = ema.Make(st.LongDayPeriod, lc, price, lc > 1 ? longDay[lc - 2] : 0);

                return shortDay[sc - 1] - longDay[lc - 1] - (shortDay[sc - 2] - longDay[lc - 2]) > 0 ? 1 : -1;
            }
            shortDay.Add(sc > 0 ? ema.Make(st.ShortDayPeriod, sc, price, shortDay[sc - 1]) : ema.Make(price));
            longDay.Add(lc > 0 ? ema.Make(st.LongDayPeriod, lc, price, longDay[lc - 1]) : ema.Make(price));
            sc = shortDay.Count;
            lc = longDay.Count;

            return sc > 1 && lc > 1 ? shortDay[sc - 1] - longDay[lc - 1] - (shortDay[sc - 2] - longDay[lc - 2]) > 0 ? 1 : -1 : 0;
        }
        private int Analysis(bool check, double price)
        {
            int sc = shortEMA.Count, lc = longEMA.Count;

            if (check == false)
            {
                shortEMA[sc - 1] = ema.Make(st.ShortMinPeriod, sc, price, sc > 1 ? shortEMA[sc - 2] : 0);
                longEMA[lc - 1] = ema.Make(st.LongMinPeriod, lc, price, lc > 1 ? longEMA[lc - 2] : 0);

                return shortEMA[sc - 1] - longEMA[lc - 1] - (shortEMA[sc - 2] - longEMA[lc - 2]) > 0 ? 1 : -1;
            }
            shortEMA.Add(sc > 0 ? ema.Make(st.ShortMinPeriod, sc, price, shortEMA[sc - 1]) : ema.Make(price));
            longEMA.Add(lc > 0 ? ema.Make(st.LongMinPeriod, lc, price, longEMA[lc - 1]) : ema.Make(price));
            sc = shortEMA.Count;
            lc = longEMA.Count;

            return sc > 1 && lc > 1 ? shortEMA[sc - 1] - longEMA[lc - 1] - (shortEMA[sc - 2] - longEMA[lc - 2]) > 0 ? 1 : -1 : 0;
        }
        private void Analysis(object sender, Datum e)
        {
            int quantity = Order(Analysis(e.Check, e.Price), Analysis(e.Time, e.Price));

            if (info != null && (e.Time.Length > 2 && e.Time.Substring(6, 4).Equals("1545") || Array.Exists(st.Type > 0 ? info.Kosdaq : info.Kospi, o => o.Equals(e.Time))))
            {
                while (info.Quantity != 0)
                    info.Operate(e.Price, info.Quantity > 0 ? -1 : 1);

                info.Save(e.Time);

                return;
            }
            if (api != null && api.Quantity != 0)
            {
                if (api.Remaining == null)
                    api.RemainingDay();

                Time = int.Parse(e.Time);

                if (api.Quantity != 0 && api.Remaining.Equals("1") && Time > 151945)
                {
                    api.OnReceiveOrder(new MarketOrder
                    {
                        SlbyTP = dic[api.Quantity > 0 ? -1 : 1]
                    });
                    return;
                }
                else if (After == false && Time > 154450)
                {
                    After = true;

                    for (quantity = Math.Abs(api.Quantity); quantity > 0; quantity--)
                        api.OnReceiveOrder(new MarketOrder
                        {
                            SlbyTP = dic[api.Quantity > 0 ? -1 : 1]
                        });
                    return;
                }
            }
            if (Math.Abs(e.Volume) > e.Reaction && Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity))
            {
                int max = (int)(st.BasicAssets / (e.Price * st.TransactionMultiplier * st.MarginRate));

                if (api != null && Math.Abs(api.Quantity + quantity) < max)
                {
                    if (!result.Equals(DialogResult.Yes))
                        om.Price = (result.Equals(DialogResult.No) ? e.Price : quantity > 0 ? e.Price + st.ErrorRate : e.Price - st.ErrorRate).ToString();

                    om.SlbyTP = dic[quantity];
                    api.OnReceiveOrder(om);
                }
                else if (info != null && Math.Abs(info.Quantity + quantity) < max)
                    info.Operate(e.Price, quantity);
            }
        }
        private int Order(int min, int day)
        {
            return min > 0 && day > 0 ? 1 : min < 0 && day < 0 ? -1 : 0;
        }
        private void GetChart()
        {
            foreach (string val in chart)
                foreach (string rd in new Fetch(val))
                {
                    string[] arr = rd.Split(',');

                    if (arr[1].Contains("-"))
                        arr[1] = arr[1].Substring(1);

                    Retention = arr[0];
                    Send?.Invoke(this, arr.Length > 2 ? new Datum(st.Reaction, arr[0], double.Parse(arr[1]), int.Parse(arr[2])) : new Datum(st.Reaction, arr[0], double.Parse(arr[1])));
                }
        }
        private bool ConfirmDate(string date)
        {
            if (date.Equals(Register))
                return false;

            Register = date;

            return true;
        }
        private bool After
        {
            get; set;
        } = false;
        private int Time
        {
            get; set;
        }
        private string Register
        {
            get; set;
        }
        private string Retention
        {
            get; set;
        }
        private readonly string[] chart =
        {
            "Day",
            "Tick"
        };
        private readonly Dictionary<int, string> dic = new Dictionary<int, string>()
        {
            {-1, "1"},
            {1, "2"},
        };
        private readonly Dictionary<DialogResult, IOrderMethod> order = new Dictionary<DialogResult, IOrderMethod>()
        {
            {DialogResult.Yes, new MarketOrder()},
            {DialogResult.No, new MostFavorableOrder()},
            {DialogResult.Cancel,new MostFavorableOrder()}
        };
        private readonly IStrategy st;
        private readonly IOrderMethod om;
        private readonly DialogResult result;
        private readonly EMA ema;
        private readonly Information info;
        private readonly PublicFutures api;
        private readonly List<double> shortEMA;
        private readonly List<double> longEMA;
        private readonly List<double> shortDay;
        private readonly List<double> longDay;
        public event EventHandler<Datum> Send;
    }
}