using System;
using System.Collections.Generic;
using ShareInvest.AutoMessageBox;
using ShareInvest.Const;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.FindbyOptionsCode;
using ShareInvest.Interface;
using ShareInvest.OpenAPI;
using ShareInvest.RetrieveInformation;
using ShareInvest.SecondaryIndicators;

namespace ShareInvest.Analysize
{
    public class Strategy
    {
        public Strategy(IStatistics st)
        {
            ema = new EMA();
            shortDay = new List<double>(512);
            longDay = new List<double>(512);
            shortTick = new List<double>(2097152);
            longTick = new List<double>(2097152);
            Send += Analysis;
            this.st = st;
            GetChart();
            Send -= Analysis;
            api = ConnectAPI.Get();
            api.SendDatum += Analysis;
            SendLiquidate += Balance.Get().OnReceiveLiquidate;
        }
        public void SetAccount(IAccount account)
        {
            this.account = account;
        }
        private void Analysis(object sender, Datum e)
        {
            int quantity = Order(Analysis(e.Price), Analysis(e.Time, e.Price));

            if (api != null && Math.Abs(api.Quantity + quantity) < (int)(account.BasicAssets / (e.Price * st.TransactionMultiplier * st.MarginRate)) && api.OnReceiveBalance && (e.Volume > st.Reaction || e.Volume < -st.Reaction) && Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity))
            {
                IStrategy strategy = new PurchaseInformation
                {
                    Code = api.Code[0].Substring(0, 8),
                    SlbyTP = dic[quantity],
                    OrdTp = Enum.GetName(typeof(IStrategy.OrderType), 3),
                    Price = string.Empty,
                    Qty = Math.Abs(quantity)
                };
                api.OnReceiveOrder(strategy);
                api.OnReceiveBalance = false;
                double temp = 0;
                string code = string.Empty;
                bool check = api.Quantity > 0 && quantity < 0 || api.Quantity < 0 && quantity > 0;

                if (check == false)
                    foreach (KeyValuePair<string, double> kv in api.OptionsCalling)
                    {
                        double approximation = e.Price * st.MarginRate * st.ErrorRate - kv.Value;

                        if (approximation > 0 && temp < approximation && (quantity > 0 ? kv.Key.Contains("301") : kv.Key.Contains("201")))
                        {
                            temp = approximation;
                            code = kv.Key;
                        }
                    }
                for (int i = 0; i < st.HedgeType; i++)
                {
                    if (check == false)
                    {
                        api.OnReceiveOrder(new PurchaseInformation
                        {
                            Code = i > 1 ? code = new FindbyOptions().Code(code) : code,
                            SlbyTP = "2",
                            OrdTp = Enum.GetName(typeof(IStrategy.OrderType), 3),
                            Price = string.Empty,
                            Qty = Math.Abs(quantity)
                        });
                        api.OnReceiveBalance = false;
                    }
                    else if (check)
                        SendLiquidate?.Invoke(this, new Liquidate(strategy));
                }
            }
        }
        private int Analysis(double price)
        {
            int sc = shortTick.Count, lc = longTick.Count;
            shortTick.Add(sc > 0 ? ema.Make(st.ShortTickPeriod, sc, price, shortTick[sc - 1]) : ema.Make(price));
            longTick.Add(lc > 0 ? ema.Make(st.LongTickPeriod, lc, price, longTick[lc - 1]) : ema.Make(price));

            return (sc < 2 || lc < 2) ? 0 : shortTick[sc] - longTick[lc] - (shortTick[sc - 1] - longTick[lc - 1]) > 0 ? 1 : -1;
        }
        private int Analysis(string time, double price)
        {
            int sc = shortDay.Count, lc = longDay.Count;

            if ((time.Length == 6 && !time.Equals(initiation) ? false : time.Length == 2 ? true : ConfirmDate(time.Substring(0, 6))) == false)
            {
                shortDay[sc - 1] = ema.Make(st.ShortDayPeriod, sc, price, sc > 1 ? shortDay[sc - 2] : 0);
                longDay[lc - 1] = ema.Make(st.LongDayPeriod, lc, price, lc > 1 ? longDay[lc - 2] : 0);

                return shortDay[sc - 1] - longDay[lc - 1] - (shortDay[sc - 2] - longDay[lc - 2]) > 0 ? 1 : -1;
            }
            shortDay.Add(sc > 0 ? ema.Make(st.ShortDayPeriod, sc, price, shortDay[sc - 1]) : ema.Make(price));
            longDay.Add(lc > 0 ? ema.Make(st.LongDayPeriod, lc, price, longDay[lc - 1]) : ema.Make(price));

            return sc > 1 && lc > 1 ? shortDay[sc] - longDay[lc] - (shortDay[sc - 1] - longDay[lc - 1]) > 0 ? 1 : -1 : 0;
        }
        private int Order(int tick, int day)
        {
            return tick > 0 && day > 0 ? 1 : tick < 0 && day < 0 ? -1 : 0;
        }
        private bool ConfirmDate(string date)
        {
            if (date.Equals(Register))
                return false;

            Register = date;

            return true;
        }
        private void GetChart()
        {
            try
            {
                foreach (string rd in new Fetch())
                {
                    string[] arr = rd.Split(',');

                    if (arr[1].Contains("-"))
                        arr[1] = arr[1].Substring(1);

                    Send?.Invoke(this, arr.Length > 2 ? new Datum(arr[0], double.Parse(arr[1]), int.Parse(arr[2])) : new Datum(arr[0], double.Parse(arr[1])));
                }
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
        }
        private string Register
        {
            get; set;
        }
        private readonly Dictionary<int, string> dic = new Dictionary<int, string>()
        {
            {-1, "1"},
            {1, "2"},
        };
        private const string initiation = "090000";
        private IAccount account;
        private readonly IStatistics st;
        private readonly EMA ema;
        private readonly ConnectAPI api;
        private readonly List<double> shortDay;
        private readonly List<double> longDay;
        private readonly List<double> shortTick;
        private readonly List<double> longTick;
        public event EventHandler<Datum> Send;
        public event EventHandler<Liquidate> SendLiquidate;
    }
}