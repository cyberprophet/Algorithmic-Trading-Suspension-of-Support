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
        public Information(string key) : base(key) => ran = new Random();
        Models.Simulations Imitation
        {
            get
            {
                var mi = new Models.Simulations
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
        public IEnumerable<Models.Simulations> GetStatistics(Models.Simulations strategy, double rate)
        {
            var list = new List<Models.Simulations>();
            int i, ms, ml, bs, bl;

            if (strategy.Strategy.Length == 2 && int.TryParse(strategy.Strategy, out int st))
                for (i = st > 30 ? st - 10 : st - st % 20; i < (st < 90 ? st + 10 : st + 10 - st % 90); i++)
                    for (ms = strategy.MonoShort - 3 > minShort ? strategy.MonoShort - 3 : minShort; ms < (strategy.MonoShort + 3 > maxShort ? maxShort + 1 : strategy.MonoShort + 3); ms++)
                        for (ml = strategy.MonoLong / 5 - 3 > minLong / 5 ? strategy.MonoLong - 15 : minLong; ml < (strategy.MonoLong / 5 + 3 > maxLong / 5 ? maxLong + 1 : strategy.MonoLong + 15 + 1); ml += 5)
                            for (bs = strategy.BaseShort - 3 > minShort ? strategy.BaseShort - 3 : minShort; bs < (strategy.BaseShort + 3 > maxShort ? maxShort + 1 : strategy.BaseShort + 3); bs++)
                                for (bl = strategy.BaseLong / 5 - 3 > minLong / 5 ? strategy.BaseLong - 15 : minLong; bl < (strategy.BaseLong / 5 + 3 > maxLong / 5 ? maxLong + 1 : strategy.BaseLong + 15 + 1); bl += 5)
                                    if (ms < ml && bs < bl)
                                        list.Add(new Models.Simulations
                                        {
                                            Assets = strategy.Assets,
                                            Code = Retrieve.Code,
                                            Commission = strategy.Commission,
                                            MarginRate = rate,
                                            Strategy = i.ToString("D2"),
                                            RollOver = strategy.RollOver,
                                            BaseTime = 1440,
                                            BaseShort = bs,
                                            BaseLong = bl,
                                            NonaTime = strategy.NonaTime,
                                            NonaShort = strategy.NonaShort,
                                            NonaLong = strategy.NonaLong,
                                            OctaTime = strategy.OctaTime,
                                            OctaShort = strategy.OctaShort,
                                            OctaLong = strategy.OctaLong,
                                            HeptaTime = strategy.HeptaTime,
                                            HeptaShort = strategy.HeptaShort,
                                            HeptaLong = strategy.HeptaLong,
                                            HexaTime = strategy.HexaTime,
                                            HexaShort = strategy.HexaShort,
                                            HexaLong = strategy.HexaLong,
                                            PentaTime = strategy.PentaTime,
                                            PentaShort = strategy.PentaShort,
                                            PentaLong = strategy.PentaLong,
                                            QuadTime = strategy.QuadTime,
                                            QuadShort = strategy.QuadShort,
                                            QuadLong = strategy.QuadLong,
                                            TriTime = strategy.TriTime,
                                            TriShort = strategy.TriShort,
                                            TriLong = strategy.TriLong,
                                            DuoTime = strategy.DuoTime,
                                            DuoShort = strategy.DuoShort,
                                            DuoLong = strategy.DuoLong,
                                            MonoTime = 1,
                                            MonoShort = ms,
                                            MonoLong = ml
                                        });
            return strategy.Strategy.Length == 2 ? list.Distinct() : list;
        }
        public Stack<Models.Simulations> GetStatistics(double count)
        {
            var stack = new Stack<Models.Simulations>();
            var strategy = GetStrategy(new List<Models.Simulations>(), Retrieve.Code);

            while (stack.Count < 375000 * count)
            {
                var mi = Imitation;

                if (mi.BaseShort < mi.BaseLong && mi.NonaShort < mi.NonaLong && mi.OctaShort < mi.OctaLong && mi.HeptaShort < mi.HeptaLong && mi.HexaShort < mi.HexaLong && mi.PentaShort < mi.PentaLong && mi.QuadShort < mi.QuadLong && mi.TriShort < mi.TriLong && mi.DuoShort < mi.DuoLong && mi.MonoShort < mi.MonoLong && mi.NonaTime > mi.OctaTime && mi.OctaTime > mi.HeptaTime && mi.HeptaTime > mi.HexaTime && mi.HexaTime > mi.PentaTime && mi.PentaTime > mi.QuadTime && mi.QuadTime > mi.TriTime && mi.TriTime > mi.DuoTime && mi.DuoTime > mi.MonoTime)
                    foreach (var st in strategy)
                        stack.Push(new Models.Simulations
                        {
                            Assets = st.Assets,
                            Code = st.Code,
                            Commission = st.Commission,
                            MarginRate = st.MarginRate,
                            Strategy = st.Strategy,
                            RollOver = st.RollOver,
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
            return stack;
        }
        public Stack<Models.Simulations> GetStatistics(bool external, double[] rate, double[] commission)
        {
            var stack = new Stack<Models.Simulations>(256);
            var assets = GetUserAssets(new List<long>());
            var roll = new bool[] { true, false };

            while (stack.Count < 397251)
                foreach (var asset in assets)
                    foreach (var co in commission)
                    {
                        var mt = ran.Next(1, maxHexa + 1);
                        var ms = ran.Next(minShort, maxShort + 1);
                        var ml = ran.Next(ms / 5, 25) * 5;
                        var bs = ran.Next(minShort, maxShort + 1);
                        var bl = ran.Next(bs / 5, 25) * 5;
                        var strategy = ran.Next(maxShort, maxLong - maxShort);

                        if (ms < ml && bs < bl && minLong < bl && ml <= maxLong && bl <= maxLong)
                            foreach (var over in roll)
                                stack.Push(new Models.Simulations
                                {
                                    Assets = asset,
                                    Code = Retrieve.Code,
                                    Commission = co,
                                    MarginRate = rate[0],
                                    Strategy = strategy.ToString(),
                                    RollOver = over,
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
                                    MonoTime = mt < 10 ? mt : mt % 5 == 0 ? mt : mt / 5 * 0b101,
                                    MonoShort = ms,
                                    MonoLong = ml
                                });
                    }
            if (external)
                foreach (var best in GetBestStrategy(stack, assets, Retrieve.Code))
                    stack.Push(best);

            return stack;
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
        public Stack<Models.Simulations> SeekingBetterAround()
        {
            var games = new Stack<Models.Simulations>();
            var game = Preheat(new Models.Simulations());

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
                                                                                                                                                games.Push(new Models.Simulations
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
            Statistics = GetBasicStrategy(initial, Retrieve.Code);
            RemainingDay = GetRemainingDay(Retrieve.Code);
        }
        public List<Models.Simulations> GetBestStrategy(bool external) => external ? GetBestExternalRecommend(new List<Models.Simulations>(128)) : GetBestStrategyRecommend(new List<Models.Simulations>(128));
        public IEnumerable<Models.Simulations> GetBestStrategy() => Preheat(new List<Models.Simulations>(256), Retrieve.Code);
        public static string[] RemainingDay
        {
            get; private set;
        }
        internal static List<Models.Statistics> Statistics
        {
            get; set;
        }
        readonly Random ran;
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