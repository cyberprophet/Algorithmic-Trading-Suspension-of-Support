using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Basic;
using ShareInvest.Const;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.FindbyOptionsCode;
using ShareInvest.Interface;
using ShareInvest.Log.Message;
using ShareInvest.OpenAPI;
using ShareInvest.RetrieveInformation;
using ShareInvest.Secondary;
using ShareInvest.SecondaryIndicators;

namespace ShareInvest.Analysize
{
    public class Strategy
    {
        public Strategy(IStatistics st)
        {
            ema = new EMA();
            bands = st.Base > 1 && st.Sigma > 0 && st.Percent > 0 && st.Max > 0 ? true : false;
            days = st.ShortDayPeriod > 1 && st.LongDayPeriod > 2 ? true : false;
            count = st.Quantity + 1;
            Headway = st.Time;

            if (bands)
            {
                over = new BollingerBands(st.Sigma * 0.1, st.Base, st.Percent, st.Max);
                baseTick = new List<double>(2097152);
            }
            if (days)
            {
                shortDay = new List<double>(512);
                longDay = new List<double>(512);
            }
            shortTick = new List<double>(2097152);
            longTick = new List<double>(2097152);
            Send += Analysis;
            this.st = st;
            GetChart();
            Send -= Analysis;
            api = ConnectAPI.Get();
            api.SendDatum += Analysis;
            balance = Balance.Get();
            SendLiquidate += balance.OnReceiveLiquidate;
        }
        public bool SetAccount(IAccount account)
        {
            Account = account;
            new BasicMaterial(account, st);

            return false;
        }
        public void SetDeposit(IAccount account)
        {
            if (Account.BasicAssets > account.BasicAssets)
                new BasicMaterial(account, st);

            Account = account;
        }
        private void Analysis(object sender, Datum e)
        {
            int i, quantity = days ? Order(Analysis(e.Price), Analysis(e.Time, e.Price)) : Order(Analysis(e.Price));
            double futures = e.Price * st.TransactionMultiplier * st.MarginRate, max = bands ? over.GetJudgingOverHeating(Account == null ? 0 : Account.BasicAssets / count / (futures + futures * rate[st.HedgeType]), e.Price, baseTick[baseTick.Count - 1]) : (Account == null ? 0 : Account.BasicAssets / count / (futures + futures * rate[st.HedgeType]));

            if (api != null && Account != null)
            {
                balance.OnRealTimeCurrentPriceReflect(e.Price, api.Quantity, st);

                if (Math.Abs(api.Quantity + quantity) < max && Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity) && api.OnReceiveBalance && (e.Volume > st.Reaction || e.Volume < -st.Reaction) && Interval())
                {
                    if (api.Remaining && Math.Abs(api.Quantity) > 0)
                    {
                        api.OnReceiveBalance = api.RollOver(quantity);

                        return;
                    }
                    IStrategy strategy = new PurchaseInformation
                    {
                        Code = api.Code[0].Substring(0, 8),
                        SlbyTP = dic[quantity],
                        OrdTp = ((int)IStrategy.OrderType.시장가).ToString(),
                        Price = string.Empty,
                        Qty = Math.Abs(quantity)
                    };
                    for (i = 0; i < count; i++)
                        api.OnReceiveOrder(strategy);

                    api.OnReceiveBalance = false;
                    double temp = 0;
                    string code = string.Empty;
                    new Task(() => new LogMessage().Record("Order", string.Concat(DateTime.Now.ToLongTimeString(), "*", e.Time, "*", e.Price))).Start();

                    if (api.Quantity > 0 && quantity < 0 || api.Quantity < 0 && quantity > 0)
                    {
                        for (i = 0; i < count; i++)
                            SendLiquidate?.Invoke(this, new Liquidate(strategy));

                        return;
                    }
                    if (st.HedgeType > 0)
                    {
                        foreach (KeyValuePair<string, double> kv in api.OptionsCalling)
                            if (e.Price * st.MarginRate * rate[st.HedgeType] - kv.Value > 0 && temp < kv.Value && (quantity > 0 ? kv.Key.Contains("301") : kv.Key.Contains("201")))
                            {
                                temp = kv.Value;
                                code = new FindbyOptions().Code(kv.Key);
                            }
                        for (i = 0; i < count; i++)
                            api.OnReceiveOrder(new PurchaseInformation
                            {
                                Code = code,
                                SlbyTP = "2",
                                OrdTp = ((int)IStrategy.OrderType.시장가).ToString(),
                                Price = string.Empty,
                                Qty = Math.Abs(quantity)
                            });
                        new Task(() => new LogMessage().Record("Options", string.Concat(DateTime.Now.ToLongTimeString(), "*", code, "*", temp, "*Buy"))).Start();
                    }
                    return;
                }
                if (Math.Abs(api.Quantity) > max && api.OnReceiveBalance)
                {
                    IStrategy strategy = new PurchaseInformation
                    {
                        Code = api.Code[0].Substring(0, 8),
                        SlbyTP = api.Quantity > 0 ? "1" : "2",
                        OrdTp = ((int)IStrategy.OrderType.시장가).ToString(),
                        Price = string.Empty,
                        Qty = 1
                    };
                    api.OnReceiveOrder(strategy);
                    api.OnReceiveBalance = false;
                    SendLiquidate?.Invoke(this, new Liquidate(strategy));
                    new Task(() => new LogMessage().Record("Liquidate", string.Concat(DateTime.Now.ToLongTimeString(), "*", e.Time, "*", e.Price))).Start();

                    return;
                }
                if (api.Remaining && api.OnReceiveBalance && Math.Abs(api.Quantity) > 0 && int.Parse(e.Time) > 151949)
                    api.OnReceiveBalance = api.RollOver(api.Quantity);
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
        private bool Interval()
        {
            if (Headway-- > 0)
                return false;

            Headway = st.Time;

            return true;
        }
        private int Order(int tick, int day)
        {
            return tick > 0 && day > 0 ? 1 : tick < 0 && day < 0 ? -1 : 0;
        }
        private int Order(int tick)
        {
            return tick > 0 ? 1 : tick < 0 ? -1 : 0;
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
                new LogMessage().Record("Error", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
        private int Headway
        {
            get; set;
        }
        private string Register
        {
            get; set;
        }
        private IAccount Account
        {
            get; set;
        }
        private readonly Dictionary<int, string> dic = new Dictionary<int, string>()
        {
            {-1, "1"},
            {1, "2"},
        };
        private readonly Dictionary<int, double> rate = new Dictionary<int, double>()
        {
            {0, 0 },
            {1, 0.05 },
            {2, 0.1 },
            {3, 0.135 },
            {4, 0.17 },
            {5, 0.205 }
        };
        private const string initiation = "090000";
        private readonly int count;
        private readonly bool bands;
        private readonly bool days;
        private readonly IStatistics st;
        private readonly EMA ema;
        private readonly Balance balance;
        private readonly ConnectAPI api;
        private readonly BollingerBands over;
        private readonly List<double> baseTick;
        private readonly List<double> shortDay;
        private readonly List<double> longDay;
        private readonly List<double> shortTick;
        private readonly List<double> longTick;
        public event EventHandler<Datum> Send;
        public event EventHandler<Liquidate> SendLiquidate;
    }
}