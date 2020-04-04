using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShareInvest.Catalog;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.Strategy
{
    class BackTesting : CallUpGoblinBat
    {
        BackTesting(string key) : base(key)
        {

        }
        BackTesting(long assets, string key) : base(key)
        {
            var list = new List<Specify>();
            int i, j;

            for (j = 15; j <= 120; j += 5)
                for (i = 2; i < 10; i++)
                    foreach (string strategy in Strategy)
                        foreach (int time in Time)
                            list.Add(new Specify
                            {
                                Time = time,
                                Code = Code,
                                Assets = assets,
                                Strategy = strategy,
                                Short = time < 0 ? i * 15 : i,
                                Long = time < 0 ? j * 10 : j
                            });
            Parallel.ForEach(list, new ParallelOptions
            {
                MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 0.65)
            },
            new Action<Specify>(param =>
            {
                if (GetRecentAnalysis(param) == false)
                    new Analysis(param, key);
            }));
            new BackTesting(key);
        }
        string Code
        {
            get; set;
        }
        string[] Strategy => new string[] { "TF" };
        int[] Time => new int[] { -1, 1, 3, 5, 10, 15, 30, 45, 60, 1440 };
    }
}