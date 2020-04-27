using System;
using System.Collections.Generic;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.Strategy
{
    enum Strategics
    {
        BaseTime = 0,
        BaseShort = 10,
        BaseLong = 20,
        NonaTime = 1,
        NonaShort = 11,
        NonaLong = 21,
        OctaTime = 2,
        OctaShort = 12,
        OctaLong = 22,
        HeptaTime = 3,
        HeptaShort = 13,
        HeptaLong = 23,
        HexaTime = 4,
        HexaShort = 14,
        HexaLong = 24,
        PentaTime = 5,
        PantaShort = 15,
        PantaLong = 25,
        QuadTime = 6,
        QuadShort = 16,
        QuadLong = 26,
        TriTime = 7,
        TriShort = 17,
        TriLong = 27,
        DuoTime = 8,
        DuoShort = 18,
        DuoLong = 28,
        MonoTime = 9,
        MonoShort = 19,
        MonoLong = 29
    }
    public partial class Information : CallUpGoblinBat
    {
        public List<string[]> GetUserIdentity() => GetUserIdentity(DateTime.Now);
        public Information(string key) : base(key)
        {
            shortVariable = GetVariable(new int[maxShort - 1], 1);
            longVariable = GetVariable(new int[maxLong / minLong], minLong);
            ran = new Random();
        }
        public List<Models.ImitationGames> GetStatistics(double count)
        {
            var list = new List<Models.ImitationGames>();

            while (list.Count < 135000 * count)
            {
                var mi = new Models.ImitationGames
                {
                    BaseTime = 1440,
                    BaseShort = ran.Next(2, 21),
                    NonaTime = ran.Next(6, 19) * 10,
                    NonaShort = ran.Next(2, 21),
                    OctaShort = ran.Next(2, 21),
                    HeptaShort = ran.Next(2, 21),
                    HexaShort = ran.Next(2, 21),
                    PentaShort = ran.Next(2, 21),
                    QuadShort = ran.Next(2, 21),
                    TriShort = ran.Next(2, 21),
                    DuoShort = ran.Next(2, 21),
                    MonoShort = ran.Next(2, 21)
                };
                mi.BaseLong = ran.Next(mi.BaseShort / 5, 25) * 5;
                mi.NonaLong = ran.Next(mi.NonaShort / 5, 25) * 5;
                mi.OctaLong = ran.Next(mi.OctaShort / 5, 25) * 5;
                mi.HeptaLong = ran.Next(mi.HeptaShort / 5, 25) * 5;
                mi.HexaLong = ran.Next(mi.HexaShort / 5, 25) * 5;
                mi.PentaLong = ran.Next(mi.PentaShort / 5, 25) * 5;
                mi.QuadLong = ran.Next(mi.QuadShort / 5, 25) * 5;
                mi.TriLong = ran.Next(mi.TriShort / 5, 25) * 5;
                mi.DuoLong = ran.Next(mi.DuoShort / 5, 25) * 5;
                mi.MonoLong = ran.Next(mi.MonoShort / 5, 25) * 5;
                mi.OctaTime = ran.Next(5, (maxOcta < mi.NonaTime ? maxOcta : mi.NonaTime) / 10) * 10;
                mi.HeptaTime = ran.Next(4, (maxHepta < mi.OctaTime ? maxHepta : mi.OctaTime) / 5) * 5;
                mi.HexaTime = ran.Next(3, (maxHexa < mi.HeptaTime ? maxHexa : mi.HeptaTime) / 5) * 5;
                mi.PentaTime = ran.Next(2, (maxPenta < mi.HexaTime ? maxPenta : mi.HexaTime) / 5) * 5;
                mi.QuadTime = ran.Next(1, (maxQuad < mi.PentaTime ? maxQuad : mi.PentaTime) / 5) * 5;
                mi.TriTime = ran.Next(3, maxTri < mi.QuadTime ? maxTri : mi.QuadTime);
                mi.DuoTime = ran.Next(2, maxDuo < mi.TriTime ? maxDuo : mi.TriTime);
                mi.MonoTime = ran.Next(1, maxMono < mi.DuoTime ? maxMono : mi.TriTime);

                if (mi.BaseShort < mi.BaseLong && mi.NonaShort < mi.NonaLong && mi.OctaShort < mi.OctaLong && mi.HeptaShort < mi.HeptaLong && mi.HexaShort < mi.HexaLong && mi.PentaShort < mi.PentaLong && mi.QuadShort < mi.QuadLong && mi.TriShort < mi.TriLong && mi.DuoShort < mi.DuoLong && mi.MonoShort < mi.MonoLong && mi.NonaTime > mi.OctaTime && mi.OctaTime > mi.HeptaTime && mi.HeptaTime > mi.HexaTime && mi.HexaTime > mi.PentaTime && mi.PentaTime > mi.QuadTime && mi.QuadTime > mi.TriTime && mi.TriTime > mi.DuoTime && mi.DuoTime > mi.MonoTime)
                    foreach (var model in Statistics)
                        list.Add(new Models.ImitationGames
                        {
                            Assets = model.Assets,
                            Code = model.Code,
                            Commission = model.Commission,
                            MarginRate = model.MarginRate,
                            Strategy = model.Strategy,
                            RollOver = model.RollOver,
                            BaseTime = mi.BaseTime,
                            BaseShort = mi.BaseShort,
                            BaseLong = mi.BaseLong,
                            NonaTime = mi.NonaTime,
                            NonaShort = mi.NonaShort,
                            NonaLong = mi.NonaLong,
                            OctaTime = mi.OctaTime,
                            OctaShort = mi.OctaShort,
                            OctaLong = mi.OctaLong,
                            HeptaTime = mi.HeptaTime,
                            HeptaShort = mi.HeptaShort,
                            HeptaLong = mi.HeptaLong,
                            HexaTime = mi.HexaTime,
                            HexaShort = mi.HexaShort,
                            HexaLong = mi.HexaLong,
                            PentaTime = mi.PentaTime,
                            PentaShort = mi.PentaShort,
                            PentaLong = mi.PentaLong,
                            QuadTime = mi.QuadTime,
                            QuadShort = mi.QuadShort,
                            QuadLong = mi.QuadLong,
                            TriTime = mi.TriTime,
                            TriShort = mi.TriShort,
                            TriLong = mi.TriLong,
                            DuoTime = mi.DuoTime,
                            DuoShort = mi.DuoShort,
                            DuoLong = mi.DuoLong,
                            MonoTime = mi.MonoTime,
                            MonoShort = mi.MonoShort,
                            MonoLong = mi.MonoLong
                        });
            }
            return list;
        }
        public void SetInsertBaseStrategy(string[] strategy, double[] rate, double[] commission)
        {
            var queue = new Queue<Models.Statistics>();

            foreach (var str in strategy)
                foreach (var margin in rate)
                    foreach (var co in commission)
                        if (str.Equals("Auto") == false)
                        {
                            queue.Enqueue(new Models.Statistics
                            {
                                Assets = 90000000,
                                Code = Retrieve.Code,
                                Commission = co,
                                MarginRate = margin,
                                Strategy = str,
                                RollOver = true
                            });
                            queue.Enqueue(new Models.Statistics
                            {
                                Assets = 90000000,
                                Code = Retrieve.Code,
                                Commission = co,
                                MarginRate = margin,
                                Strategy = str,
                                RollOver = false
                            });
                        }
            SetInsertBaseStrategy(queue);
        }
        public void SetInsertStrategy(string[] param)
        {
            var list = new List<Models.Strategics>(1024);

            for (int mi = maxMono; mi > 0; mi--)
                for (int di = maxDuo; di > 0; di--)
                    for (int ti = maxTri; ti > 0; ti--)
                        for (int qi = maxQuad; qi > 0; qi -= minLong)
                            for (int pi = maxPenta; pi > 0; pi -= minLong)
                                for (int xi = maxHexa; xi > 0; xi -= minLong)
                                    for (int hi = maxHepta; hi > 0; hi -= minLong)
                                        for (int oi = maxOcta; oi > 0; oi -= minLong)
                                            for (int ni = maxNona; ni > 0; ni -= minLong)
                                                foreach (var vl in longVariable)
                                                    foreach (var vs in shortVariable)
                                                    {
                                                        if (vs >= vl || vs < minShort || vl < minLong)
                                                            continue;

                                                        if (mi < di && di < ti && ti < qi && qi < pi && pi < xi && xi < hi && hi < oi && oi < ni && ni < maxBase)
                                                            list.Add(new Models.Strategics
                                                            {
                                                                Assets = param[0],
                                                                Code = param[1],
                                                                Commission = param[2],
                                                                MarginRate = param[3],
                                                                Strategy = param[4],
                                                                RollOver = param[5],
                                                                BaseTime = maxBase.ToString("D4"),
                                                                BaseShort = vs.ToString("D4"),
                                                                BaseLong = vl.ToString("D4"),
                                                                NonaTime = ni.ToString("D4"),
                                                                NonaShort = vs.ToString("D4"),
                                                                NonaLong = vl.ToString("D4"),
                                                                OctaTime = oi.ToString("D4"),
                                                                OctaShort = vs.ToString("D4"),
                                                                OctaLong = vl.ToString("D4"),
                                                                HeptaTime = hi.ToString("D4"),
                                                                HeptaShort = vs.ToString("D4"),
                                                                HeptaLong = vl.ToString("D4"),
                                                                HexaTime = xi.ToString("D4"),
                                                                HexaShort = vs.ToString("D4"),
                                                                HexaLong = vl.ToString("D4"),
                                                                PentaTime = pi.ToString("D4"),
                                                                PantaShort = vs.ToString("D4"),
                                                                PantaLong = vl.ToString("D4"),
                                                                QuadTime = qi.ToString("D4"),
                                                                QuadShort = vs.ToString("D4"),
                                                                QuadLong = vl.ToString("D4"),
                                                                TriTime = ti.ToString("D4"),
                                                                TriShort = vs.ToString("D4"),
                                                                TriLong = vl.ToString("D4"),
                                                                DuoTime = di.ToString("D4"),
                                                                DuoShort = vs.ToString("D4"),
                                                                DuoLong = vl.ToString("D4"),
                                                                MonoTime = mi.ToString("D4"),
                                                                MonoShort = vs.ToString("D4"),
                                                                MonoLong = vl.ToString("D4")
                                                            });
                                                        if (list.Count == 125000)
                                                        {
                                                            SetInsertStrategy(list);
                                                            list = new List<Models.Strategics>(1024);
                                                            GC.Collect();
                                                        }
                                                    }
        }
        public void GetUserIdentity(char initial) => Statistics = GetBasicStrategy(initial);
        public List<Models.ImitationGames> GetBestStrategy() => GetBestStrategyRecommand(new List<Models.ImitationGames>(128));
        internal static List<Models.Statistics> Statistics
        {
            get; set;
        }
        int[] GetVariable(int[] param, int type)
        {
            for (int i = param.Length; i > 0; i--)
                param[i - 1] = type * (type == 1 ? (i + 1) : i);

            return param;
        }
        readonly Random ran;
        readonly int[] shortVariable;
        readonly int[] longVariable;
        const int maxBase = 1440;
        const int maxNona = 180;
        const int maxOcta = 150;
        const int maxHepta = 120;
        const int maxHexa = 90;
        const int maxPenta = 60;
        const int maxQuad = 45;
        const int maxTri = 30;
        const int maxDuo = 15;
        const int maxMono = 5;
        const int maxLong = 120;
        const int maxShort = 20;
        const int minLong = 5;
        const int minShort = 2;
    }
}