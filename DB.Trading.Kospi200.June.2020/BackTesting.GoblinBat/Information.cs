using System;
using System.Collections.Generic;
using System.Linq;

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
        Models.ImitationGames Imitation
        {
            get
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

                return mi;
            }
        }
        public List<Models.ImitationGames> GetStatistics(double count)
        {
            var list = new List<Models.ImitationGames>();

            while (list.Count < 375000 * count)
            {
                var mi = Imitation;

                if (Statistics.Count == 1 && Statistics.Where(o => o.Strategy.Equals(string.Empty)).Any() && mi.BaseShort < mi.BaseLong && mi.NonaShort < mi.NonaLong && mi.OctaShort < mi.OctaLong && mi.HeptaShort < mi.HeptaLong && mi.HexaShort < mi.HexaLong && mi.PentaShort < mi.PentaLong && mi.QuadShort < mi.QuadLong && mi.TriShort < mi.TriLong && mi.DuoShort < mi.DuoLong && mi.MonoShort < mi.MonoLong && mi.NonaTime > mi.OctaTime && mi.OctaTime > mi.HeptaTime && mi.HeptaTime > mi.HexaTime && mi.HexaTime > mi.PentaTime && mi.PentaTime > mi.QuadTime && mi.QuadTime > mi.TriTime && mi.TriTime > mi.DuoTime && mi.DuoTime > mi.MonoTime)
                {
                    mi.Strategy = ran.Next(20, 100).ToString();
                    var model = Statistics.First();
                    list.Add(new Models.ImitationGames
                    {
                        Assets = model.Assets,
                        Code = model.Code,
                        Commission = model.Commission,
                        MarginRate = model.MarginRate,
                        Strategy = mi.Strategy,
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
                    continue;
                }
                if (mi.BaseShort < mi.BaseLong && mi.NonaShort < mi.NonaLong && mi.OctaShort < mi.OctaLong && mi.HeptaShort < mi.HeptaLong && mi.HexaShort < mi.HexaLong && mi.PentaShort < mi.PentaLong && mi.QuadShort < mi.QuadLong && mi.TriShort < mi.TriLong && mi.DuoShort < mi.DuoLong && mi.MonoShort < mi.MonoLong && mi.NonaTime > mi.OctaTime && mi.OctaTime > mi.HeptaTime && mi.HeptaTime > mi.HexaTime && mi.HexaTime > mi.PentaTime && mi.PentaTime > mi.QuadTime && mi.QuadTime > mi.TriTime && mi.TriTime > mi.DuoTime && mi.DuoTime > mi.MonoTime)
                    foreach (var model in Statistics)
                        if (model.Strategy.Length > 2)
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
        public Stack<Models.ImitationGames> GetStatistics(double[] rate, double[] commission)
        {
            var stack = new Stack<Models.ImitationGames>(256);
            var assets = GetUserAssets(new List<long>());

            while (stack.Count < 75123)
                foreach (var asset in assets)
                    foreach (var co in commission)
                    {
                        var mt = ran.Next(1, maxHexa + 1);
                        var ms = ran.Next(minShort, maxShort + 1);
                        var ml = ran.Next(ms / 5, 25) * 5;
                        var bs = ran.Next(minShort, maxShort + 1);
                        var bl = ran.Next(bs / 5, 25) * 5;
                        var strategy = ran.Next(maxShort, maxLong - maxShort);

                        if (ms < ml && bs < bl && ml <= maxLong && bl <= maxLong)
                            stack.Push(new Models.ImitationGames
                            {
                                Assets = asset,
                                Code = Retrieve.Code,
                                Commission = co,
                                MarginRate = rate[0],
                                Strategy = strategy.ToString(),
                                RollOver = true,
                                BaseTime = 1440,
                                BaseShort = bs,
                                BaseLong = bl,
                                NonaTime = 0,
                                NonaShort = 4,
                                NonaLong = 60,
                                OctaTime = 0,
                                OctaShort = 4,
                                OctaLong = 60,
                                HeptaTime = 0,
                                HeptaShort = 4,
                                HeptaLong = 60,
                                HexaTime = 0,
                                HexaShort = 4,
                                HexaLong = 60,
                                PentaTime = 0,
                                PentaShort = 4,
                                PentaLong = 60,
                                QuadTime = 0,
                                QuadShort = 4,
                                QuadLong = 60,
                                TriTime = 0,
                                TriShort = 4,
                                TriLong = 60,
                                DuoTime = 0,
                                DuoShort = 4,
                                DuoLong = 60,
                                MonoTime = mt,
                                MonoShort = ms,
                                MonoLong = ml
                            });
                    }
            return GetBestStrategy(stack,assets);
        }
        public void SetInsertBaseStrategy(string[] strategy, double[] rate, double[] commission)
        {
            var queue = new Queue<Models.Statistics>();
            var list = new List<string>(strategy);

            for (int i = 20; i < 100; i++)
                list.Add(i.ToString());

            foreach (var assets in GetUserAssets(new List<long>()))
                foreach (var str in list)
                    foreach (var co in commission)
                        if (str.Equals("Auto") == false)
                        {
                            queue.Enqueue(new Models.Statistics
                            {
                                Assets = assets,
                                Code = Retrieve.Code,
                                Commission = co,
                                MarginRate = rate[0],
                                Strategy = str,
                                RollOver = true
                            });
                            queue.Enqueue(new Models.Statistics
                            {
                                Assets = assets,
                                Code = Retrieve.Code,
                                Commission = co,
                                MarginRate = rate[0],
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
        public Stack<Models.ImitationGames> SeekingBetterAround()
        {
            var games = new Stack<Models.ImitationGames>();
            var game = Preheat(new Models.ImitationGames());

            if (game != null)
                foreach (int ns in new int[] { game.NonaShort, ran.Next(0, 2) == 0 ? game.NonaShort - 1 : game.NonaShort + 1 })
                    foreach (int os in new int[] { game.OctaShort, ran.Next(0, 2) == 0 ? game.OctaShort - 1 : game.OctaShort + 1 })
                        foreach (int hs in new int[] { game.HeptaShort, ran.Next(0, 2) == 0 ? game.HeptaShort - 1 : game.HeptaShort + 1 })
                            foreach (int xs in new int[] { game.HexaShort, ran.Next(0, 2) == 0 ? game.HexaShort - 1 : game.HexaShort + 1 })
                                foreach (int ps in new int[] { game.PentaShort, ran.Next(0, 2) == 0 ? game.PentaShort - 1 : game.PentaShort + 1 })
                                    foreach (int qs in new int[] { game.QuadShort, ran.Next(0, 2) == 0 ? game.QuadShort - 1 : game.QuadShort + 1 })
                                        foreach (int ts in new int[] { game.TriShort, ran.Next(0, 2) == 0 ? game.TriShort - 1 : game.TriShort + 1 })
                                            foreach (int ds in new int[] { game.DuoShort, ran.Next(0, 2) == 0 ? game.DuoShort - 1 : game.DuoShort + 1 })
                                                foreach (int ms in new int[] { game.MonoShort, ran.Next(0, 2) == 0 ? game.MonoShort - 1 : game.MonoShort + 1 })
                                                    foreach (int bs in new int[] { game.BaseShort, ran.Next(0, 2) == 0 ? game.BaseShort - 1 : game.BaseShort + 1 })
                                                        foreach (int bl in new int[] { game.BaseLong, ran.Next(0, 2) == 0 ? game.BaseLong - 5 : game.BaseLong + 5 })
                                                            foreach (int nl in new int[] { game.NonaLong, ran.Next(0, 2) == 0 ? game.NonaLong - 5 : game.NonaLong + 5 })
                                                                foreach (int ol in new int[] { game.OctaLong, ran.Next(0, 2) == 0 ? game.OctaLong - 5 : game.OctaLong + 5 })
                                                                    foreach (int hl in new int[] { game.HeptaLong, ran.Next(0, 2) == 0 ? game.HeptaLong - 5 : game.HeptaLong + 5 })
                                                                        foreach (int xl in new int[] { game.HexaLong, ran.Next(0, 2) == 0 ? game.HexaLong - 5 : game.HexaLong + 5 })
                                                                            foreach (int pl in new int[] { game.PentaLong, ran.Next(0, 2) == 0 ? game.PentaLong - 5 : game.PentaLong + 5 })
                                                                                foreach (int ql in new int[] { game.QuadLong, ran.Next(0, 2) == 0 ? game.QuadLong - 5 : game.QuadLong + 5 })
                                                                                    foreach (int tl in new int[] { game.TriLong, ran.Next(0, 2) == 0 ? game.TriLong - 5 : game.TriLong + 5 })
                                                                                        foreach (int dl in new int[] { game.DuoLong, ran.Next(0, 2) == 0 ? game.DuoLong - 5 : game.DuoLong + 5 })
                                                                                            foreach (int ml in new int[] { game.MonoLong, ran.Next(0, 2) == 0 ? game.MonoLong - 5 : game.MonoLong + 5 })
                                                                                                foreach (int nt in new int[] { game.NonaTime, ran.Next(0, 2) == 0 ? game.NonaTime - 10 : game.NonaTime + 10 })
                                                                                                    foreach (int ot in new int[] { game.OctaTime, ran.Next(0, 2) == 0 ? game.OctaTime - 10 : game.OctaTime + 10 })
                                                                                                        foreach (int ht in new int[] { game.HeptaTime, ran.Next(0, 2) == 0 ? game.HeptaTime - 10 : game.HeptaTime + 10 })
                                                                                                            foreach (int xt in new int[] { game.HexaTime, ran.Next(0, 2) == 0 ? game.HexaTime - 10 : game.HexaTime + 10 })
                                                                                                                foreach (int pt in new int[] { game.PentaTime, ran.Next(0, 2) == 0 ? game.PentaTime - 5 : game.PentaTime + 5 })
                                                                                                                    foreach (int qt in new int[] { game.QuadTime, ran.Next(0, 2) == 0 ? game.QuadTime - 5 : game.QuadTime + 5 })
                                                                                                                        foreach (int tt in new int[] { game.TriTime, ran.Next(0, 2) == 0 ? game.TriTime - 1 : game.TriTime + 1 })
                                                                                                                            foreach (int dt in new int[] { game.DuoTime, ran.Next(0, 2) == 0 ? game.DuoTime - 1 : game.DuoTime + 1 })
                                                                                                                                foreach (var mt in new int[] { game.MonoTime, ran.Next(0, 2) == 0 ? game.MonoTime - 1 : game.MonoTime + 1 })
                                                                                                                                    if (bs >= minShort && bs <= maxShort && ns >= minShort && ns <= maxShort && os >= minShort && os <= maxShort && hs >= minShort && hs <= maxShort && xs >= minShort && xs <= maxShort && ps >= minShort && ps <= maxShort && qs >= minShort && qs <= maxShort && ts >= minShort && ts <= maxShort && ds >= minShort && ds <= maxShort && ms >= minShort && ms <= maxShort && bl >= minLong && bl <= maxLong && nl >= minLong && nl <= maxLong && ol >= minLong && ol <= maxLong && hl >= minLong && hl <= maxLong && xl >= minLong && xl <= maxLong && pl >= minLong && pl <= maxLong && ql >= minLong && ql <= maxLong && tl >= minLong && tl <= maxLong && dl >= minLong && dl <= maxLong && ml >= minLong && ml <= maxLong && bs < bl && ns < nl && os < ol && hs < hl && xs < xl && ps < pl && qs < ql && ts < tl && ds < dl && ms < ml && nt <= maxNona && ot <= maxOcta && ht <= maxHepta && xt <= maxHexa && pt <= maxPenta && qt <= maxQuad && tt <= maxTri && dt <= maxDuo && mt <= maxMono && nt > ot && ot > ht && ht > xt && xt > pt && pt > qt && qt > tt && tt > dt && dt > mt && mt > 0)
                                                                                                                                        foreach (var model in Statistics.Select(o => new { o.Strategy }).Distinct())
                                                                                                                                            if (model.Strategy.Length > 2)
                                                                                                                                            {
                                                                                                                                                games.Push(new Models.ImitationGames
                                                                                                                                                {
                                                                                                                                                    Assets = game.Assets,
                                                                                                                                                    Code = game.Code,
                                                                                                                                                    Commission = game.Commission,
                                                                                                                                                    MarginRate = game.MarginRate,
                                                                                                                                                    Strategy = model.Strategy,
                                                                                                                                                    RollOver = game.RollOver,
                                                                                                                                                    BaseTime = maxBase,
                                                                                                                                                    BaseShort = bs,
                                                                                                                                                    BaseLong = bl,
                                                                                                                                                    NonaTime = nt,
                                                                                                                                                    NonaShort = ns,
                                                                                                                                                    NonaLong = nl,
                                                                                                                                                    OctaTime = ot,
                                                                                                                                                    OctaShort = os,
                                                                                                                                                    OctaLong = ol,
                                                                                                                                                    HeptaTime = ht,
                                                                                                                                                    HeptaShort = hs,
                                                                                                                                                    HeptaLong = hl,
                                                                                                                                                    HexaTime = xt,
                                                                                                                                                    HexaShort = xs,
                                                                                                                                                    HexaLong = xl,
                                                                                                                                                    PentaTime = pt,
                                                                                                                                                    PentaShort = ps,
                                                                                                                                                    PentaLong = pl,
                                                                                                                                                    QuadTime = qt,
                                                                                                                                                    QuadShort = qs,
                                                                                                                                                    QuadLong = ql,
                                                                                                                                                    TriTime = tt,
                                                                                                                                                    TriShort = ts,
                                                                                                                                                    TriLong = tl,
                                                                                                                                                    DuoTime = dt,
                                                                                                                                                    DuoShort = ds,
                                                                                                                                                    DuoLong = dl,
                                                                                                                                                    MonoTime = mt,
                                                                                                                                                    MonoShort = ms,
                                                                                                                                                    MonoLong = ml
                                                                                                                                                });
                                                                                                                                                if (games.Count > 937)
                                                                                                                                                    return games;
                                                                                                                                            }
            return games;
        }
        public void GetUserIdentity(char initial)
        {
            Statistics = GetBasicStrategy(initial);
            RemainingDay = GetRemainingDay(Retrieve.Code);
        }
        public List<Models.ImitationGames> GetBestStrategy(bool external) => external ? GetBestExternalRecommend(new List<Models.ImitationGames>(128)) : GetBestStrategyRecommend(new List<Models.ImitationGames>(128));
        public List<Models.ImitationGames> GetBestStrategy() => Preheat(new List<Models.ImitationGames>(256));
        internal static List<Models.Statistics> Statistics
        {
            get; set;
        }
        internal static string[] RemainingDay
        {
            get; private set;
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