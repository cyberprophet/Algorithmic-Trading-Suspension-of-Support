using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpStatisticalAnalysis : CallUpGoblinBat
    {
        protected Dictionary<DateTime, string> GetInformation(long number)
        {
            try
            {
                var date = DateTime.Now.ToString(recent);

                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (DateTime.Now.Hour < 16)
                            date = DateTime.Now.AddDays(-3).ToString(recent);

                        break;

                    case DayOfWeek.Sunday:
                        date = DateTime.Now.AddDays(-2).ToString(recent);
                        break;

                    case DayOfWeek.Saturday:
                        date = DateTime.Now.AddDays(-1).ToString(recent);
                        break;

                    default:
                        if (DateTime.Now.Hour < 16 || new Secret().GetHoliday(DateTime.Now))
                            date = DateTime.Now.AddDays(-1).ToString(recent);

                        break;
                }
                using (var db = new GoblinBatDbContext(key))
                {
                    var memo = db.Memorize.Where(o => o.Index == number && o.Date.Equals(date));

                    if (memo.Any())
                    {
                        var info = new Dictionary<DateTime, string>(16);
                        var temp = new Dictionary<string, long>();
                        long recentUnrealized = 0;

                        foreach (var str in db.Memorize.Where(o => o.Index == number).Select(o => new
                        {
                            o.Date,
                            o.Unrealized,
                            o.Cumulative
                        }).OrderBy(o => o.Date))
                            if (long.TryParse(str.Unrealized, out long unrealized) && long.TryParse(str.Cumulative, out long cumulative))
                            {
                                temp[str.Date] = cumulative;
                                recentUnrealized = unrealized;
                            }
                        var last = temp.Last();
                        temp[last.Key] = last.Value + recentUnrealized;
                        string code = memo.First().Code;

                        foreach (var kv in temp.OrderBy(o => o.Key))
                            if (DateTime.TryParseExact(string.Concat(kv.Key, "154500"), CallUpStatisticalAnalysis.date, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime infoDate) && long.TryParse(string.Concat(kv.Key, "154500000"), out long find))
                            {
                                var recent = db.Futures.Where(o => o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(3)) && o.Date == find);

                                if (recent.Any())
                                    info[infoDate] = string.Concat(kv.Value, ';', recent.FirstOrDefault().Price);

                                else
                                    info[infoDate] = string.Concat(kv.Value, ';', db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(3)) && o.Date.Equals(find.ToString())).FirstOrDefault().Price);
                            }
                        return info;
                    }
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, string.Concat("No.", number));
            }
            return null;
        }
        protected string SetDate(string code)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    return db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3)) && (o.Date.Contains("1545") || o.Date.Contains("0500"))).Max(o => o.Date).Trim();
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return DateTime.Now.ToString(date);
        }
        protected Queue<Quotes> GetQuotes(string code)
        {
            var chart = new Queue<Quotes>();

            if (code.Length > 6 && code.Substring(5, 3).Equals(futures))
                try
                {
                    if (GetFileExists(new FileInfo(File)))
                    {
                        string temp = string.Empty;
                        using (var sr = new StreamReader(File))
                            if (sr != null)
                                while (sr.EndOfStream == false)
                                {
                                    var str = sr.ReadLine().Split(',');
                                    temp = str[0];

                                    if (str.Length == 3)
                                    {
                                        chart.Enqueue(new Quotes
                                        {
                                            Time = str[0],
                                            Price = str[1],
                                            Volume = str[2]
                                        });
                                        continue;
                                    }
                                    chart.Enqueue(new Quotes
                                    {
                                        Time = str[0],
                                        SellPrice = str[1],
                                        SellQuantity = str[2],
                                        SellAmount = str[3],
                                        BuyPrice = str[4],
                                        BuyQuantity = str[5],
                                        BuyAmount = str[6]
                                    });
                                }
                        var storage = new Stack<Quotes>();
                        using (var db = new GoblinBatDbContext(key))
                            foreach (var datum in db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(5)) && o.Date.CompareTo(temp) > 0).Select(o => new
                            {
                                o.Date,
                                o.Price,
                                o.Volume,
                                o.SellPrice,
                                o.SellQuantity,
                                o.TotalSellAmount,
                                o.BuyPrice,
                                o.BuyQuantity,
                                o.TotalBuyAmount
                            }).OrderByDescending(o => o.Date))
                            {
                                if (datum.Price == null && datum.Volume == null)
                                {
                                    storage.Push(new Quotes
                                    {
                                        Time = datum.Date,
                                        SellPrice = datum.SellPrice,
                                        SellQuantity = datum.SellQuantity,
                                        SellAmount = datum.TotalSellAmount,
                                        BuyPrice = datum.BuyPrice,
                                        BuyQuantity = datum.BuyQuantity,
                                        BuyAmount = datum.TotalBuyAmount
                                    });
                                    continue;
                                }
                                storage.Push(new Quotes
                                {
                                    Time = datum.Date,
                                    Price = datum.Price,
                                    Volume = datum.Volume
                                });
                            }
                        using (var sw = new StreamWriter(File, true))
                            while (storage.Count > 0)
                            {
                                var str = storage.Pop();

                                if (str.Price == null && str.Volume == null)
                                {
                                    sw.WriteLine(string.Concat(str.Time, ",", str.SellPrice, ",", str.SellQuantity, ",", str.SellAmount, ",", str.BuyPrice, ",", str.BuyQuantity, ",", str.BuyAmount));
                                    chart.Enqueue(new Quotes
                                    {
                                        Time = str.Time,
                                        SellPrice = str.SellPrice,
                                        SellQuantity = str.SellQuantity,
                                        SellAmount = str.SellAmount,
                                        BuyPrice = str.BuyPrice,
                                        BuyQuantity = str.BuyQuantity,
                                        BuyAmount = str.BuyAmount
                                    });
                                    continue;
                                }
                                sw.WriteLine(string.Concat(str.Time, ",", str.Price, ",", str.Volume));
                                chart.Enqueue(new Quotes
                                {
                                    Time = str.Time,
                                    Price = str.Price,
                                    Volume = str.Volume
                                });
                            }
                        return chart;
                    }
                    else
                        using (var db = new GoblinBatDbContext(key))
                            foreach (var temp in db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3))).Select(o => new
                            {
                                o.Date,
                                o.Price,
                                o.Volume,
                                o.SellPrice,
                                o.SellQuantity,
                                o.TotalSellAmount,
                                o.BuyPrice,
                                o.BuyQuantity,
                                o.TotalBuyAmount
                            }).OrderBy(o => o.Date))
                            {
                                if (uint.TryParse(temp.Date.Substring(0, 8), out uint date) && date < 20040318)
                                    continue;

                                if (temp.Price == null && temp.Volume == null)
                                {
                                    chart.Enqueue(new Quotes
                                    {
                                        Time = temp.Date,
                                        SellPrice = temp.SellPrice,
                                        SellQuantity = temp.SellQuantity,
                                        SellAmount = temp.TotalSellAmount,
                                        BuyPrice = temp.BuyPrice,
                                        BuyQuantity = temp.BuyQuantity,
                                        BuyAmount = temp.TotalBuyAmount
                                    });
                                    continue;
                                }
                                chart.Enqueue(new Quotes
                                {
                                    Time = temp.Date,
                                    Price = temp.Price,
                                    Volume = temp.Volume
                                });
                            }
                    SetQuotesFile(chart);
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, code);
                }
            return chart;
        }
        protected string GetStrategy()
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    return db.Codes.First(c => c.Info.Equals(db.Codes.Where(o => o.Code.Length == 8 && o.Code.Substring(0, 3).Equals(kospi200f) && o.Code.Substring(5, 3).Equals(futures)).Max(o => o.Info))).Code;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return string.Empty;
        }
        protected List<long> GetStrategy(string rate)
        {
            var list = new List<long>(128);

            try
            {
                using (var db = new GoblinBatDbContext(key))
                    foreach (var index in db.Strategy.Where(o => o.MarginRate.Equals(rate)).Select(o => new
                    {
                        o.Index
                    }))
                        list.Add(index.Index);
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return list;
        }
        protected Specify[] GetStrategy(long index)
        {
            var temp = new Specify[10];
            var list = new List<Strategics>();
            int i = 0;

            try
            {
                using (var db = new GoblinBatDbContext(key))
                    list = db.Strategy.Where(o => o.Index == index).ToList();

                var find = list.First();

                while (i < temp.Length)
                    switch (i)
                    {
                        case 0:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.BaseTime),
                                Short = int.Parse(find.BaseShort),
                                Long = int.Parse(find.BaseLong)
                            };
                            break;

                        case 1:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.NonaTime),
                                Short = int.Parse(find.NonaShort),
                                Long = int.Parse(find.NonaLong)
                            };
                            break;

                        case 2:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.OctaTime),
                                Short = int.Parse(find.OctaShort),
                                Long = int.Parse(find.OctaLong)
                            };
                            break;

                        case 3:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.HeptaTime),
                                Short = int.Parse(find.HeptaShort),
                                Long = int.Parse(find.HeptaLong)
                            };
                            break;

                        case 4:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.HexaTime),
                                Short = int.Parse(find.HexaShort),
                                Long = int.Parse(find.HexaLong)
                            };
                            break;

                        case 5:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.PentaTime),
                                Short = int.Parse(find.PantaShort),
                                Long = int.Parse(find.PantaLong)
                            };
                            break;

                        case 6:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.QuadTime),
                                Short = int.Parse(find.QuadShort),
                                Long = int.Parse(find.QuadLong)
                            };
                            break;

                        case 7:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.TriTime),
                                Short = int.Parse(find.TriShort),
                                Long = int.Parse(find.TriLong)
                            };
                            break;

                        case 8:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.DuoTime),
                                Short = int.Parse(find.DuoShort),
                                Long = int.Parse(find.DuoLong)
                            };
                            break;

                        case 9:
                            temp[i++] = new Specify
                            {
                                Index = index,
                                Assets = ulong.Parse(find.Assets) * ar,
                                Code = GetConvertCode(find.Code),
                                Commission = uint.Parse(find.Commission) / (double)cr,
                                MarginRate = double.Parse(find.MarginRate) / mr,
                                Strategy = find.Strategy,
                                RollOver = find.RollOver.Equals("T") ? true : false,
                                Time = uint.Parse(find.MonoTime),
                                Short = int.Parse(find.MonoShort),
                                Long = int.Parse(find.MonoLong)
                            };
                            break;
                    }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, string.Concat("No.", index));
            }
            return temp;
        }
        protected long GetRepositoryID(Specify[] specifies)
        {
            try
            {
                int index = 0;
                string[] time = new string[specifies.Length], st = new string[specifies.Length], lt = new string[specifies.Length];

                foreach (var str in specifies.OrderByDescending(o => o.Time))
                {
                    time[index] = str.Time.ToString("D4");
                    st[index] = str.Short.ToString("D4");
                    lt[index++] = str.Long.ToString("D4");
                }
                string bt = time[0], bs = st[0], bl = lt[0], nt = time[1], ns = st[1], nl = lt[1], ot = time[2], os = st[2], ol = lt[2], ht = time[3], hs = st[3], hl = lt[3], xt = time[4], xs = st[4], xl = lt[4], pt = time[5], ps = st[5], pl = lt[5], qt = time[6], qs = st[6], ql = lt[6], tt = time[7], ts = st[7], tl = lt[7], dt = time[8], ds = st[8], dl = lt[8], mt = time[9], ms = st[9], ml = lt[9], name = specifies[0].Strategy, ro = specifies[0].RollOver.ToString().Substring(0, 1), margin = (specifies[0].MarginRate * mr).ToString(), commission = (specifies[0].Commission * cr).ToString(), code = string.Concat(specifies[0].Code.Substring(0, 3), specifies[0].Code.Substring(5)), assets = (specifies[0].Assets / ar).ToString("D4");
                using (var db = new GoblinBatDbContext(key))
                {
                    var temp = db.Strategy.Where(o => o.BaseTime.Equals(bt) && o.BaseShort.Equals(bs) && o.BaseLong.Equals(bl) && o.NonaTime.Equals(nt) && o.NonaShort.Equals(ns) && o.NonaLong.Equals(nl) && o.OctaTime.Equals(ot) && o.OctaLong.Equals(ol) && o.OctaShort.Equals(os) && o.HeptaTime.Equals(ht) && o.HeptaShort.Equals(hs) && o.HeptaLong.Equals(hl) && o.HexaTime.Equals(xt) && o.HexaShort.Equals(xs) && o.HexaLong.Equals(xl) && o.PentaTime.Equals(pt) && o.PantaShort.Equals(ps) && o.PantaLong.Equals(pl) && o.QuadTime.Equals(qt) && o.QuadShort.Equals(qs) && o.QuadLong.Equals(ql) && o.TriTime.Equals(tt) && o.TriShort.Equals(ts) && o.TriLong.Equals(tl) && o.DuoTime.Equals(dt) && o.DuoShort.Equals(ds) && o.DuoLong.Equals(dl) && o.MonoTime.Equals(mt) && o.MonoShort.Equals(ms) && o.MonoLong.Equals(ml) && o.Assets.Equals(assets) && o.Code.Equals(code) && o.Commission.Equals(commission) && o.MarginRate.Equals(margin) && o.Strategy.Equals(name) && o.RollOver.Equals(ro));

                    if (temp.Any())
                        return temp.First().Index;

                    db.Strategy.AddOrUpdate(new Strategics
                    {
                        Assets = assets,
                        Code = code,
                        Commission = commission,
                        MarginRate = margin,
                        Strategy = name,
                        RollOver = ro,
                        BaseTime = bt,
                        BaseShort = bs,
                        BaseLong = bl,
                        NonaTime = nt,
                        NonaShort = ns,
                        NonaLong = nl,
                        OctaTime = ot,
                        OctaShort = os,
                        OctaLong = ol,
                        HeptaTime = ht,
                        HeptaShort = hs,
                        HeptaLong = hl,
                        HexaTime = xt,
                        HexaShort = xs,
                        HexaLong = xl,
                        PentaTime = pt,
                        PantaShort = ps,
                        PantaLong = pl,
                        QuadTime = qt,
                        QuadShort = qs,
                        QuadLong = ql,
                        TriTime = tt,
                        TriShort = ts,
                        TriLong = tl,
                        DuoTime = dt,
                        DuoShort = ds,
                        DuoLong = dl,
                        MonoTime = mt,
                        MonoShort = ms,
                        MonoLong = ml
                    });
                    return db.BatchSaveChanges();
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, specifies[0].Assets.ToString("N0"));
            }
            return 0;
        }
        protected async Task SetStatisticalStorage(Queue<Memorize> memo)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    await db.BulkInsertAsync(memo, o =>
                    {
                        o.InsertIfNotExists = true;
                        o.ColumnPrimaryKeyExpression = x => new
                        {
                            x.Index,
                            x.Date,
                            x.Code
                        };
                        o.BatchSize = 100;
                        o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                        o.AutoMapOutputDirection = false;
                    });
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, string.Concat("No.", memo.First().Index));
            }
        }
        protected bool GetDuplicateResults(long index, string date)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    if (db.Memorize.Where(o => o.Index == index && o.Date.Equals(date)).Any())
                        return true;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, string.Concat("No.", index));
            }
            return false;
        }
        void SetQuotesFile(Queue<Quotes> quotes)
        {
            using (var sw = new StreamWriter(File, true))
                foreach (var str in quotes.OrderBy(o => o.Time))
                {
                    if (str.Price == null && str.Volume == null)
                    {
                        sw.WriteLine(string.Concat(str.Time, ",", str.SellPrice, ",", str.SellQuantity, ",", str.SellAmount, ",", str.BuyPrice, ",", str.BuyQuantity, ",", str.BuyAmount));

                        continue;
                    }
                    sw.WriteLine(string.Concat(str.Time, ",", str.Price, ",", str.Volume));
                }
        }
        protected virtual string GetConvertCode(string code) => code;
        protected CallUpStatisticalAnalysis(string key) : base(key) => this.key = key;
        protected string File => string.Concat(Path, @"\", quotes);
        bool GetFileExists(FileInfo info) => info.Exists;
        readonly string key;
        const int ar = 10000000;
        const int mr = 100;
        const int cr = 1000000;
        const string recent = "yyMMdd";
        const string date = "yyMMddHHmmss";
        const string quotes = "QuotesA.res";
    }
}