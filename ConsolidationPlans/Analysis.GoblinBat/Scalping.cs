using System;
using System.Collections.Generic;
using ShareInvest.BackTest;
using ShareInvest.Chart;
using ShareInvest.EventHandler;
using ShareInvest.SecondaryIndicators;
using ShareInvest.Secret;

namespace ShareInvest.Analysis
{
    public class Scalping : Conceal
    {
        public Scalping(int reaction, int type, int se, int le)
        {
            this.type = type;
            info = new Information(type);
            ema = new EMA(se, le);
            short_ema = new List<double>(32768);
            long_ema = new List<double>(32768);
            act = new Action(() => info.Log(reaction, se, le));
            Send += Analysis;

            foreach (string rd in new Daily(type))
            {
                arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(reaction, arr[0], double.Parse(arr[1])));
            }
            foreach (string rd in new Tick(type))
            {
                arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(reaction, arr[0], double.Parse(arr[1]), int.Parse(arr[2])));
            }
            act.BeginInvoke(act.EndInvoke, null);
        }
        public Scalping(int type, int se, int le)
        {
            this.type = type;
            ema = new EMA(se, le);
            short_ema = new List<double>(32768);
            long_ema = new List<double>(32768);
            Send += Analysis;

            foreach (string rd in new Daily(type))
            {
                arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(arr[0], double.Parse(arr[1])));
            }
            foreach (string rd in new Tick(type))
            {
                arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(arr[0], double.Parse(arr[1]), int.Parse(arr[2])));
            }
            Send -= Analysis;
            arr = SetSecret(type).Split('^');
            Secret = int.Parse(arr[0]);
            api = Futures.Get();
            api.Send += Analysis;
        }
        private void Analysis(object sender, Datum e)
        {
            int quantity, sc = short_ema.Count, lc = long_ema.Count;

            short_ema.Add(sc > 0 ? ema.Make(ema.ShortPeriod, sc, e.Price, short_ema[sc - 1]) : ema.Make(e.Price));
            long_ema.Add(lc > 0 ? ema.Make(ema.LongPeriod, lc, e.Price, long_ema[lc - 1]) : ema.Make(e.Price));

            if (api != null)
            {
                if (e.Volume > Secret || e.Volume < -Secret)
                {
                    quantity = Order(sc > 1 ? Trend() : 0);

                    if (Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity) && Math.Abs(api.Quantity + quantity) < (int)(basicAsset / (e.Price * (type > 0 ? ktm * kqm : tm * margin))))
                        api.OnReceiveOrder(dic[quantity]);

                    return;
                }
                if (api.Remaining == null)
                    api.RemainingDay();

                Time = int.Parse(e.Time);

                if (After == false && Time > 154450)
                {
                    After = true;

                    for (quantity = Math.Abs(api.Quantity); quantity > 0; quantity--)
                        api.OnReceiveOrder(dic[api.Quantity > 0 ? -1 : 1]);

                    return;
                }
                if (api.Quantity != 0 && api.Remaining.Equals("1") && Time > 151945)
                    api.OnReceiveOrder(dic[api.Quantity > 0 ? -1 : 1]);
            }
            else if (e.Reaction > 0)
            {
                if (e.Time.Length > 2 && e.Time.Substring(6, 4).Equals("1545") || Array.Exists(type > 0 ? info.KosdaqRemaining : info.Remaining, o => o.Equals(e.Time)))
                {
                    while (info.Quantity != 0)
                        info.OperateScalping(e.Price, info.Quantity > 0 ? -1 : 1);

                    info.Save(e.Time);
                }
                else if (e.Volume > e.Reaction || e.Volume < -e.Reaction)
                {
                    quantity = Order(sc > 1 ? Trend() : 0);

                    if (Math.Abs(info.Quantity + quantity) < (int)(basicAsset / (e.Price * (type > 0 ? ktm * kqm : tm * margin))))
                        info.OperateScalping(e.Price, quantity);
                }
            }
        }
        private int Order(double eg)
        {
            return eg > 0 ? 1 : eg < 0 ? -1 : 0;
        }
        private double Trend()
        {
            int sc = short_ema.Count, lc = long_ema.Count;

            return short_ema[sc - 1] - long_ema[lc - 1] - (short_ema[sc - 2] - long_ema[lc - 2]);
        }
        private int Time
        {
            get; set;
        }
        private bool After
        {
            get; set;
        } = false;
        private static int Secret
        {
            get; set;
        }
        private readonly Dictionary<int, string> dic = new Dictionary<int, string>()
        {
            {-1, "1"},
            {1, "2"},
        };
        private readonly Action act;
        private readonly Information info;
        private readonly Futures api;
        private readonly EMA ema;
        private readonly List<double> short_ema;
        private readonly List<double> long_ema;
        private readonly string[] arr;
        private readonly int type;
        private const int basicAsset = 35000000;
        public event EventHandler<Datum> Send;
    }
}