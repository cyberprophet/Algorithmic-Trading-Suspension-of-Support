using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpStatisticalAnalysis : CallUpGoblinBat
    {
        protected Dictionary<DateTime, string> GetInformation(Catalog.DataBase.ImitationGame game, string code)
        {
            try
            {
                var info = new Dictionary<DateTime, string>(32);
                var temp = new Dictionary<string, long>(32);
                string date = new Secret().RecentDate;
                using (var db = new GoblinBatDbContext(key))
                    foreach (var str in db.Games.Where(o => o.Assets == game.Assets && o.Code.Equals(game.Code) && o.Commission == game.Commission && o.MarginRate == game.MarginRate && o.Strategy.Equals(game.Strategy) && o.RollOver.Equals(game.RollOver) && o.BaseTime == game.BaseTime && o.BaseShort == game.BaseShort && o.BaseLong == game.BaseLong && o.NonaTime == game.NonaTime && o.NonaShort == game.NonaShort && o.NonaLong == game.NonaLong && o.OctaTime == game.OctaTime && o.OctaShort == game.OctaShort && o.OctaLong == game.OctaLong && o.HeptaTime == game.HeptaTime && o.HeptaShort == game.HeptaShort && o.HeptaLong == game.HeptaLong && o.HexaTime == game.HexaTime && o.HexaShort == game.HexaShort && o.HexaLong == game.HexaLong && o.PentaTime == game.PentaTime && o.PentaShort == game.PentaShort && o.PentaLong == game.PentaLong && o.QuadTime == game.QuadTime && o.QuadShort == game.QuadShort && o.QuadLong == game.QuadLong && o.TriTime == game.TriTime && o.TriShort == game.TriShort && o.TriLong == game.TriLong && o.DuoTime == game.DuoTime && o.DuoShort == game.DuoShort && o.DuoLong == game.DuoLong && o.MonoTime == game.MonoTime && o.MonoShort == game.MonoShort && o.MonoLong == game.MonoLong).AsNoTracking().OrderBy(o => o.Date).Select(o => new { o.Date, o.Unrealized, o.Cumulative }))
                        temp[str.Date] = str.Cumulative + str.Unrealized;

                foreach (var kv in temp.OrderBy(o => o.Key))
                    if (DateTime.TryParseExact(string.Concat(kv.Key, "154500"), CallUpStatisticalAnalysis.date, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime infoDate) && long.TryParse(string.Concat(kv.Key, "154500000"), out long find))
                        using (var db = new GoblinBatDbContext(key))
                        {
                            var recent = db.Futures.Where(o => o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(5)) && o.Date == find).AsNoTracking();

                            if (recent.Any())
                                info[infoDate] = string.Concat(kv.Value, ';', recent.FirstOrDefault().Price);

                            else
                                info[infoDate] = string.Concat(kv.Value, ';', db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(5)) && o.Date.Equals(find.ToString())).AsNoTracking().FirstOrDefault().Price);
                        }
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
        protected List<long> GetStrategy(string rate)
        {
            var path = System.IO.Path.Combine(Application.StartupPath, strategy);
            var exists = new DirectoryInfo(path);
            long temp = 0;
            var list = new List<long>();

            try
            {
                if (GetDirectoryExists(exists))
                {
                    using (var sr = new StreamReader(string.Concat(path, @"\", code)))
                        if (sr != null)
                            while (sr.EndOfStream == false)
                                if (long.TryParse(sr.ReadLine(), out long number))
                                {
                                    list.Add(number);
                                    temp = number;
                                }
                    var presence = new List<long>();
                    using (var db = new GoblinBatDbContext(key))
                    {
                        var being = db.Strategy.Where(o => o.MarginRate.Equals(rate) && o.Index > temp);

                        if (being.Any())
                            foreach (var num in being.OrderBy(o => o.Index).Select(o => new { o.Index }).AsNoTracking())
                                presence.Add(num.Index);

                        else
                            return list;
                    }
                    SetStrategyDirectory(path, presence);
                    list.AddRange(presence);

                    return list;
                }
                else
                    using (var db = new GoblinBatDbContext(key))
                        foreach (var index in db.Strategy.Where(o => o.MarginRate.Equals(rate)).OrderBy(o => o.Index).Select(o => new { o.Index }).AsNoTracking())
                            list.Add(index.Index);

                exists.Create();
                SetStrategyDirectory(path, list);
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
                    list = db.Strategy.Where(o => o.Index == index).AsNoTracking().ToList();

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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
                                RollOver = find.RollOver.Equals("T"),
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
        protected bool GetRepositoryID(Catalog.DataBase.ImitationGame game)
        {
            try
            {
                var date = new Secret().RecentDate;
                using (var db = new GoblinBatDbContext(key))
                    if (db.Games.Where(o => o.Assets == game.Assets && o.Code.Equals(game.Code) && o.Commission == game.Commission && o.MarginRate == game.MarginRate && o.Strategy.Equals(game.Strategy) && o.RollOver.Equals(game.RollOver) && o.BaseTime == game.BaseTime && o.BaseShort == game.BaseShort && o.BaseLong == game.BaseLong && o.NonaTime == game.NonaTime && o.NonaShort == game.NonaShort && o.NonaLong == game.NonaLong && o.OctaTime == game.OctaTime && o.OctaShort == game.OctaShort && o.OctaLong == game.OctaLong && o.HeptaTime == game.HeptaTime && o.HeptaShort == game.HeptaShort && o.HeptaLong == game.HeptaLong && o.HexaTime == game.HexaTime && o.HexaShort == game.HexaShort && o.HexaLong == game.HexaLong && o.PentaTime == game.PentaTime && o.PentaShort == game.PentaShort && o.PentaLong == game.PentaLong && o.QuadTime == game.QuadTime && o.QuadShort == game.QuadShort && o.QuadLong == game.QuadLong && o.TriTime == game.TriTime && o.TriShort == game.TriShort && o.TriLong == game.TriLong && o.DuoTime == game.DuoTime && o.DuoShort == game.DuoShort && o.DuoLong == game.DuoLong && o.MonoTime == game.MonoTime && o.MonoShort == game.MonoShort && o.MonoLong == game.MonoLong && o.Date.Equals(date)).AsNoTracking().Any())
                        return true;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return false;
        }
        protected bool SetStatisticalStorage(Queue<ImitationGames> memo)
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    while (memo.Count > 0)
                        db.Games.AddOrUpdate(memo.Dequeue());

                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return false;
        }
        protected async Task<string> GetRecentDate()
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    return await db.Games.MaxAsync(o => o.Date);
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return string.Empty;
        }
        protected bool GetDuplicateResults(ImitationGames game, string date)
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var check = db.Games.Where(o => o.Assets == game.Assets && o.Code.Equals(game.Code) && o.Commission == game.Commission && o.MarginRate == game.MarginRate && o.Strategy.Equals(game.Strategy) && o.RollOver.Equals(game.RollOver) && o.BaseTime == game.BaseTime && o.BaseShort == game.BaseShort && o.BaseLong == game.BaseLong && o.NonaTime == game.NonaTime && o.NonaShort == game.NonaShort && o.NonaLong == game.NonaLong && o.OctaTime == game.OctaTime && o.OctaShort == game.OctaShort && o.OctaLong == game.OctaLong && o.HeptaTime == game.HeptaTime && o.HeptaShort == game.HeptaShort && o.HeptaLong == game.HeptaLong && o.HexaTime == game.HexaTime && o.HexaShort == game.HexaShort && o.HexaLong == game.HexaLong && o.PentaTime == game.PentaTime && o.PentaShort == game.PentaShort && o.PentaLong == game.PentaLong && o.QuadTime == game.QuadTime && o.QuadShort == game.QuadShort && o.QuadLong == game.QuadLong && o.TriTime == game.TriTime && o.TriShort == game.TriShort && o.TriLong == game.TriLong && o.DuoTime == game.DuoTime && o.DuoShort == game.DuoShort && o.DuoLong == game.DuoLong && o.MonoTime == game.MonoTime && o.MonoShort == game.MonoShort && o.MonoLong == game.MonoLong).AsNoTracking();

                    if (check.Any())
                    {
                        if (check.Any(o => o.Date.Equals(date)))
                            return true;

                        var max = check.Max(o => o.Date);

                        return check.Where(o => o.Date.Equals(max)).Any(o => o.Cumulative < 0 || o.Statistic < 0);
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
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
        void SetStrategyDirectory(string path, List<long> list)
        {
            using (var sw = new StreamWriter(string.Concat(path, @"\", code), true))
                foreach (var str in list)
                    sw.WriteLine(str);
        }
        protected virtual string GetConvertCode(string code) => code;
        protected CallUpStatisticalAnalysis(string key) : base(key) => this.key = key;
        protected string File => string.Concat(Path, @"\", quotes);
        bool GetDirectoryExists(DirectoryInfo directory) => directory.Exists;
        bool GetFileExists(FileInfo info) => info.Exists;
        protected const int ar = 10000000;
        protected const int mr = 100;
        protected const int cr = 1000000;
        readonly string key;
        const string date = "yyMMddHHmmss";
        const string quotes = "QuotesA.res";
        const string strategy = "Strategy";
        const string code = "101000.res";
    }
}