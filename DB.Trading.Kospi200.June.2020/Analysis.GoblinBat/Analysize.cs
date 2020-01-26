using System;
using System.Linq;
using System.Threading.Tasks;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.Analysis
{
    public class Analysize
    {
        private int[] Time
        {
            get
            {
                return new int[] { -1, 1, 3, 5, 10, 15, 30, 60, 1440 };
            }
        }
        private int[] Short
        {
            get
            {
                return new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            }
        }
        private int[] Long
        {
            get
            {
                return new int[] { 20, 25, 30, 35, 40, 45, 50, 60, 65, 70, 90, 120 };
            }
        }
        private StrategyComposition SetStrategy(Random random)
        {
            return new StrategyComposition
            {
                Code = "101Q3000",
                Strategy = "TrendFollowing",
                Assets = 35000000,
                Time = Time[random.Next(0, Time.Length)],
                Short = Short[random.Next(0, Short.Length)],
                Long = Long[random.Next(0, Long.Length)]
            };
        }
        private void StartBackTesting(StrategyComposition strategy)
        {
            using (var db = new GoblinBatDbContext())
            {
                if (db.Logs.Any(o => o.Code.Equals(strategy.Code) && o.Strategy.Equals(strategy.Strategy) && o.Assets.Equals(strategy.Assets) && o.Time.Equals(strategy.Time) && o.Short.Equals(strategy.Short) && o.Long.Equals(strategy.Long) && o.Date.Equals(DateTime.Now.ToString("yyMMdd"))))
                    StartBackTesting(SetStrategy(new Random()));

                else if (strategy.Code.Length == 8)
                {
                    var daily = db.Days.Where(o => o.Code.Contains(strategy.Code.Substring(0, 3)) && o.Code.Contains(strategy.Code.Substring(5, 3))).OrderBy(o => o.Date).ToList();
                    var tick = db.Futures.Where(o => o.Code.Contains(strategy.Code.Substring(0, 3)) && o.Code.Contains(strategy.Code.Substring(5, 3))).OrderBy(o => o.Date).ToList();

                }
            }
        }
        public Analysize()
        {
            var max = (int)(Environment.ProcessorCount * 1.5);

            Parallel.For(0, max, new ParallelOptions
            {
                MaxDegreeOfParallelism = max
            }, index => StartBackTesting(SetStrategy(new Random(index))));
        }
    }
}