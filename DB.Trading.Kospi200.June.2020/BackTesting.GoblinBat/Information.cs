using System;
using System.Collections.Generic;
using ShareInvest.GoblinBatContext;
using ShareInvest.Message;

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
        }
        public void SetInsertStrategy(string[] param)
        {
            var list = new List<Models.Strategics>(1024);

            for (int ni = maxNona; ni > 0; ni -= minLong)
                for (int oi = maxOcta; oi > 0; oi -= minLong)
                    for (int hi = maxHepta; hi > 0; hi -= minLong)
                        for (int xi = maxHexa; xi > 0; xi -= minLong)
                            for (int pi = maxPenta; pi > 0; pi -= minLong)
                                for (int qi = maxQuad; qi > 0; qi -= minLong)
                                    for (int ti = maxTri; ti > 0; ti--)
                                        for (int di = maxDuo; di > 0; di--)
                                            for (int mi = maxMono; mi > 0; mi--)
                                                foreach (var vs in shortVariable)
                                                    foreach (var vl in longVariable)
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
                                                        if (list.Count == 750000)
                                                        {
                                                            SetInsertStrategy(list).Wait();
                                                            list = new List<Models.Strategics>(1024);
                                                            GC.Collect();
                                                        }
                                                    }
        }
        int[] GetVariable(int[] param, int type)
        {
            for (int i = param.Length; i > 0; i--)
                param[i - 1] = type * (type == 1 ? (i + 1) : i);

            return param;
        }
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