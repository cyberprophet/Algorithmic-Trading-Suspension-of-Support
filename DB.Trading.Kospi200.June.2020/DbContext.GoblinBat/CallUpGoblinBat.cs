using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        protected async Task<int> SetInsertStrategy(Strategics strategy)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    if (db.Strategy.Where(o => o.Assets.Equals(strategy.Assets) && o.Code.Equals(strategy.Code) && o.Commission.Equals(strategy.Commission) && o.MarginRate.Equals(strategy.MarginRate) && o.Strategy.Equals(strategy.Strategy) && o.RollOver.Equals(strategy.RollOver) && o.BaseTime.Equals(strategy.BaseTime) && o.BaseShort.Equals(strategy.BaseShort) && o.BaseLong.Equals(strategy.BaseLong) && o.NonaTime.Equals(strategy.NonaTime) && o.NonaShort.Equals(strategy.NonaShort) && o.NonaLong.Equals(strategy.NonaLong) && o.OctaTime.Equals(strategy.OctaTime) && o.OctaLong.Equals(strategy.OctaLong) && o.OctaShort.Equals(strategy.OctaShort) && o.HeptaTime.Equals(strategy.HeptaTime) && o.HeptaShort.Equals(strategy.HeptaShort) && o.HeptaLong.Equals(strategy.HeptaLong) && o.HexaTime.Equals(strategy.HexaTime) && o.HexaShort.Equals(strategy.HexaShort) && o.HexaLong.Equals(strategy.HexaLong) && o.PentaTime.Equals(strategy.PentaTime) && o.PantaShort.Equals(strategy.PantaShort) && o.PantaLong.Equals(strategy.PantaLong) && o.QuadTime.Equals(strategy.QuadTime) && o.QuadShort.Equals(strategy.QuadShort) && o.QuadLong.Equals(strategy.QuadLong) && o.TriTime.Equals(strategy.TriTime) && o.TriShort.Equals(strategy.TriShort) && o.TriLong.Equals(strategy.TriLong) && o.DuoTime.Equals(strategy.DuoTime) && o.DuoShort.Equals(strategy.DuoShort) && o.DuoLong.Equals(strategy.DuoLong) && o.MonoTime.Equals(strategy.MonoTime) && o.MonoShort.Equals(strategy.MonoShort) && o.MonoLong.Equals(strategy.MonoLong)).Any() == false)
                    {
                        db.Strategy.AddOrUpdate(strategy);

                        return await db.SaveChangesAsync();
                    }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, strategy.GetType().Name);
            }
            return int.MaxValue;
        }
        protected bool GetRemainingDate(string code, long date)
        {
            try
            {
                if (code.Length == 8 && date.ToString().Substring(6).Equals("151959000"))
                    using (var db = new GoblinBatDbContext(key))
                        if (db.Codes.FirstOrDefault(o => o.Code.Equals(code)).Info.Substring(2).Equals(date.ToString().Substring(0, 6)))
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
            var path = Path.Combine(Application.StartupPath, CallUpGoblinBat.chart);
            var exists = new DirectoryInfo(path);

            if (code.Length > 6 && code.Substring(5, 3).Equals(futures))
                try
                {
                    if (GetDirectoryExists(exists))
                    {
                        using (var sr = new StreamReader(string.Concat(path, @"\", basic)))
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
                            var tick = db.Futures.Where(o => o.Code.Contains(code.Substring(0, 3))).Select(o => new
                            {
                                o.Code,
                                o.Date,
                                o.Price,
                                o.Volume
                            }).OrderBy(o => o.Date);
                            var min = int.Parse(tick.Min(o => o.Date).ToString().Substring(0, 6));
                            var remaining = db.Codes.Where(o => o.Code.Length == 8 && o.Code.Contains(code.Substring(0, 3))).Select(o => new
                            {
                                o.Code,
                                o.Info
                            }).OrderBy(o => o.Info).ToList();

                            foreach (var temp in db.Days.Where(o => o.Code.Length == 8 && o.Code.Contains(code.Substring(0, 3))).Select(o => new
                            {
                                o.Date,
                                o.Price
                            }).OrderBy(o => o.Date))
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
                    SetChartDirectory(path, chart);
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
                        return db.Codes.First(code => code.Info.Equals(db.Codes.Where(o => o.Code.Substring(0, 3).Equals(kospi200f) && o.Code.Substring(5, 3).Equals(futures)).Max(o => o.Info))).Code;
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return string.Empty;
        }
        void SetChartDirectory(string path, Queue<Chart> chart)
        {
            using (var sw = new StreamWriter(string.Concat(path, @"\", basic), true))
                foreach (var str in chart.OrderBy(o => o.Date))
                    sw.WriteLine(string.Concat(str.Date, ',', str.Price, ',', str.Volume));
        }
        bool GetDirectoryExists(DirectoryInfo directory) => directory.Exists;
        protected CallUpGoblinBat(string key) => this.key = key;
        protected internal const string futures = "000";
        protected internal const string kospi200f = "101";
        const string basic = "Base.res";
        const string chart = "Chart";
        readonly string key;
    }
}