using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpStatisticalAnalysis : CallUpGoblinBat
    {       
        protected string GetRecentDate(string max)
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    if (db.Material.Any(o => o.Date.Equals(max)))
                        return max;
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return string.Empty;
        }
        protected Dictionary<DateTime, string> GetInformation(Catalog.DataBase.ImitationGame game, string code)
        {
            try
            {
                var file = string.Empty;
                var info = new Dictionary<DateTime, string>(32);
                var temp = new Dictionary<string, long>(32);
                var contents = new Dictionary<string, string>(32);
                var now = DateTime.Now;
                using (var db = new GoblinBatDbContext(key))
                {
                    var count = 1;

                    while (count-- <= 1)
                    {
                        var find = (count == 0 ? now : now.AddDays(count)).ToString("yyMMdd");
                        var exists = db.Material.Where(o => o.Date.Equals(find) && o.Assets == game.Assets && o.Code.Equals(game.Code) && o.Commission == game.Commission && o.MarginRate == game.MarginRate && o.Strategy.Equals(game.Strategy) && o.RollOver.Equals(game.RollOver) && o.BaseTime == game.BaseTime && o.BaseShort == game.BaseShort && o.BaseLong == game.BaseLong && o.NonaTime == game.NonaTime && o.NonaShort == game.NonaShort && o.NonaLong == game.NonaLong && o.OctaTime == game.OctaTime && o.OctaShort == game.OctaShort && o.OctaLong == game.OctaLong && o.HeptaTime == game.HeptaTime && o.HeptaShort == game.HeptaShort && o.HeptaLong == game.HeptaLong && o.HexaTime == game.HexaTime && o.HexaShort == game.HexaShort && o.HexaLong == game.HexaLong && o.PentaTime == game.PentaTime && o.PentaShort == game.PentaShort && o.PentaLong == game.PentaLong && o.QuadTime == game.QuadTime && o.QuadShort == game.QuadShort && o.QuadLong == game.QuadLong && o.TriTime == game.TriTime && o.TriShort == game.TriShort && o.TriLong == game.TriLong && o.DuoTime == game.DuoTime && o.DuoShort == game.DuoShort && o.DuoLong == game.DuoLong && o.MonoTime == game.MonoTime && o.MonoShort == game.MonoShort && o.MonoLong == game.MonoLong);

                        if (exists.Any())
                        {
                            var str = exists.First();
                            temp[str.Date] = str.Cumulative + str.Unrealized;
                            contents[str.Date] = string.Concat(',', str.Unrealized, ',', str.Revenue, ',', str.Fees, ',', str.Cumulative, ',', str.Statistic);

                            if (string.IsNullOrEmpty(file))
                                file = string.Concat(str.BaseShort, '-', str.BaseLong, '-', str.MonoTime, '-', str.MonoShort, '-', str.MonoLong, '-', str.Strategy, '-', str.Primary, csv);
                        }
                        else if (find.Equals("190721"))
                            break;
                    }
                }
                if (string.IsNullOrEmpty(file) == false && contents.Count > 0)
                {
                    var queue = new Queue<string>(32);

                    foreach (var kv in contents.OrderBy(o => o.Key))
                        queue.Enqueue(string.Concat(kv.Key, kv.Value));

                    new ExceptionMessage(file, queue);
                    contents.Clear();
                }
                var dicPrice = new Dictionary<long, double>(128);
                var dicShort = new Dictionary<string, double>(128);
                var dicLong = new Dictionary<string, double>(128);
                using (var db = new GoblinBatDbContext(key))
                    foreach (var dt in db.Futures.Where(o => o.Date % 0x3B9ACA00 == 0x9357BA0 && o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(5))).AsNoTracking().Select(o => new { o.Date, o.Price }))
                        dicPrice[dt.Date / 0x3B9ACA00] = dt.Price;

                using (var db = new GoblinBatDbContext(key))
                    foreach (var chart in db.Charts.Where(o => (o.Base == game.BaseShort || o.Base == game.BaseLong) && o.Code.Equals(code) && o.Time == 405).AsNoTracking().Select(o => new { o.Date, o.Base, o.Value }))
                    {
                        if (chart.Base == game.BaseShort)
                            dicShort[chart.Date] = chart.Value;

                        if (chart.Base == game.BaseLong)
                            dicLong[chart.Date] = chart.Value;
                    }
                foreach (var kv in temp.OrderBy(o => o.Key))
                    if (dicShort.TryGetValue(kv.Key, out double bs) && dicLong.TryGetValue(kv.Key, out double bl) && long.TryParse(kv.Key, out long dp) && dicPrice.TryGetValue(dp, out double price) && DateTime.TryParseExact(string.Concat(kv.Key, "154500"), date, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime infoDate))
                        info[infoDate] = string.Concat(kv.Value, ';', price, ';', bs, ';', bl);

                return info;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return null;
        }
        protected string SetDate(string code)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    return db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3)) && (o.Date.Contains("1545") || o.Date.Contains("0500"))).AsNoTracking().Max(o => o.Date).Trim();
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return DateTime.Now.ToString(date);
        }
        protected IOrderedEnumerable<KeyValuePair<long, Queue<Quotes>>> GetQuotes(Dictionary<long, Queue<Quotes>> catalog, string code)
        {
            string path = System.IO.Path.Combine(Application.StartupPath, enumerable, code), date = string.Empty;
            var search = new List<string>();
            var exists = new DirectoryInfo(path);
            var chart = new Queue<Quotes>(2048);

            try
            {
                using (var db = new GoblinBatDbContext(key))
                    date = db.Datums.Where(o => o.Code.Equals(code)).AsNoTracking().Max(o => o.Date).Substring(0, 8);

                if (exists.Exists)
                {
                    path = System.IO.Path.Combine(Application.StartupPath, enumerable);

                    foreach (var file in Directory.GetFiles(path, "*.res", SearchOption.AllDirectories))
                    {
                        using (var sr = new StreamReader(file))
                            if (sr != null)
                                while (sr.EndOfStream == false)
                                {
                                    var str = sr.ReadLine().Split(',');

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
                        var temp = file.Split('\\');
                        search.Add(temp[temp.Length - 1].Split('.')[0]);
                        catalog[new FileInfo(file).CreationTime.Ticks] = new Queue<Quotes>(chart);
                        chart.Clear();
                        GC.Collect();
                    }
                    if (string.IsNullOrEmpty(date) == false && string.Compare(search.Max(), date) < 0)
                    {
                        var storage = new Stack<Quotes>();
                        var max = search.Max();
                        using (var db = new GoblinBatDbContext(key))
                            foreach (var datum in db.Datums.Where(o => o.Code.Equals(code)).AsNoTracking().OrderByDescending(o => o.Date))
                            {
                                if (string.Compare(datum.Date.Substring(0, 8), max) <= 0)
                                    break;

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
                        var file = string.Concat(path, @"\", code, @"\", date, res);
                        using (var sw = new StreamWriter(file, true))
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
                        catalog[new FileInfo(file).CreationTime.Ticks] = new Queue<Quotes>(chart);
                        chart.Clear();
                        GC.Collect();
                    }
                }
                else
                {
                    exists.Create();
                    using (var db = new GoblinBatDbContext(key))
                        foreach (var tick in db.Datums.Where(o => o.Code.Equals(code)).AsNoTracking().OrderBy(o => o.Date))
                        {
                            if (tick.Price == null && tick.Volume == null)
                            {
                                chart.Enqueue(new Quotes
                                {
                                    Time = tick.Date,
                                    SellPrice = tick.SellPrice,
                                    SellQuantity = tick.SellQuantity,
                                    SellAmount = tick.TotalSellAmount,
                                    BuyPrice = tick.BuyPrice,
                                    BuyQuantity = tick.BuyQuantity,
                                    BuyAmount = tick.TotalBuyAmount
                                });
                                continue;
                            }
                            chart.Enqueue(new Quotes
                            {
                                Time = tick.Date,
                                Price = tick.Price,
                                Volume = tick.Volume
                            });
                        }
                    var file = string.Concat(path, @"\", date, res);
                    using (var sw = new StreamWriter(file, true))
                        foreach (var str in chart.OrderBy(o => o.Time))
                        {
                            if (str.Price == null && str.Volume == null)
                            {
                                sw.WriteLine(string.Concat(str.Time, ",", str.SellPrice, ",", str.SellQuantity, ",", str.SellAmount, ",", str.BuyPrice, ",", str.BuyQuantity, ",", str.BuyAmount));

                                continue;
                            }
                            sw.WriteLine(string.Concat(str.Time, ',', str.Price, ',', str.Volume));
                        }
                    catalog[new FileInfo(file).LastWriteTime.Ticks] = new Queue<Quotes>(chart);
                    chart.Clear();
                    GC.Collect();
                }
                return catalog.OrderBy(o => o.Key);
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return null;
        }
        protected Queue<Quotes> GetQuotes(string code, Queue<Quotes> chart)
        {
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
                        if (temp.Equals(end))
                            return chart;

                        var storage = new Stack<Quotes>();
                        using (var db = new GoblinBatDbContext(key))
                        {
                            var recent = db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(5)) && o.Date.CompareTo(temp) > 0);

                            if (recent.Any())
                                foreach (var datum in recent.OrderByDescending(o => o.Date).Select(o => new
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
                                }).AsNoTracking())
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
                            foreach (var temp in db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3))).OrderBy(o => o.Date).Select(o => new
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
                            }).AsNoTracking())
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
                                if (temp.Date.Equals(end))
                                    return chart;
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
                    return db.Codes.AsNoTracking().First(c => c.Info.Equals(db.Codes.Where(o => o.Code.Length == 8 && o.Code.Substring(0, 3).Equals(kospi200f) && o.Code.Substring(5, 3).Equals(futures)).Max(o => o.Info))).Code;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return string.Empty;
        }
        protected bool GetRepositoryID(Catalog.DataBase.ImitationGame game, string date)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    return db.Material.Where(o => o.Assets == game.Assets && o.Code.Equals(game.Code) && o.Commission == game.Commission && o.MarginRate == game.MarginRate && o.Strategy.Equals(game.Strategy) && o.RollOver.Equals(game.RollOver) && o.BaseTime == game.BaseTime && o.BaseShort == game.BaseShort && o.BaseLong == game.BaseLong && o.NonaTime == game.NonaTime && o.NonaShort == game.NonaShort && o.NonaLong == game.NonaLong && o.OctaTime == game.OctaTime && o.OctaShort == game.OctaShort && o.OctaLong == game.OctaLong && o.HeptaTime == game.HeptaTime && o.HeptaShort == game.HeptaShort && o.HeptaLong == game.HeptaLong && o.HexaTime == game.HexaTime && o.HexaShort == game.HexaShort && o.HexaLong == game.HexaLong && o.PentaTime == game.PentaTime && o.PentaShort == game.PentaShort && o.PentaLong == game.PentaLong && o.QuadTime == game.QuadTime && o.QuadShort == game.QuadShort && o.QuadLong == game.QuadLong && o.TriTime == game.TriTime && o.TriShort == game.TriShort && o.TriLong == game.TriLong && o.DuoTime == game.DuoTime && o.DuoShort == game.DuoShort && o.DuoLong == game.DuoLong && o.MonoTime == game.MonoTime && o.MonoShort == game.MonoShort && o.MonoLong == game.MonoLong && o.Date.Equals(date)).AsNoTracking().Any();
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return false;
        }
        protected bool SetStatisticalStorage(Queue<Strategics> memo)
        {
            int count = 0;
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    while (memo.Count > 0)
                        db.Material.AddOrUpdate(memo.Dequeue());

                    count = db.SaveChanges();
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, ex.TargetSite.Name);
                }
            return count > 0;
        }
        protected bool SetStatisticalBulkStorage(Queue<Strategics> memo)
        {
            var complete = false;
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.BulkInsert(memo, o =>
                    {
                        o.InsertIfNotExists = true;
                        o.BatchSize = 13750;
                        o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                        o.AutoMapOutputDirection = false;
                    });
                    complete = true;
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, ex.TargetSite.Name);
                }
                finally
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
                }
            return complete;
        }
        protected bool GetDuplicateResults(Strategics game, string date)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    return db.Material.Where(o => o.Assets == game.Assets && o.Code.Equals(game.Code) && o.Commission == game.Commission && o.MarginRate == game.MarginRate && o.Strategy.Equals(game.Strategy) && o.RollOver.Equals(game.RollOver) && o.BaseTime == game.BaseTime && o.BaseShort == game.BaseShort && o.BaseLong == game.BaseLong && o.NonaTime == game.NonaTime && o.NonaShort == game.NonaShort && o.NonaLong == game.NonaLong && o.OctaTime == game.OctaTime && o.OctaShort == game.OctaShort && o.OctaLong == game.OctaLong && o.HeptaTime == game.HeptaTime && o.HeptaShort == game.HeptaShort && o.HeptaLong == game.HeptaLong && o.HexaTime == game.HexaTime && o.HexaShort == game.HexaShort && o.HexaLong == game.HexaLong && o.PentaTime == game.PentaTime && o.PentaShort == game.PentaShort && o.PentaLong == game.PentaLong && o.QuadTime == game.QuadTime && o.QuadShort == game.QuadShort && o.QuadLong == game.QuadLong && o.TriTime == game.TriTime && o.TriShort == game.TriShort && o.TriLong == game.TriLong && o.DuoTime == game.DuoTime && o.DuoShort == game.DuoShort && o.DuoLong == game.DuoLong && o.MonoTime == game.MonoTime && o.MonoShort == game.MonoShort && o.MonoLong == game.MonoLong).AsNoTracking().Any(o => o.Date.Equals(date));
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.TargetSite.Name, game.Strategy);
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
        protected const int ar = 10000000;
        protected const int mr = 100;
        protected const int cr = 1000000;
        readonly string key;
        const string date = "yyMMddHHmmss";
        const string quotes = "QuotesA.res";
        const string end = "200611154500001";
        const string enumerable = "Quotes";
        const string csv = ".csv";
    }
}