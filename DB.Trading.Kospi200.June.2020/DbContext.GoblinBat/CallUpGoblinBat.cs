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
        protected string[] GetClosestDueDate()
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var temp = new string[2];
                    var count = 0;

                    foreach (var codes in db.Codes.Where(o => o.Code.Contains(kospi200f) && o.Code.Contains(futures) && o.Code.Length == 8).AsNoTracking().OrderByDescending(o => o.Info))
                    {
                        if (count == 0)
                        {
                            temp[count++] = codes.Code;

                            continue;
                        }
                        if (count == 1)
                        {
                            temp[count++] = codes.Info.Substring(2);

                            continue;
                        }
                        if (count > 1)
                            return temp;
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return null;
        }
        protected IEnumerable<Simulations> GetStrategy(List<Simulations> strategy, string code)
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    foreach (var st in db.Statistics.Select(o => new
                    {
                        o.Assets,
                        o.Code,
                        o.Commission,
                        o.Strategy,
                        o.RollOver
                    }).AsNoTracking())
                        if (st.Strategy.Length > 2 && st.Code.Equals(code))
                            strategy.Add(new Simulations
                            {
                                Assets = st.Assets,
                                Code = st.Code,
                                Commission = st.Commission,
                                MarginRate = marginRate,
                                Strategy = st.Strategy,
                                RollOver = st.RollOver
                            });
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return strategy.Distinct();
        }
        protected string[] GetRemainingDay(string code)
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var array = db.Codes.Where(o => o.Code.Length == 8 && o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(5))).AsNoTracking().Select(o => new { o.Info }).ToArray();
                    var days = new string[array.Length + 2];
                    var count = 0;

                    foreach (var remaining in array)
                        if (int.TryParse(remaining.Info.Substring(2), out int day))
                            days[++count] = (day - 1).ToString();

                    days[0] = "191211";
                    days[days.Length - 1] = "190910";

                    return days;
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return null;
        }
        protected List<Statistics> GetBasicStrategy(char initial, string code)
        {
            var list = new List<Statistics>();
            var identity = new Secret().GetIdentify(key);
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    switch (initial)
                    {
                        case (char)67:
                            return db.Statistics.Where(o => o.Code.Equals(code)).AsNoTracking().Distinct().ToList();

                        case (char)83:
                            var select = db.Identifies.Where(o => o.Code.Equals(code) && o.Identity.Equals(identity)).AsNoTracking().OrderByDescending(o => o.Date).First();

                            return new List<Statistics>()
                            {
                                new Statistics
                                {
                                    Assets = select.Assets,
                                    Code = select.Code,
                                    Commission = select.Commission,
                                    MarginRate = marginRate,
                                    Strategy = select.Strategy.Length == 2 ? select.Strategy : string.Empty,
                                    RollOver = !select.RollOver.Equals(CheckState.Unchecked)
                                }
                            };
                        default:
                            var choice = new bool[] { true, false };
                            var strategy = db.Statistics.Select(o => new { o.Strategy }).AsNoTracking().Distinct().ToArray();

                            foreach (var str in db.Identifies.Where(o => o.Code.Equals(code) && o.Identity.Equals(identity)).OrderByDescending(o => o.Date).AsNoTracking().Take(1).Select(o => new
                            {
                                o.Assets,
                                o.Code,
                                o.Commission,
                                o.Strategy,
                                o.RollOver
                            }))
                                if (str.Strategy.Length > 2)
                                {
                                    if (str.Strategy.Equals("Auto") == false && str.RollOver.Equals(CheckState.Indeterminate) == false)
                                        list.Add(new Statistics
                                        {
                                            Assets = str.Assets,
                                            Code = str.Code,
                                            Commission = str.Commission,
                                            MarginRate = marginRate,
                                            Strategy = str.Strategy,
                                            RollOver = str.RollOver.Equals(CheckState.Checked)
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
                                                RollOver = str.RollOver.Equals(CheckState.Checked)
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
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (TimerBox.Show(new Secret().Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 3517).Equals(DialogResult.OK))
                        return db.Statistics.Distinct().ToList();
                }
            return list.Distinct().ToList();
        }
        protected IEnumerable<long> GetUserAssets(List<long> list)
        {
            var now = DateTime.Now;
            string today = now.ToString(recent), yesterday = now.AddDays(-1).ToString(recent);
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    foreach (var assets in db.Identifies.Where(o => o.Date.Equals(today) || o.Date.Equals(yesterday)).AsNoTracking())
                        list.Add(assets.Assets);
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return list.Distinct();
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
        protected Simulations Preheat(Simulations imitation)
        {
            var index = new Secret().GetIdentityIndex(key);

            if (index > 0)
            {
                string max = MostRecentDate;
                using (var db = new GoblinBatDbContext(key))
                    try
                    {
                        var identity = new Secret().GetIdentify(key);
                        var identify = db.Identifies.Where(o => o.Identity.Equals(identity)).OrderByDescending(o => o.Date).AsNoTracking().First();
                        var strategy = identify.Strategy;
                        var over = identify.RollOver.Equals(CheckState.Checked);

                        foreach (var game in db.Virtual.Where(o => o.Strategy.Equals(strategy) && o.Date.Equals(max) && o.RollOver.Equals(over) && o.Cumulative > 0 && o.Statistic > 0 && o.MarginRate == marginRate).AsNoTracking().OrderByDescending(o => o.Statistic).Take(index))
                            imitation = new Simulations
                            {
                                Assets = game.Assets,
                                Code = game.Code,
                                Commission = game.Commission,
                                MarginRate = game.MarginRate,
                                RollOver = game.RollOver,
                                BaseTime = game.BaseTime,
                                BaseShort = game.BaseShort,
                                BaseLong = game.BaseLong,
                                NonaTime = game.NonaTime,
                                NonaShort = game.NonaShort,
                                NonaLong = game.NonaLong,
                                OctaTime = game.OctaTime,
                                OctaShort = game.OctaShort,
                                OctaLong = game.OctaLong,
                                HeptaTime = game.HeptaTime,
                                HeptaShort = game.HeptaShort,
                                HeptaLong = game.HeptaLong,
                                HexaTime = game.HexaTime,
                                HexaShort = game.HexaShort,
                                HexaLong = game.HexaLong,
                                PentaTime = game.PentaTime,
                                PentaShort = game.PentaShort,
                                PentaLong = game.PentaLong,
                                QuadTime = game.QuadTime,
                                QuadShort = game.QuadShort,
                                QuadLong = game.QuadLong,
                                TriTime = game.TriTime,
                                TriShort = game.TriShort,
                                TriLong = game.TriLong,
                                DuoTime = game.DuoTime,
                                DuoShort = game.DuoShort,
                                DuoLong = game.DuoLong,
                                MonoTime = game.MonoTime,
                                MonoShort = game.MonoShort,
                                MonoLong = game.MonoLong
                            };
                        return imitation;
                    }
                    catch (Exception ex)
                    {
                        new ExceptionMessage(ex.StackTrace);
                    }
            }
            return null;
        }
        protected IEnumerable<Simulations> Preheat(List<Simulations> games, string code)
        {
            string max = MostRecentDate;
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    foreach (var game in db.Virtual.Where(o => o.Strategy.Length > 2 && o.RollOver.Equals(false) && o.Date.Equals(max) && o.MarginRate == marginRate && o.Statistic > 0 && o.Cumulative > 0).AsNoTracking().OrderBy(o => o.Statistic))
                        games.Add(new Simulations
                        {
                            Assets = game.Assets,
                            Code = code,
                            Commission = game.Commission,
                            MarginRate = game.MarginRate,
                            Strategy = game.Strategy,
                            RollOver = game.RollOver,
                            BaseTime = game.BaseTime,
                            BaseShort = game.BaseShort,
                            BaseLong = game.BaseLong,
                            NonaTime = game.NonaTime,
                            NonaShort = game.NonaShort,
                            NonaLong = game.NonaLong,
                            OctaTime = game.OctaTime,
                            OctaShort = game.OctaShort,
                            OctaLong = game.OctaLong,
                            HeptaTime = game.HeptaTime,
                            HeptaShort = game.HeptaShort,
                            HeptaLong = game.HeptaLong,
                            HexaTime = game.HexaTime,
                            HexaShort = game.HexaShort,
                            HexaLong = game.HexaLong,
                            PentaTime = game.PentaTime,
                            PentaShort = game.PentaShort,
                            PentaLong = game.PentaLong,
                            QuadTime = game.QuadTime,
                            QuadShort = game.QuadShort,
                            QuadLong = game.QuadLong,
                            TriTime = game.TriTime,
                            TriShort = game.TriShort,
                            TriLong = game.TriLong,
                            DuoTime = game.DuoTime,
                            DuoShort = game.DuoShort,
                            DuoLong = game.DuoLong,
                            MonoTime = game.MonoTime,
                            MonoShort = game.MonoShort,
                            MonoLong = game.MonoLong
                        });
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return games;
        }
        protected Stack<Simulations> GetBestStrategy(Stack<Simulations> stack, IEnumerable<long> list, string code)
        {
            string max = MostRecentDate;
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    foreach (var assets in list)
                        foreach (var game in db.Virtual.Where(o => o.MonoTime == 1 && o.Assets == assets && o.Strategy.Length == 2 && o.Date.Equals(max) && o.MarginRate == marginRate && o.Statistic > 0 && o.Cumulative > 0).AsNoTracking().OrderBy(o => o.Statistic))
                            stack.Push(new Simulations
                            {
                                Assets = game.Assets,
                                Code = code,
                                Commission = game.Commission,
                                MarginRate = marginRate,
                                Strategy = game.Strategy,
                                RollOver = game.RollOver,
                                BaseTime = game.BaseTime,
                                BaseShort = game.BaseShort,
                                BaseLong = game.BaseLong,
                                NonaTime = game.NonaTime,
                                NonaShort = game.NonaShort,
                                NonaLong = game.NonaLong,
                                OctaTime = game.OctaTime,
                                OctaShort = game.OctaShort,
                                OctaLong = game.OctaLong,
                                HeptaTime = game.HeptaTime,
                                HeptaShort = game.HeptaShort,
                                HeptaLong = game.HeptaLong,
                                HexaTime = game.HexaTime,
                                HexaShort = game.HexaShort,
                                HexaLong = game.HexaLong,
                                PentaTime = game.PentaTime,
                                PentaShort = game.PentaShort,
                                PentaLong = game.PentaLong,
                                QuadTime = game.QuadTime,
                                QuadShort = game.QuadShort,
                                QuadLong = game.QuadLong,
                                TriTime = game.TriTime,
                                TriShort = game.TriShort,
                                TriLong = game.TriLong,
                                DuoTime = game.DuoTime,
                                DuoShort = game.DuoShort,
                                DuoLong = game.DuoLong,
                                MonoTime = game.MonoTime,
                                MonoShort = game.MonoShort,
                                MonoLong = game.MonoLong
                            });
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return stack;
        }
        protected List<Simulations> GetBestExternalRecommend(List<Simulations> games)
        {
            try
            {
                string date;
                using (var db = new GoblinBatDbContext(key))
                    date = db.Virtual.Max(o => o.Date);

                using (var db = new GoblinBatDbContext(key))
                    foreach (var game in db.Virtual.Where(o => o.Date.Equals(date) && o.MarginRate == marginRate).OrderByDescending(o => o.Cumulative).AsNoTracking().Take(3750))
                        games.Add(new Simulations
                        {
                            Assets = game.Assets,
                            Code = game.Code,
                            Commission = game.Commission,
                            MarginRate = game.MarginRate,
                            Strategy = game.Strategy,
                            RollOver = game.RollOver,
                            BaseTime = game.BaseTime,
                            BaseShort = game.BaseShort,
                            BaseLong = game.BaseLong,
                            NonaTime = game.NonaTime,
                            NonaShort = game.NonaShort,
                            NonaLong = game.NonaLong,
                            OctaTime = game.OctaTime,
                            OctaShort = game.OctaShort,
                            OctaLong = game.OctaLong,
                            HeptaTime = game.HeptaTime,
                            HeptaShort = game.HeptaShort,
                            HeptaLong = game.HeptaLong,
                            HexaTime = game.HexaTime,
                            HexaShort = game.HexaShort,
                            HexaLong = game.HexaLong,
                            PentaTime = game.PentaTime,
                            PentaShort = game.PentaShort,
                            PentaLong = game.PentaLong,
                            QuadTime = game.QuadTime,
                            QuadShort = game.QuadShort,
                            QuadLong = game.QuadLong,
                            TriTime = game.TriTime,
                            TriShort = game.TriShort,
                            TriLong = game.TriLong,
                            DuoTime = game.DuoTime,
                            DuoShort = game.DuoShort,
                            DuoLong = game.DuoLong,
                            MonoTime = game.MonoTime,
                            MonoShort = game.MonoShort,
                            MonoLong = game.MonoLong
                        });
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return games;
        }
        protected List<Simulations> GetBestStrategyRecommend(List<Simulations> games)
        {
            try
            {
                string date;
                using (var db = new GoblinBatDbContext(key))
                    date = db.Virtual.Max(o => o.Date);

                using (var db = new GoblinBatDbContext(key))
                    foreach (var game in db.Virtual.Where(o => o.MarginRate == marginRate && o.Date.Equals(date)).OrderByDescending(o => o.Statistic).AsNoTracking().Take(3750))
                        games.Add(new Simulations
                        {
                            Assets = game.Assets,
                            Code = game.Code,
                            Commission = game.Commission,
                            MarginRate = game.MarginRate,
                            Strategy = game.Strategy,
                            RollOver = game.RollOver,
                            BaseTime = game.BaseTime,
                            BaseShort = game.BaseShort,
                            BaseLong = game.BaseLong,
                            NonaTime = game.NonaTime,
                            NonaShort = game.NonaShort,
                            NonaLong = game.NonaLong,
                            OctaTime = game.OctaTime,
                            OctaShort = game.OctaShort,
                            OctaLong = game.OctaLong,
                            HeptaTime = game.HeptaTime,
                            HeptaShort = game.HeptaShort,
                            HeptaLong = game.HeptaLong,
                            HexaTime = game.HexaTime,
                            HexaShort = game.HexaShort,
                            HexaLong = game.HexaLong,
                            PentaTime = game.PentaTime,
                            PentaShort = game.PentaShort,
                            PentaLong = game.PentaLong,
                            QuadTime = game.QuadTime,
                            QuadShort = game.QuadShort,
                            QuadLong = game.QuadLong,
                            TriTime = game.TriTime,
                            TriShort = game.TriShort,
                            TriLong = game.TriLong,
                            DuoTime = game.DuoTime,
                            DuoShort = game.DuoShort,
                            DuoLong = game.DuoLong,
                            MonoTime = game.MonoTime,
                            MonoShort = game.MonoShort,
                            MonoLong = game.MonoLong
                        });
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return games;
        }
        protected Simulations GetMyStrategy()
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var identify = new Secret().GetIdentify(key);
                    var identity = db.Identifies.Any(o => o.Identity.Equals(identify)) ? db.Identifies.Where(o => o.Identity.Equals(identify)).AsNoTracking().OrderByDescending(o => o.Date).Select(o => new
                    {
                        o.Assets,
                        o.Code,
                        o.Commission,
                        o.Strategy,
                        o.RollOver,
                        o.BaseShort,
                        o.BaseLong,
                        o.NonaTime,
                        o.NonaShort,
                        o.NonaLong,
                        o.OctaTime,
                        o.OctaShort,
                        o.OctaLong,
                        o.HeptaTime,
                        o.HeptaShort,
                        o.HeptaLong,
                        o.HexaTime,
                        o.HexaShort,
                        o.HexaLong,
                        o.PentaTime,
                        o.PentaShort,
                        o.PentaLong,
                        o.QuadTime,
                        o.QuadShort,
                        o.QuadLong,
                        o.TriTime,
                        o.TriShort,
                        o.TriLong,
                        o.DuoTime,
                        o.DuoShort,
                        o.DuoLong,
                        o.MonoTime,
                        o.MonoShort,
                        o.MonoLong
                    }).Take(1).First() : null;

                    if (identity != null)
                        return new Simulations
                        {
                            Assets = identity.Assets,
                            Code = identity.Code,
                            Commission = identity.Commission,
                            Strategy = identity.Strategy,
                            MarginRate = marginRate,
                            RollOver = identity.RollOver.Equals(CheckState.Checked),
                            BaseTime = 1440,
                            BaseShort = identity.BaseShort,
                            BaseLong = identity.BaseLong,
                            NonaTime = identity.NonaTime,
                            NonaShort = identity.NonaShort,
                            NonaLong = identity.NonaLong,
                            OctaTime = identity.OctaTime,
                            OctaShort = identity.OctaShort,
                            OctaLong = identity.OctaLong,
                            HeptaTime = identity.HeptaTime,
                            HeptaShort = identity.HeptaShort,
                            HeptaLong = identity.HeptaLong,
                            HexaTime = identity.HexaTime,
                            HexaShort = identity.HexaShort,
                            HexaLong = identity.HexaLong,
                            PentaTime = identity.PentaTime,
                            PentaShort = identity.PentaShort,
                            PentaLong = identity.PentaLong,
                            QuadTime = identity.QuadTime,
                            QuadShort = identity.QuadShort,
                            QuadLong = identity.QuadLong,
                            TriTime = identity.TriTime,
                            TriShort = identity.TriShort,
                            TriLong = identity.TriLong,
                            DuoTime = identity.DuoTime,
                            DuoShort = identity.DuoShort,
                            DuoLong = identity.DuoLong,
                            MonoTime = identity.MonoTime,
                            MonoShort = identity.MonoShort,
                            MonoLong = identity.MonoLong
                        };
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return null;
        }
        protected (int, Simulations, uint, int, Simulations) GetBestStrategyRecommend(List<Statistics> list, Simulations game)
        {
            uint count = 1;
            int temp = int.MinValue, eTemp = int.MinValue;
            var exist = new Simulations();
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var max = db.Virtual.Max(o => o.Date);
                    var identify = new Secret().GetIdentify(key);
                    var identity = db.Identifies.Any(o => o.Identity.Equals(identify)) ? db.Identifies.Where(o => o.Identity.Equals(identify)).AsNoTracking().OrderByDescending(o => o.Date).Select(o => new
                    {
                        o.Assets,
                        o.Code,
                        o.Commission,
                        o.Strategy,
                        o.BaseShort,
                        o.BaseLong,
                        o.NonaTime,
                        o.NonaShort,
                        o.NonaLong,
                        o.OctaTime,
                        o.OctaShort,
                        o.OctaLong,
                        o.HeptaTime,
                        o.HeptaShort,
                        o.HeptaLong,
                        o.HexaTime,
                        o.HexaShort,
                        o.HexaLong,
                        o.PentaTime,
                        o.PentaShort,
                        o.PentaLong,
                        o.QuadTime,
                        o.QuadShort,
                        o.QuadLong,
                        o.TriTime,
                        o.TriShort,
                        o.TriLong,
                        o.DuoTime,
                        o.DuoShort,
                        o.DuoLong,
                        o.MonoTime,
                        o.MonoShort,
                        o.MonoLong
                    }).Take(1).First() : null;
                    if (list.Count == 1 && list.First().Strategy.Length == 2)
                        foreach (var select in db.Virtual.Where(o => o.Cumulative > 0 && o.Statistic > 0 && o.Strategy.Length == 2 && o.Date.Equals(max) && o.Assets == identity.Assets && o.Code.Equals(identity.Code) && o.Commission == identity.Commission && o.MarginRate == marginRate).AsNoTracking().OrderByDescending(o => o.Statistic))
                        {
                            if (temp < select.Statistic)
                            {
                                game = new Simulations
                                {
                                    Assets = select.Assets,
                                    Code = select.Code,
                                    Commission = select.Commission,
                                    MarginRate = select.MarginRate,
                                    Strategy = select.Strategy,
                                    RollOver = select.RollOver,
                                    BaseTime = select.BaseTime,
                                    BaseShort = select.BaseShort,
                                    BaseLong = select.BaseLong,
                                    NonaTime = select.NonaTime,
                                    NonaShort = select.NonaShort,
                                    NonaLong = select.NonaLong,
                                    OctaTime = select.OctaTime,
                                    OctaShort = select.OctaShort,
                                    OctaLong = select.OctaLong,
                                    HeptaTime = select.HeptaTime,
                                    HeptaShort = select.HeptaShort,
                                    HeptaLong = select.HeptaLong,
                                    HexaTime = select.HexaTime,
                                    HexaShort = select.HexaShort,
                                    HexaLong = select.HexaLong,
                                    PentaTime = select.PentaTime,
                                    PentaShort = select.PentaShort,
                                    PentaLong = select.PentaLong,
                                    QuadTime = select.QuadTime,
                                    QuadShort = select.QuadShort,
                                    QuadLong = select.QuadLong,
                                    TriTime = select.TriTime,
                                    TriShort = select.TriShort,
                                    TriLong = select.TriLong,
                                    DuoTime = select.DuoTime,
                                    DuoShort = select.DuoShort,
                                    DuoLong = select.DuoLong,
                                    MonoTime = select.MonoTime,
                                    MonoShort = select.MonoShort,
                                    MonoLong = select.MonoLong
                                };
                                temp = select.Statistic;
                            }
                            if (identity != null && identity.Strategy.Length == 2 && select.Assets == identity.Assets && select.Code.Equals(identity.Code) && select.Strategy.Length == identity.Strategy.Length && select.Commission == identity.Commission && select.MarginRate == marginRate && select.BaseShort == identity.BaseShort && select.BaseLong == identity.BaseLong && select.NonaTime == identity.NonaTime && select.NonaShort == identity.NonaShort && select.NonaLong == identity.NonaLong && select.OctaTime == identity.OctaTime && select.OctaShort == identity.OctaShort && select.OctaLong == identity.OctaLong && select.HeptaTime == identity.HeptaTime && select.HeptaShort == identity.HeptaShort && select.HeptaLong == identity.HeptaLong && select.HexaTime == identity.HexaTime && select.HexaShort == identity.HexaShort && select.HexaLong == identity.HexaLong && select.PentaTime == identity.PentaTime && select.PentaShort == identity.PentaShort && select.PentaLong == identity.PentaLong && select.QuadTime == identity.QuadTime && select.QuadShort == identity.QuadShort && select.QuadLong == identity.QuadLong && select.TriTime == identity.TriTime && select.TriShort == identity.TriShort && select.TriLong == identity.TriLong && select.DuoTime == identity.DuoTime && select.DuoShort == identity.DuoShort && select.DuoLong == identity.DuoLong && select.MonoTime == identity.MonoTime && select.MonoShort == identity.MonoShort && select.MonoLong == identity.MonoLong)
                            {
                                exist = new Simulations
                                {
                                    Assets = select.Assets,
                                    Code = select.Code,
                                    Commission = select.Commission,
                                    MarginRate = select.MarginRate,
                                    Strategy = select.Strategy,
                                    RollOver = select.RollOver,
                                    BaseTime = select.BaseTime,
                                    BaseShort = select.BaseShort,
                                    BaseLong = select.BaseLong,
                                    NonaTime = select.NonaTime,
                                    NonaShort = select.NonaShort,
                                    NonaLong = select.NonaLong,
                                    OctaTime = select.OctaTime,
                                    OctaShort = select.OctaShort,
                                    OctaLong = select.OctaLong,
                                    HeptaTime = select.HeptaTime,
                                    HeptaShort = select.HeptaShort,
                                    HeptaLong = select.HeptaLong,
                                    HexaTime = select.HexaTime,
                                    HexaShort = select.HexaShort,
                                    HexaLong = select.HexaLong,
                                    PentaTime = select.PentaTime,
                                    PentaShort = select.PentaShort,
                                    PentaLong = select.PentaLong,
                                    QuadTime = select.QuadTime,
                                    QuadShort = select.QuadShort,
                                    QuadLong = select.QuadLong,
                                    TriTime = select.TriTime,
                                    TriShort = select.TriShort,
                                    TriLong = select.TriLong,
                                    DuoTime = select.DuoTime,
                                    DuoShort = select.DuoShort,
                                    DuoLong = select.DuoLong,
                                    MonoTime = select.MonoTime,
                                    MonoShort = select.MonoShort,
                                    MonoLong = select.MonoLong
                                };
                                eTemp = select.Statistic;
                                break;
                            }
                            count++;
                        }
                    else
                        foreach (var ch in list)
                        {
                            count = 1;

                            if (ch.Strategy.Length > 2)
                                foreach (var choice in db.Virtual.Where(o => o.Date.Equals(max) && o.Assets == identity.Assets && o.Code.Equals(identity.Code) && o.Commission == identity.Commission && o.MarginRate == marginRate && o.Strategy.Length > 2 && o.Cumulative > 0 && o.Statistic > 0).AsNoTracking().OrderByDescending(o => o.Statistic))
                                {
                                    if (temp < choice.Statistic && choice.Revenue < 0)
                                    {
                                        game = new Simulations
                                        {
                                            Assets = choice.Assets,
                                            Code = choice.Code,
                                            Commission = choice.Commission,
                                            MarginRate = choice.MarginRate,
                                            Strategy = choice.Strategy,
                                            RollOver = choice.RollOver,
                                            BaseTime = choice.BaseTime,
                                            BaseShort = choice.BaseShort,
                                            BaseLong = choice.BaseLong,
                                            NonaTime = choice.NonaTime,
                                            NonaShort = choice.NonaShort,
                                            NonaLong = choice.NonaLong,
                                            OctaTime = choice.OctaTime,
                                            OctaShort = choice.OctaShort,
                                            OctaLong = choice.OctaLong,
                                            HeptaTime = choice.HeptaTime,
                                            HeptaShort = choice.HeptaShort,
                                            HeptaLong = choice.HeptaLong,
                                            HexaTime = choice.HexaTime,
                                            HexaShort = choice.HexaShort,
                                            HexaLong = choice.HexaLong,
                                            PentaTime = choice.PentaTime,
                                            PentaShort = choice.PentaShort,
                                            PentaLong = choice.PentaLong,
                                            QuadTime = choice.QuadTime,
                                            QuadShort = choice.QuadShort,
                                            QuadLong = choice.QuadLong,
                                            TriTime = choice.TriTime,
                                            TriShort = choice.TriShort,
                                            TriLong = choice.TriLong,
                                            DuoTime = choice.DuoTime,
                                            DuoShort = choice.DuoShort,
                                            DuoLong = choice.DuoLong,
                                            MonoTime = choice.MonoTime,
                                            MonoShort = choice.MonoShort,
                                            MonoLong = choice.MonoLong
                                        };
                                        temp = choice.Statistic;
                                    }
                                    if (identity != null && identity.Strategy.Length > 2 && choice.Assets == identity.Assets && choice.Code.Equals(identity.Code) && choice.Strategy.Length == identity.Strategy.Length && choice.Commission == identity.Commission && choice.MarginRate == marginRate && choice.BaseShort == identity.BaseShort && choice.BaseLong == identity.BaseLong && choice.NonaTime == identity.NonaTime && choice.NonaShort == identity.NonaShort && choice.NonaLong == identity.NonaLong && choice.OctaTime == identity.OctaTime && choice.OctaShort == identity.OctaShort && choice.OctaLong == identity.OctaLong && choice.HeptaTime == identity.HeptaTime && choice.HeptaShort == identity.HeptaShort && choice.HeptaLong == identity.HeptaLong && choice.HexaTime == identity.HexaTime && choice.HexaShort == identity.HexaShort && choice.HexaLong == identity.HexaLong && choice.PentaTime == identity.PentaTime && choice.PentaShort == identity.PentaShort && choice.PentaLong == identity.PentaLong && choice.QuadTime == identity.QuadTime && choice.QuadShort == identity.QuadShort && choice.QuadLong == identity.QuadLong && choice.TriTime == identity.TriTime && choice.TriShort == identity.TriShort && choice.TriLong == identity.TriLong && choice.DuoTime == identity.DuoTime && choice.DuoShort == identity.DuoShort && choice.DuoLong == identity.DuoLong && choice.MonoTime == identity.MonoTime && choice.MonoShort == identity.MonoShort && choice.MonoLong == identity.MonoLong)
                                    {
                                        exist = new Simulations
                                        {
                                            Assets = choice.Assets,
                                            Code = choice.Code,
                                            Commission = choice.Commission,
                                            MarginRate = choice.MarginRate,
                                            Strategy = choice.Strategy,
                                            RollOver = choice.RollOver,
                                            BaseTime = choice.BaseTime,
                                            BaseShort = choice.BaseShort,
                                            BaseLong = choice.BaseLong,
                                            NonaTime = choice.NonaTime,
                                            NonaShort = choice.NonaShort,
                                            NonaLong = choice.NonaLong,
                                            OctaTime = choice.OctaTime,
                                            OctaShort = choice.OctaShort,
                                            OctaLong = choice.OctaLong,
                                            HeptaTime = choice.HeptaTime,
                                            HeptaShort = choice.HeptaShort,
                                            HeptaLong = choice.HeptaLong,
                                            HexaTime = choice.HexaTime,
                                            HexaShort = choice.HexaShort,
                                            HexaLong = choice.HexaLong,
                                            PentaTime = choice.PentaTime,
                                            PentaShort = choice.PentaShort,
                                            PentaLong = choice.PentaLong,
                                            QuadTime = choice.QuadTime,
                                            QuadShort = choice.QuadShort,
                                            QuadLong = choice.QuadLong,
                                            TriTime = choice.TriTime,
                                            TriShort = choice.TriShort,
                                            TriLong = choice.TriLong,
                                            DuoTime = choice.DuoTime,
                                            DuoShort = choice.DuoShort,
                                            DuoLong = choice.DuoLong,
                                            MonoTime = choice.MonoTime,
                                            MonoShort = choice.MonoShort,
                                            MonoLong = choice.MonoLong
                                        };
                                        eTemp = choice.Statistic;
                                        break;
                                    }
                                    count++;
                                }
                        }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return (temp, game, count, eTemp == int.MinValue ? 0 : eTemp, exist ?? null);
        }
        protected Simulations GetBestStrategyRecommend(List<Statistics> list)
        {
            var game = new Simulations();
            var temp = double.MinValue;
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var date = db.Virtual.Max(o => o.Date);

                    foreach (var choice in list)
                    {
                        var select = db.Virtual.Where(o => o.Strategy.Length > 2 && o.Assets == choice.Assets && o.Code.Equals(choice.Code) && o.Commission == choice.Commission && o.MarginRate == choice.MarginRate && o.Strategy.Equals(choice.Strategy) && o.RollOver.Equals(choice.RollOver) && o.Cumulative > 0 && o.Statistic > 0).AsNoTracking();

                        foreach (var cu in select.Where(o => o.Date.Equals(date)).OrderByDescending(o => o.Statistic * (o.Unrealized + o.Cumulative)).AsNoTracking().Take(25))
                        {
                            uint count = 0;
                            double before = 0, avg = 0;

                            foreach (var en in select.Where(ea => ea.BaseTime == cu.BaseTime && ea.BaseShort == cu.BaseShort && ea.BaseLong == cu.BaseLong && ea.NonaTime == cu.NonaTime && ea.NonaShort == cu.NonaShort && ea.NonaLong == cu.NonaLong && ea.OctaTime == cu.OctaTime && ea.OctaShort == cu.OctaShort && ea.OctaLong == cu.OctaLong && ea.HeptaTime == cu.HeptaTime && ea.HeptaShort == cu.HeptaShort && ea.HeptaLong == cu.HeptaLong && ea.HexaTime == cu.HexaTime && ea.HexaShort == cu.HexaShort && ea.HexaLong == cu.HexaLong && ea.PentaTime == cu.PentaTime && ea.PentaShort == cu.PentaShort && ea.PentaLong == cu.PentaLong && ea.QuadTime == cu.QuadTime && ea.QuadShort == cu.QuadShort && ea.QuadLong == cu.QuadLong && ea.TriTime == cu.TriTime && ea.TriShort == cu.TriShort && ea.TriLong == cu.TriLong && ea.DuoTime == cu.DuoTime && ea.DuoShort == cu.DuoShort && ea.DuoLong == cu.DuoLong && ea.MonoTime == cu.MonoTime && ea.MonoShort == cu.MonoShort && ea.MonoLong == cu.MonoLong).OrderBy(ea => ea.Date).AsNoTracking().Select(ea => new
                            {
                                ea.Unrealized,
                                ea.Revenue
                            }))
                            {
                                if (count++ > 0)
                                {
                                    var k = 2 / (double)(1 + count);
                                    avg = (en.Unrealized + en.Revenue) * k + before * (1 - k);
                                }
                                else
                                    avg = en.Unrealized + en.Revenue;

                                before = avg;
                            }
                            if (temp < avg)
                            {
                                temp = avg;
                                game = new Simulations
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
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return game;
        }
        protected int SetIdentify(Identify setting)
        {
            setting.Identity = new Secret().GetIdentify(key);
            setting.Date = DateTime.Now.ToString(recent);
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    db.Identifies.AddOrUpdate(setting);

                    return db.SaveChanges();
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, setting.Strategy);
                }
            return 0;
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
        protected Dictionary<DateTime, Queue<Chart>> GetChart(Dictionary<DateTime, Queue<Chart>> catalog, string code)
        {
            var path = System.IO.Path.Combine(Application.StartupPath, charts);
            var exists = new DirectoryInfo(path);
            var chart = new Queue<Chart>(2048);

            try
            {
                if (GetDirectoryExists(exists))
                {
                    foreach (var file in Directory.GetFiles(path, "*.res", SearchOption.AllDirectories))
                    {
                        long info = long.MaxValue;
                        using (var sr = new StreamReader(file))
                            if (sr != null)
                                while (sr.EndOfStream == false)
                                {
                                    var str = sr.ReadLine().Split(',');

                                    if (long.TryParse(str[0], out long time) && double.TryParse(str[1], out double price) && int.TryParse(str[2], out int volume))
                                    {
                                        chart.Enqueue(new Chart
                                        {
                                            Date = time,
                                            Price = price,
                                            Volume = volume
                                        });
                                        info = time;
                                    }
                                }
                        var temp = file.Split('\\');
                        var search = temp[temp.Length - 1].Split('.')[0];
                        var storage = new Stack<Chart>();
                        using (var db = new GoblinBatDbContext(key))
                        {
                            var check = db.Codes.Where(o => o.Code.Length == 8 && o.Code.Equals(search)).First().Info;
                            var recent = db.Futures.Where(o => o.Date > info && o.Code.Length == 8 && o.Code.Equals(search)).AsNoTracking();

                            if (recent.Any() && check.Substring(2).CompareTo(info.ToString().Substring(0, 6)) > 0 && recent.Max(o => o.Date).ToString().Substring(6, 4).Equals(remaining))
                                foreach (var find in recent.OrderByDescending(o => o.Date).Select(o => new { o.Date, o.Price, o.Volume }))
                                    storage.Push(new Chart
                                    {
                                        Date = find.Date,
                                        Price = find.Price,
                                        Volume = find.Volume
                                    });
                        }
                        using (var sw = new StreamWriter(file, true))
                            while (storage.Count > 0)
                            {
                                var str = storage.Pop();
                                sw.WriteLine(string.Concat(str.Date, ',', str.Price, ',', str.Volume));
                                chart.Enqueue(new Chart
                                {
                                    Date = str.Date,
                                    Price = str.Price,
                                    Volume = str.Volume
                                });
                            }
                        GC.Collect();
                        catalog[new FileInfo(file).CreationTime] = new Queue<Chart>(chart);
                        chart.Clear();
                    }
                    if (GetClosestDueDate()[1].Equals(DateTime.Now.ToString(recent)) && new FileInfo(string.Concat(path, @"\", code, res)).Exists == false)
                    {
                        var storage = new Stack<Chart>();
                        var today = long.TryParse(DateTime.Now.ToString(recent), out long date) ? date * 1000000000 : long.MinValue;
                        using (var db = new GoblinBatDbContext(key))
                            foreach (var find in db.Futures.Where(o => o.Code.Equals(code)).AsNoTracking().OrderByDescending(o => o.Date))
                            {
                                if (find.Date < today)
                                    break;

                                storage.Push(new Chart
                                {
                                    Date = find.Date,
                                    Price = find.Price,
                                    Volume = find.Volume
                                });
                            }
                        var file = string.Concat(path, @"\", code, res);
                        using (var sw = new StreamWriter(file, true))
                            while (storage.Count > 0)
                            {
                                var str = storage.Pop();
                                sw.WriteLine(string.Concat(str.Date, ',', str.Price, ',', str.Volume));
                                chart.Enqueue(new Chart
                                {
                                    Date = str.Date,
                                    Price = str.Price,
                                    Volume = str.Volume
                                });
                            }
                        GC.Collect();
                        catalog[new FileInfo(file).CreationTime] = new Queue<Chart>(chart);
                        chart.Clear();
                    }
                }
                else
                {
                    exists.Create();
                    string info;
                    using (var db = new GoblinBatDbContext(key))
                    {
                        var days = db.Days.Where(o => o.Code.Length == 8 && o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(5))).AsNoTracking();

                        foreach (var temp in days.OrderBy(o => o.Date).Select(o => new { o.Date, o.Price }))
                            chart.Enqueue(new Chart
                            {
                                Date = temp.Date,
                                Price = temp.Price,
                                Volume = 0
                            });
                        var writer = days.First().Code;
                        info = string.Concat(path, @"\", writer, ".res");
                    }
                    using (var sw = new StreamWriter(info, true))
                        foreach (var str in chart.OrderBy(o => o.Date))
                            sw.WriteLine(string.Concat(str.Date, ',', str.Price, ',', str.Volume));

                    catalog[new FileInfo(info).CreationTime] = new Queue<Chart>(chart);
                    chart.Clear();
                    var remaining = new Dictionary<string, string>();
                    using (var db = new GoblinBatDbContext(key))
                        foreach (var rInfo in db.Codes.Where(o => o.Code.Length == 8 && o.Code.Contains(code.Substring(0, 3)) && o.Code.Contains(code.Substring(5))).AsNoTracking().OrderBy(o => o.Info).Select(o => new { o.Code, o.Info }))
                            remaining[rInfo.Code] = rInfo.Info;

                    foreach (var kv in remaining.OrderBy(o => o.Value))
                    {
                        using (var db = new GoblinBatDbContext(key))
                            foreach (var tick in db.Futures.Where(o => o.Code.Length == 8 && o.Code.Equals(kv.Key)).AsNoTracking().OrderBy(o => o.Date).Select(o => new { o.Date, o.Price, o.Volume }))
                            {
                                if (info.Length == 6 && info.CompareTo(tick.Date.ToString().Substring(0, 6)) > 0)
                                    continue;

                                else if (tick.Date.ToString().Substring(0, 6).Equals(kv.Value.Substring(2)))
                                    break;

                                chart.Enqueue(new Chart
                                {
                                    Date = tick.Date,
                                    Price = tick.Price,
                                    Volume = tick.Volume
                                });
                            }
                        info = string.Concat(path, @"\", kv.Key, ".res");
                        using (var sw = new StreamWriter(info, true))
                            foreach (var str in chart.OrderBy(o => o.Date))
                                sw.WriteLine(string.Concat(str.Date, ',', str.Price, ',', str.Volume));

                        GC.Collect();
                        catalog[new FileInfo(info).LastWriteTime] = new Queue<Chart>(chart);
                        info = kv.Value.Substring(2);
                        chart.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return catalog;
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
        string MostRecentDate
        {
            get
            {
                for (int i = -1; i > int.MinValue; i--)
                {
                    var date = DateTime.Now.AddDays(i);
                    string max = date.ToString(recent);
                    using (var db = new GoblinBatDbContext(key))
                        try
                        {
                            if (db.Virtual.Any(o => o.Date.Equals(max)))
                                return max;
                        }
                        catch (Exception ex)
                        {
                            new ExceptionMessage(ex.StackTrace);
                        }
                }
                return string.Empty;
            }
        }
        protected CallUpGoblinBat(string key) : base(key)
        {
            this.key = key;
            ran = new Random();
        }
        protected internal string Path => System.IO.Path.Combine(Application.StartupPath, chart);
        protected internal const string futures = "000";
        protected internal const string kospi200f = "101";
        protected internal const string recent = "yyMMdd";
        protected internal const string res = ".res";
        const double marginRate = 0.1395;
        const string basic = "Base.res";
        const string chart = "ChartOf101000";
        const string charts = "Charts";
        const string remaining = "1545";
        const int ar = 10000000;
        const int cr = 1000000;
        readonly string key;
        readonly Random ran;
    }
}