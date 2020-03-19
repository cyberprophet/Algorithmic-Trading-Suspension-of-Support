using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using ShareInvest.Catalog;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpGoblinBat
    {
        protected CallUpGoblinBat(char initial)
        {
            this.initial = initial;
        }
        protected bool GetRecentAnalysis(Specify s)
        {
            var date = DateTime.Now.Hour < 5 && DateTime.Now.Hour >= 0 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd");

            try
            {
                using (var db = new GoblinBatDbContext(initial))
                {
                    return db.Logs.Any(o => o.Date.ToString().Equals(date) && o.Code.Equals(s.Code) && o.Assets.Equals(s.Assets) && o.Strategy.Equals(s.Strategy) && o.Date.Equals(s.Time));
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, s.Code);
            }
            return false;
        }
        protected bool GetRegister()
        {
            try
            {
                using (var db = new GoblinBatDbContext(initial))
                {
                    return db.Logs.Any();
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return false;
        }
        protected bool GetRemainingDate(string code, long date)
        {
            try
            {
                if (code.Length == 8 && date.ToString().Substring(6).Equals("151959000"))
                    using (var db = new GoblinBatDbContext(initial))
                    {
                        if (db.Codes.FirstOrDefault(o => o.Code.Equals(code)).Info.Substring(2).Equals(date.ToString().Substring(0, 6)))
                            return true;
                    }
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

            if (code.Length > 6 && code.Substring(5, 3).Equals("000"))
            {
                try
                {
                    using (var db = new GoblinBatDbContext(initial))
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

                                else if (date > 200315)
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
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, code);
                }
            }
            return chart;
        }
        protected string GetRecentFuturesCode(bool register)
        {
            if (register == false)
            {
                try
                {
                    using (var db = new GoblinBatDbContext(initial))
                    {
                        return db.Codes.First(code => code.Info.Equals(db.Codes.Where(o => o.Code.Substring(0, 3).Equals("101") && o.Code.Substring(5, 3).Equals("000")).Max(o => o.Info))).Code;
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            }
            return string.Empty;
        }
        protected void SetStorage(Logs log)
        {
            new Task(() =>
            {
                try
                {
                    using (var db = new GoblinBatDbContext(initial))
                    {
                        var check = db.Logs.Find(new object[]
                        {
                            log.Code,
                            log.Strategy,
                            log.Assets,
                            log.Date
                        });
                        if (check != null && db.Logs.Where(o => o.Cumulative.Equals(log.Cumulative) && check.Cumulative.Equals(log.Cumulative) && o.Revenue.Equals(log.Revenue) && check.Revenue.Equals(log.Revenue) && check.Unrealized.Equals(log.Unrealized) && o.Unrealized.Equals(log.Unrealized)).Any())
                            return;

                        db.Logs.AddOrUpdate(log);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            }).Start();
        }
        protected void DeleteLogs()
        {
            try
            {
                using (var db = new GoblinBatDbContext(initial))
                {
                    db.Logs.BulkDelete(db.Logs.Where(o => o.Code.Equals("101Q3000")));
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
        }private readonly char initial;
    }
}