using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShareInvest.GoblinBatContext;
using ShareInvest.Models;

namespace ShareInvest.Communication
{
    public class Retrieve
    {
        private Retrieve(string code)
        {
            GetChart(code);
        }
        private void GetChart(string code)
        {
            string min = string.Empty;
            DayChart = new List<Days>();
            using var db = new GoblinBatDbContext();
            switch (code.Length)
            {
                case 6:
                    break;

                case 8:
                    if (code.Substring(5, 3).Equals("000"))
                    {
                        TickChart = new List<Futures>();
                        var tick = db.Futures.Where(o => o.Code.Contains(code.Substring(0, 3))).OrderBy(o => o.Date).ToList();
                        min = tick.Min(o => o.Date).ToString().Substring(0, 6);
                        var remaining = db.Codes.Where(o => o.Code.Contains(code.Substring(0, 3))).OrderBy(o => o.Info).ToList();

                        foreach (var temp in tick)
                        {
                            var index = remaining.FindIndex(o => o.Code.Equals(temp.Code));

                            if (index > 0 && int.Parse(temp.Date.ToString().Substring(0, 6)) < int.Parse(remaining[index - 1].Info.Substring(2)))
                                continue;

                            else if (index > 0 && int.Parse(temp.Date.ToString().Substring(0, 6)) == int.Parse(remaining[index - 1].Info.Substring(2)) && int.Parse(temp.Date.ToString().Substring(6, 4)) < 1520)
                                continue;

                            TickChart.Add(new Futures
                            {
                                Code = temp.Code,
                                Date = temp.Date,
                                Price = temp.Price,
                                Volume = temp.Volume
                            });
                        }
                    }
                    else
                    {

                    }
                    break;
            };
            foreach (var temp in db.Days.Where(o => o.Code.Equals(code)).OrderBy(o => o.Date).ToList())
            {
                if (temp.Date.ToString().Substring(2).Equals(min))
                    break;

                DayChart.Add(new Days
                {
                    Code = temp.Code,
                    Date = temp.Date,
                    Price = temp.Price
                });
            }
        }
        public IList DayChart
        {
            get; private set;
        }
        public IList TickChart
        {
            get; private set;
        }
        public static Retrieve GetInstance(string code)
        {
            if (retrieve == null)
                retrieve = new Retrieve(code);

            else if (!Code.Equals(code))
            {
                Code = code;
                retrieve = new Retrieve(code);
            }
            return retrieve;
        }
        private static string Code
        {
            get; set;
        }
        private static Retrieve retrieve;
    }
}