using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using ShareInvest.GoblinBatContext;
using ShareInvest.Strategy;

namespace ShareInvest.CallUpDataBase
{
    public class CallUpGoblinBat
    {
        protected bool GetRecentAnalysis(Specify s)
        {
            var date = DateTime.Now.Hour < 5 && DateTime.Now.Hour >= 0 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd");
            using (var db = new GoblinBatDbContext())
            {
                return db.Logs.Any(o => o.Date.ToString().Equals(date) && o.Code.Equals(s.Code) && o.Assets.Equals(s.Assets) && o.Strategy.Equals(s.Strategy) && o.Time.Equals(s.Time) && o.Short.Equals(s.Short) && o.Long.Equals(s.Long));
            }
        }
        protected bool GetRegister()
        {
            using (var db = new GoblinBatDbContext())
            {
                return db.Logs.Any();
            }
        }
        protected Queue<Chart> GetChart(string code)
        {
            Queue<Chart> chart = new Queue<Chart>();
            using (var db = new GoblinBatDbContext())
            {
                if (code.Length > 6 && code.Substring(5, 3).Equals("000"))
                {
                    var tick = db.Futures.Where(o => o.Code.Contains(code.Substring(0, 3))).Select(o => new
                    {
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
                    foreach (var temp in db.Days.Where(o => o.Code.Equals(code)).Select(o => new
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
                        var index = remaining.FindIndex(o => o.Code.Equals(code));

                        if (index > 0 && int.Parse(temp.Date.ToString().Substring(0, 6)) < int.Parse(remaining[index - 1].Info.Substring(2)))
                            continue;

                        else if (index > 0 && int.Parse(temp.Date.ToString().Substring(0, 6)) == int.Parse(remaining[index - 1].Info.Substring(2)) && int.Parse(temp.Date.ToString().Substring(6, 4)) < 1520)
                            continue;

                        chart.Enqueue(new Chart
                        {
                            Date = temp.Date,
                            Price = temp.Price,
                            Volume = temp.Volume
                        });
                    }
                }
                else
                {

                }
            }
            return chart;
        }
        protected string GetRecentFuturesCode(bool register)
        {
            if (register == false)
                using (var db = new GoblinBatDbContext())
                {
                    var instance = SqlProviderServices.Instance;

                    return db.Codes.FirstOrDefault(code => code.Info.Equals(db.Codes.Where(o => o.Code.Substring(0, 3).Equals("101") && o.Code.Substring(5, 3).Equals("000")).Max(o => o.Info))).Code;
                }
            return string.Empty;
        }
    }
}