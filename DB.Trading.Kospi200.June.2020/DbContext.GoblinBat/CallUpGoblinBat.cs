using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpGoblinBat
    {
        protected Dictionary<long, long> GetBestStrategyRecommend()
        {
            var date = new Secret().RecentDate;
            var indexes = new Dictionary<long, long>();

            try
            {
                using (var db = new GoblinBatDbContext(key))
                    foreach (var str in db.Memorize.Where(o => o.Date.Equals(date)).OrderByDescending(o => o.Statistic).Take(1000).Select(o => new
                    {
                        o.Cumulative,
                        o.Unrealized,
                        o.Index,
                        o.Statistic
                    }).AsNoTracking())
                        if (long.TryParse(str.Unrealized, out long unrealized) && long.TryParse(str.Cumulative, out long cumulative))
                            indexes[str.Index] = unrealized + cumulative;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return indexes;
        }
        protected string[] GetUserIdentify()
        {
            try
            {
                var identity = new Secret().GetIdentify(key);
                using (var db = new GoblinBatDbContext(key))
                    foreach (var str in db.Logs.Where(o => o.Identity.Equals(identity)).OrderByDescending(o => o.Date).Select(o => new
                    {
                        o.Assets,
                        o.Code,
                        o.Commission,
                        o.Strategy,
                        o.RollOver
                    }).AsNoTracking())
                        return new string[]
                        {
                            str.Assets,
                            str.Code,
                            str.Commission,
                            marginRate,
                            str.Strategy,
                            str.RollOver
                        };
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return null;
        }
        protected List<string[]> GetUserIdentity(DateTime date)
        {
            var list = new List<string[]>();

            try
            {
                var convert = date.ToString(recent);
                using (var db = new GoblinBatDbContext(key))
                    foreach (var str in db.Logs.Where(o => o.Date.Equals(convert)).Select(o => new
                    {
                        o.Assets,
                        o.Code,
                        o.Commission,
                        o.Strategy,
                        o.RollOver
                    }).AsNoTracking())
                    {
                        var temp = new string[]
                        {
                            str.Assets,
                            str.Code,
                            str.Commission,
                            marginRate,
                            str.Strategy,
                            str.RollOver
                        };
                        if (list.Exists(o => o[0].Equals(temp[0]) && o[1].Equals(temp[1]) && o[2].Equals(temp[2]) && o[3].Equals(temp[3]) && o[4].Equals(temp[4]) && o[5].Equals(temp[5])) == false)
                            list.Add(temp);
                    }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return list;
        }
        protected async Task<int> SetIndentify(Setting setting)
        {
            string temp = string.Empty;

            switch (setting.RollOver)
            {
                case CheckState.Checked:
                    temp = "T";
                    break;

                case CheckState.Unchecked:
                    temp = "F";
                    break;

                case CheckState.Indeterminate:
                    temp = "A";
                    break;
            }
            try
            {
                using (var db = new GoblinBatDbContext(key))
                {
                    db.Logs.AddOrUpdate(new Logs
                    {
                        Identity = new Secret().GetIdentify(key),
                        Date = DateTime.Now.ToString(recent),
                        Assets = (setting.Assets / ar).ToString("D4"),
                        Strategy = setting.Strategy,
                        Commission = (setting.Commission * cr).ToString(),
                        RollOver = temp,
                        Code = string.Concat(setting.Code.Substring(0, 3), setting.Code.Substring(5))
                    });
                    return await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, setting.Strategy);
            }
            return 1;
        }
        protected async Task SetDeleteDuplicateData(List<Strategics> strategics)
        {
            var list = new List<long>();

            foreach (var str in strategics)
                using (var db = new GoblinBatDbContext(key))
                {
                    var strategy = db.Strategy.Where(o => o.Assets.Equals(str.Assets) && o.Code.Equals(str.Code) && o.Commission.Equals(str.Commission) && o.MarginRate.Equals(str.MarginRate) && o.Strategy.Equals(str.Strategy) && o.RollOver.Equals(str.RollOver) && o.BaseTime.Equals(str.BaseTime) && o.BaseShort.Equals(str.BaseShort) && o.BaseLong.Equals(str.BaseLong) && o.NonaTime.Equals(str.NonaTime) && o.NonaShort.Equals(str.NonaShort) && o.NonaLong.Equals(str.NonaLong) && o.OctaTime.Equals(str.OctaTime) && o.OctaShort.Equals(str.OctaShort) && o.OctaLong.Equals(str.OctaLong) && o.HeptaTime.Equals(str.HeptaTime) && o.HeptaShort.Equals(str.HeptaShort) && o.HeptaLong.Equals(str.HeptaLong) && o.HexaTime.Equals(str.HexaTime) && o.HexaShort.Equals(str.HexaShort) && o.HexaLong.Equals(str.HexaLong) && o.PentaTime.Equals(str.PentaTime) && o.PantaShort.Equals(str.PantaShort) && o.PantaLong.Equals(str.PantaLong) && o.QuadTime.Equals(str.QuadTime) && o.QuadShort.Equals(str.QuadShort) && o.QuadLong.Equals(str.QuadLong) && o.TriTime.Equals(str.TriTime) && o.TriShort.Equals(str.TriShort) && o.TriLong.Equals(str.TriLong) && o.DuoTime.Equals(str.DuoTime) && o.DuoShort.Equals(str.DuoShort) && o.DuoLong.Equals(str.DuoLong) && o.MonoTime.Equals(str.MonoTime) && o.MonoShort.Equals(str.MonoShort) && o.MonoLong.Equals(str.MonoLong));
                    int i = 0;

                    try
                    {
                        foreach (var temp in strategy.Select(o => new
                        {
                            o.Index
                        }).AsNoTracking())
                        {
                            if (i++ > 0)
                                list.Add(temp.Index);
                        }
                        i = 0;
                    }
                    catch (Exception ex)
                    {
                        new ExceptionMessage(ex.StackTrace);
                    }
                }
            if (list.Count > 0)
                try
                {
                    var remove = new List<Strategics>();
                    using (var db = new GoblinBatDbContext(key))
                    {
                        foreach (var index in list)
                            remove.Add(db.Strategy.Where(o => o.Index == index).First());

                        await db.Strategy.BulkDeleteAsync(db.Strategy.Where(o => o.Index > 909770), o => o.BatchSize = 50000);
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
        }
        protected async Task SetInsertStrategy(List<Strategics> strategy)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                {
                    var str = strategy.First();

                    if (db.Strategy.Where(o => o.Assets.Equals(str.Assets) && o.Code.Equals(str.Code) && o.Commission.Equals(str.Commission) && o.MarginRate.Equals(str.MarginRate) && o.Strategy.Equals(str.Strategy) && o.RollOver.Equals(str.RollOver) && o.BaseTime.Equals(str.BaseTime) && o.BaseShort.Equals(str.BaseShort) && o.BaseLong.Equals(str.BaseLong) && o.NonaTime.Equals(str.NonaTime) && o.NonaShort.Equals(str.NonaShort) && o.NonaLong.Equals(str.NonaLong) && o.OctaTime.Equals(str.OctaTime) && o.OctaShort.Equals(str.OctaShort) && o.OctaLong.Equals(str.OctaLong) && o.HeptaTime.Equals(str.HeptaTime) && o.HeptaShort.Equals(str.HeptaShort) && o.HeptaLong.Equals(str.HeptaLong) && o.HexaTime.Equals(str.HexaTime) && o.HexaShort.Equals(str.HexaShort) && o.HexaLong.Equals(str.HexaLong) && o.PentaTime.Equals(str.PentaTime) && o.PantaShort.Equals(str.PantaShort) && o.PantaLong.Equals(str.PantaLong) && o.QuadTime.Equals(str.QuadTime) && o.QuadShort.Equals(str.QuadShort) && o.QuadLong.Equals(str.QuadLong) && o.TriTime.Equals(str.TriTime) && o.TriShort.Equals(str.TriShort) && o.TriLong.Equals(str.TriLong) && o.DuoTime.Equals(str.DuoTime) && o.DuoShort.Equals(str.DuoShort) && o.DuoLong.Equals(str.DuoLong) && o.MonoTime.Equals(str.MonoTime) && o.MonoShort.Equals(str.MonoShort) && o.MonoLong.Equals(str.MonoLong)).Any() == false)
                        await db.BulkInsertAsync(strategy, o =>
                        {
                            o.InsertIfNotExists = true;
                            o.ColumnPrimaryKeyExpression = x => new
                            {
                                x.Assets,
                                x.Code,
                                x.Commission,
                                x.MarginRate,
                                x.Strategy,
                                x.RollOver,
                                x.BaseTime,
                                x.BaseShort,
                                x.BaseLong,
                                x.NonaTime,
                                x.NonaShort,
                                x.NonaLong,
                                x.OctaTime,
                                x.OctaShort,
                                x.OctaLong,
                                x.HeptaTime,
                                x.HeptaShort,
                                x.HeptaLong,
                                x.HexaTime,
                                x.HexaShort,
                                x.HexaLong,
                                x.PentaTime,
                                x.PantaShort,
                                x.PantaLong,
                                x.QuadTime,
                                x.QuadShort,
                                x.QuadLong,
                                x.TriTime,
                                x.TriShort,
                                x.TriLong,
                                x.DuoTime,
                                x.DuoShort,
                                x.DuoLong,
                                x.MonoTime,
                                x.MonoShort,
                                x.MonoLong
                            };
                            o.BatchSize = 50000;
                            o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                            o.AutoMapOutputDirection = false;
                        });
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, strategy.GetType().Name);
            }
        }
        protected bool GetRemainingDate(string code, long date)
        {
            try
            {
                if (code.Length == 8 && date.ToString().Substring(6).Equals("151959000"))
                    using (var db = new GoblinBatDbContext(key))
                        if (db.Codes.AsNoTracking().FirstOrDefault(o => o.Code.Equals(code)).Info.Substring(2).Equals(date.ToString().Substring(0, 6)))
                            return true;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return false;
        }
        protected Queue<Chart> GetChart(string code)
        {
            var chart = new Queue<Chart>();
            var exists = new DirectoryInfo(Path);

            if (code.Length > 6 && code.Substring(5, 3).Equals(futures))
                try
                {
                    if (GetDirectoryExists(exists))
                    {
                        using (var sr = new StreamReader(string.Concat(Path, @"\", basic)))
                            if (sr != null)
                                while (sr.EndOfStream == false)
                                {
                                    var str = sr.ReadLine().Split(',');

                                    if (long.TryParse(str[0], out long time) && double.TryParse(str[1], out double price) && int.TryParse(str[2], out int volume))
                                        chart.Enqueue(new Chart
                                        {
                                            Date = time,
                                            Price = price,
                                            Volume = volume
                                        });
                                }
                        return chart;
                    }
                    else
                        using (var db = new GoblinBatDbContext(key))
                        {
                            var tick = db.Futures.Where(o => o.Code.Contains(code.Substring(0, 3))).OrderBy(o => o.Date).Select(o => new
                            {
                                o.Code,
                                o.Date,
                                o.Price,
                                o.Volume
                            }).AsNoTracking();
                            var min = int.Parse(tick.Min(o => o.Date).ToString().Substring(0, 6));
                            var remaining = db.Codes.Where(o => o.Code.Length == 8 && o.Code.Contains(code.Substring(0, 3))).OrderBy(o => o.Info).Select(o => new
                            {
                                o.Code,
                                o.Info
                            }).AsNoTracking().ToList();

                            foreach (var temp in db.Days.Where(o => o.Code.Length == 8 && o.Code.Contains(code.Substring(0, 3))).OrderBy(o => o.Date).Select(o => new
                            {
                                o.Date,
                                o.Price
                            }).AsNoTracking())
                                if (int.Parse(temp.Date.ToString().Substring(2)) < min)
                                    chart.Enqueue(new Chart
                                    {
                                        Date = temp.Date,
                                        Price = temp.Price,
                                        Volume = 0
                                    });
                            foreach (var temp in tick)
                            {
                                int index = remaining.FindIndex(o => o.Code.Equals(temp.Code)) - 1;

                                if (index > -1 && int.TryParse(temp.Date.ToString().Substring(0, 6), out int date) && int.TryParse(remaining[index].Info.Substring(2), out int remain))
                                {
                                    if (date < remain)
                                        continue;

                                    else if (date > 200403)
                                        break;
                                }
                                chart.Enqueue(new Chart
                                {
                                    Date = temp.Date,
                                    Price = temp.Price,
                                    Volume = temp.Volume
                                });
                            }
                        }
                    exists.Create();
                    SetChartDirectory(chart);
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, code);
                }
            return chart;
        }
        protected string GetRecentFuturesCode(bool register)
        {
            if (register == false)
                try
                {
                    using (var db = new GoblinBatDbContext(key))
                        return db.Codes.AsNoTracking().First(code => code.Info.Equals(db.Codes.Where(o => o.Code.Substring(0, 3).Equals(kospi200f) && o.Code.Substring(5, 3).Equals(futures)).Max(o => o.Info))).Code;
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return string.Empty;
        }
        void SetChartDirectory(Queue<Chart> chart)
        {
            using (var sw = new StreamWriter(string.Concat(Path, @"\", basic), true))
                foreach (var str in chart.OrderBy(o => o.Date))
                    sw.WriteLine(string.Concat(str.Date, ',', str.Price, ',', str.Volume));
        }
        bool GetDirectoryExists(DirectoryInfo directory) => directory.Exists;
        protected string Path => System.IO.Path.Combine(Application.StartupPath, chart);
        protected CallUpGoblinBat(string key) => this.key = key;
        protected internal const string futures = "000";
        protected internal const string kospi200f = "101";
        const string basic = "Base.res";
        const string chart = "ChartOf101000";
        const int ar = 10000000;
        const int cr = 1000000;
        const string recent = "yyMMdd";
        const string marginRate = "16.2";
        readonly string key;
    }
}