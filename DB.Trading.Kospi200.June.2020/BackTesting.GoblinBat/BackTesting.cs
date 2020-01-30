using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShareInvest.GoblinBatContext;
using ShareInvest.Interface.Struct;

namespace ShareInvest.Strategy
{
    public class BackTesting : CallUpGoblinBat
    {
        public BackTesting()
        {

        }
        public BackTesting(long assets)
        {
            Retrieve.GetInstance(Code = GetRecentFuturesCode(GetRegister()));
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
                MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 0.7)
            },
            new Action<Specify>(param =>
            {
                if (GetRecentAnalysis(param) == false)
                    new Analysis(param);
            }));
            new BackTesting();
        }
        private int[] Time
        {
            get
            {
                return new int[] { -1, 1, 3, 5, 10, 15, 30, 45, 60, 1440 };
            }
        }
        private string[] Strategy
        {
            get
            {
                return new string[] { "TF" };
            }
        }
        private string Code
        {
            get; set;
        }
    }
}