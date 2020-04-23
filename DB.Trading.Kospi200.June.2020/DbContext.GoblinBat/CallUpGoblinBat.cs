using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpGoblinBat : CallUpBasicInformation
    {
        protected List<Statistics> GetBasicStrategy(char initial)
        {
            var list = new List<Statistics>();
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    if (initial.Equals((char)84) == false)
                        return db.Statistics.AsNoTracking().ToList();

                    var identity = new Secret().GetIdentify(key);
                    var date = DateTime.Now.AddDays(-7).ToString(recent);
                    var choice = new bool[] { true, false };
                    var strategy = db.Statistics.Select(o => new { o.Strategy }).AsNoTracking().Distinct().ToArray();

                    foreach (var str in db.Identifies.Where(o => o.Identity.Equals(identity) && o.Date.CompareTo(date) >= 0).AsNoTracking().Select(o => new
                    {
                        o.Assets,
                        o.Code,
                        o.Commission,
                        o.Strategy,
                        o.RollOver
                    }))
                        if (str.Strategy.Equals("Auto") == false && str.RollOver.Equals(CheckState.Indeterminate) == false)
                            list.Add(new Statistics
                            {
                                Assets = str.Assets,
                                Code = str.Code,
                                Commission = str.Commission,
                                MarginRate = marginRate,
                                Strategy = str.Strategy,
                                RollOver = str.RollOver.Equals(CheckState.Checked) ? true : false
                            });
                        else if (str.RollOver.Equals(CheckState.Indeterminate) && str.Strategy.Equals("Auto") == false)
                            foreach (var tf in choice)
                                list.Add(new Statistics
                                {
                                    Assets = str.Assets,
                                    Code = str.Code,
                                    Commission = str.Commission,
                                    MarginRate = marginRate,
                                    Strategy = str.Strategy,
                                    RollOver = tf
                                });
                        else if (str.RollOver.Equals(CheckState.Indeterminate) == false && str.Strategy.Equals("Auto"))
                            foreach (var st in strategy)
                                list.Add(new Statistics
                                {
                                    Assets = str.Assets,
                                    Code = str.Code,
                                    Commission = str.Commission,
                                    MarginRate = marginRate,
                                    Strategy = st.Strategy,
                                    RollOver = str.RollOver.Equals(CheckState.Checked) ? true : false
                                });
                        else
                            foreach (var tf in choice)
                                foreach (var st in strategy)
                                    list.Add(new Statistics
                                    {
                                        Assets = str.Assets,
                                        Code = str.Code,
                                        Commission = str.Commission,
                                        MarginRate = marginRate,
                                        Strategy = st.Strategy,
                                        RollOver = tf
                                    });
                }
                catch (Exception ex)
                {
                    if (TimerBox.Show(new Secret().Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 3517).Equals(DialogResult.OK))
                        return db.Statistics.ToList();
                }
            return list.Distinct().ToList();
        }
        protected void SetInsertBaseStrategy(Queue<Statistics> queue)
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.BulkInsert(queue, o =>
                    {
                        o.InsertIfNotExists = true;
                        o.BatchSize = 150;
                        o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                        o.AutoMapOutputDirection = false;
                    });
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
                finally
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
                }
        }
        protected ImitationGames GetBestStrategyRecommend(List<Statistics> list)
        {
            var game = new ImitationGames();
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var date = new Secret().RecentDate;
                    var temp = long.MinValue;
                    var best = db.Games.Where(o => o.Date.Equals(date)).AsNoTracking();
                    var max = (long)(best.Max(o => o.Cumulative + o.Unrealized) * 0.85);
                    int statistic = (int)(best.Max(o => o.Statistic) * 0.65), remain = list.Count;

                    foreach (var choice in list)
                    {
                        var select = db.Games.Where(o => o.Date.Equals(date) && o.Assets == choice.Assets && o.Code.Equals(choice.Code) && o.Commission == choice.Commission && o.MarginRate == choice.MarginRate && o.Strategy.Equals(choice.Strategy) && o.RollOver.Equals(choice.RollOver));

                        foreach (var cu in select.OrderByDescending(o => (o.Cumulative + o.Unrealized) * o.Statistic).AsNoTracking().Take(1000))
                            if (max <= cu.Cumulative + cu.Unrealized && cu.Statistic >= statistic && temp <= (cu.Cumulative + cu.Unrealized) * cu.Statistic)
                            {
                                remain--;
                                temp = (cu.Cumulative + cu.Unrealized) * cu.Statistic;
                                game = new ImitationGames
                                {
                                    Assets = cu.Assets,
                                    Code = cu.Code,
                                    Commission = cu.Commission,
                                    MarginRate = cu.MarginRate,
                                    Strategy = cu.Strategy,
                                    RollOver = cu.RollOver,
                                    BaseTime = cu.BaseTime,
                                    BaseShort = cu.BaseShort,
                                    BaseLong = cu.BaseLong,
                                    NonaTime = cu.NonaTime,
                                    NonaShort = cu.NonaShort,
                                    NonaLong = cu.NonaLong,
                                    OctaTime = cu.OctaTime,
                                    OctaShort = cu.OctaShort,
                                    OctaLong = cu.OctaLong,
                                    HeptaTime = cu.HeptaTime,
                                    HeptaShort = cu.HeptaShort,
                                    HeptaLong = cu.HeptaLong,
                                    HexaTime = cu.HexaTime,
                                    HexaShort = cu.HexaShort,
                                    HexaLong = cu.HexaLong,
                                    PentaTime = cu.PentaTime,
                                    PentaShort = cu.PentaShort,
                                    PentaLong = cu.PentaLong,
                                    QuadTime = cu.QuadTime,
                                    QuadShort = cu.QuadShort,
                                    QuadLong = cu.QuadLong,
                                    TriTime = cu.TriTime,
                                    TriShort = cu.TriShort,
                                    TriLong = cu.TriLong,
                                    DuoTime = cu.DuoTime,
                                    DuoShort = cu.DuoShort,
                                    DuoLong = cu.DuoLong,
                                    MonoTime = cu.MonoTime,
                                    MonoShort = cu.MonoShort,
                                    MonoLong = cu.MonoLong
                                };
                            }
                        if (string.IsNullOrEmpty(game.Strategy) || remain-- > 0)
                            foreach (var cu in select.OrderByDescending(o => (o.Cumulative + o.Unrealized) * o.Statistic).AsNoTracking().Take(1))
                                if (temp <= (cu.Cumulative + cu.Unrealized) * cu.Statistic)
                                {
                                    temp = (cu.Cumulative + cu.Unrealized) * cu.Statistic;
                                    game = new ImitationGames
                                    {
                                        Assets = cu.Assets,
                                        Code = cu.Code,
                                        Commission = cu.Commission,
                                        MarginRate = cu.MarginRate,
                                        Strategy = cu.Strategy,
                                        RollOver = cu.RollOver,
                                        BaseTime = cu.BaseTime,
                                        BaseShort = cu.BaseShort,
                                        BaseLong = cu.BaseLong,
                                        NonaTime = cu.NonaTime,
                                        NonaShort = cu.NonaShort,
                                        NonaLong = cu.NonaLong,
                                        OctaTime = cu.OctaTime,
                                        OctaShort = cu.OctaShort,
                                        OctaLong = cu.OctaLong,
                                        HeptaTime = cu.HeptaTime,
                                        HeptaShort = cu.HeptaShort,
                                        HeptaLong = cu.HeptaLong,
                                        HexaTime = cu.HexaTime,
                                        HexaShort = cu.HexaShort,
                                        HexaLong = cu.HexaLong,
                                        PentaTime = cu.PentaTime,
                                        PentaShort = cu.PentaShort,
                                        PentaLong = cu.PentaLong,
                                        QuadTime = cu.QuadTime,
                                        QuadShort = cu.QuadShort,
                                        QuadLong = cu.QuadLong,
                                        TriTime = cu.TriTime,
                                        TriShort = cu.TriShort,
                                        TriLong = cu.TriLong,
                                        DuoTime = cu.DuoTime,
                                        DuoShort = cu.DuoShort,
                                        DuoLong = cu.DuoLong,
                                        MonoTime = cu.MonoTime,
                                        MonoShort = cu.MonoShort,
                                        MonoLong = cu.MonoLong
                                    };
                                }
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return game;
        }
        protected List<long> GetUserIdentify(string date)
        {
            var select = new Dictionary<long, long>();
            var choice = new List<long>();
            var identity = new Secret().GetIdentify(key);
            string assets = string.Empty, code = string.Empty, commission = string.Empty, rate = string.Empty, strategy = string.Empty, over = string.Empty;

            try
            {
                using (var db = new GoblinBatDbContext(key))
                {
                    foreach (var str in db.Logs.Where(o => o.Identity.Equals(identity)).OrderByDescending(o => o.Date).Take(1).Select(o => new
                    {
                        o.Assets,
                        o.Code,
                        o.Commission,
                        o.Strategy,
                        o.RollOver
                    }).AsNoTracking())
                    {
                        assets = str.Assets;
                        code = str.Code;
                        commission = str.Commission;
                        rate = marginRate.ToString();
                        strategy = str.Strategy;
                        over = str.RollOver;
                    }
                    if (string.IsNullOrEmpty(assets) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(commission) || string.IsNullOrEmpty(rate) || string.IsNullOrEmpty(strategy) || string.IsNullOrEmpty(over))
                        return null;

                    else
                        foreach (var str in db.Memorize.Where(o => o.Date.Equals(date)).OrderByDescending(o => o.Statistic).Take(25000).Select(o => new
                        {
                            o.Index,
                            o.Cumulative,
                            o.Unrealized
                        }).AsNoTracking())
                            if (long.TryParse(str.Unrealized, out long unrealized) && long.TryParse(str.Cumulative, out long cumulative))
                                select[str.Index] = unrealized + cumulative;
                }
                foreach (var kv in select.OrderByDescending(o => o.Value))
                    using (var db = new GoblinBatDbContext(key))
                    {
                        if (over.Equals("A") && strategy.Equals("Auto") && db.Strategy.Where(o => o.Index == kv.Key && o.Assets.Equals(assets) && o.Code.Equals(code) && o.Commission.Equals(commission) && o.MarginRate.Equals(rate)).AsNoTracking().Any())
                            choice.Add(kv.Key);

                        else if (over.Equals("A") && strategy.Equals("Auto") == false && db.Strategy.Where(o => o.Index == kv.Key && o.Assets.Equals(assets) && o.Code.Equals(code) && o.Commission.Equals(commission) && o.MarginRate.Equals(rate) && o.Strategy.Equals(strategy)).AsNoTracking().Any())
                            choice.Add(kv.Key);

                        else if (strategy.Equals("Auto") && over.Equals("A") == false && db.Strategy.Where(o => o.Index == kv.Key && o.Assets.Equals(assets) && o.Code.Equals(code) && o.Commission.Equals(commission) && o.MarginRate.Equals(rate) && o.RollOver.Equals(over)).AsNoTracking().Any())
                            choice.Add(kv.Key);

                        else if (db.Strategy.Where(o => o.Index == kv.Key && o.Assets.Equals(assets) && o.Code.Equals(code) && o.Commission.Equals(commission) && o.MarginRate.Equals(rate) && o.Strategy.Equals(strategy) && o.RollOver.Equals(over)).AsNoTracking().Any())
                            choice.Add(kv.Key);
                    }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return choice;
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
                            marginRate.ToString(),
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
        protected int SetIndentify(Setting setting)
        {
            string identity = new Secret().GetIdentify(key), date = DateTime.Now.ToString(recent);
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    db.Identifies.AddOrUpdate(new Identify
                    {
                        Identity = identity,
                        Date = date,
                        Assets = setting.Assets,
                        Strategy = setting.Strategy,
                        Commission = setting.Commission,
                        RollOver = setting.RollOver,
                        Code = setting.Code
                    });
                    return db.SaveChanges();
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, setting.Strategy);
                }
            return 0;
        }
        protected void SetDeleteDuplicateData(List<Strategics> strategics)
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

                        db.Strategy.BulkDelete(db.Strategy.Where(o => o.Index > 909770), o => o.BatchSize = 50000);
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
        }
        protected void SetInsertStrategy(List<Strategics> strategy)
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var str = strategy.First();
                    db.Configuration.AutoDetectChangesEnabled = false;

                    if (db.Strategy.Where(o => o.Assets.Equals(str.Assets) && o.Code.Equals(str.Code) && o.Commission.Equals(str.Commission) && o.MarginRate.Equals(str.MarginRate) && o.Strategy.Equals(str.Strategy) && o.RollOver.Equals(str.RollOver) && o.BaseTime.Equals(str.BaseTime) && o.BaseShort.Equals(str.BaseShort) && o.BaseLong.Equals(str.BaseLong) && o.NonaTime.Equals(str.NonaTime) && o.NonaShort.Equals(str.NonaShort) && o.NonaLong.Equals(str.NonaLong) && o.OctaTime.Equals(str.OctaTime) && o.OctaShort.Equals(str.OctaShort) && o.OctaLong.Equals(str.OctaLong) && o.HeptaTime.Equals(str.HeptaTime) && o.HeptaShort.Equals(str.HeptaShort) && o.HeptaLong.Equals(str.HeptaLong) && o.HexaTime.Equals(str.HexaTime) && o.HexaShort.Equals(str.HexaShort) && o.HexaLong.Equals(str.HexaLong) && o.PentaTime.Equals(str.PentaTime) && o.PantaShort.Equals(str.PantaShort) && o.PantaLong.Equals(str.PantaLong) && o.QuadTime.Equals(str.QuadTime) && o.QuadShort.Equals(str.QuadShort) && o.QuadLong.Equals(str.QuadLong) && o.TriTime.Equals(str.TriTime) && o.TriShort.Equals(str.TriShort) && o.TriLong.Equals(str.TriLong) && o.DuoTime.Equals(str.DuoTime) && o.DuoShort.Equals(str.DuoShort) && o.DuoLong.Equals(str.DuoLong) && o.MonoTime.Equals(str.MonoTime) && o.MonoShort.Equals(str.MonoShort) && o.MonoLong.Equals(str.MonoLong)).Any() == false)
                        db.BulkInsert(strategy, o =>
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
                            o.BatchSize = 150000;
                            o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                            o.AutoMapOutputDirection = false;
                        });
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, strategy.GetType().Name);
                }
                finally
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
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
        protected CallUpGoblinBat(string key) : base(key)
        {
            this.key = key;
            ran = new Random();
        }
        protected internal const string futures = "000";
        protected internal const string kospi200f = "101";
        const double marginRate = 0.162;
        const string basic = "Base.res";
        const string chart = "ChartOf101000";
        const int ar = 10000000;
        const int cr = 1000000;
        const string recent = "yyMMdd";
        readonly string key;
        readonly Random ran;
    }
}