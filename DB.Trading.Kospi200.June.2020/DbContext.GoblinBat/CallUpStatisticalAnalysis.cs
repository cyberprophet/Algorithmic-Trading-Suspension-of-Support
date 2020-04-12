using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpStatisticalAnalysis : CallUpGoblinBat
    {
        protected string SetDate(string code)
        {
            string temp = DateTime.Now.ToString(date);

            try
            {
                using (var db = new GoblinBatDbContext(key))
                    return db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3)) && o.Date.Contains(temp.Substring(0, 6))).Last().Date;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return temp;
        }
        protected Queue<Quotes> GetQuotes(string code)
        {
            var chart = new Queue<Quotes>();
            Quotes quotes;

            if (code.Length > 6 && code.Substring(5, 3).Equals(futures))
                try
                {
                    using (var db = new GoblinBatDbContext(key))
                        foreach (var temp in db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3))).Select(o => new
                        {
                            o.Date,
                            o.Price,
                            o.Volume,
                            o.SellPrice,
                            o.SellQuantity,
                            o.TotalSellAmount,
                            o.BuyPrice,
                            o.BuyQuantity,
                            o.TotalBuyAmount
                        }).OrderBy(o => o.Date))
                        {
                            if (uint.TryParse(temp.Date.Substring(0, 8), out uint date) && date < 20040318)
                                continue;

                            if (temp.Price == null && temp.Volume == null)
                                quotes = new Quotes
                                {
                                    Time = temp.Date,
                                    SellPrice = temp.SellPrice,
                                    SellQuantity = temp.SellQuantity,
                                    SellAmount = temp.TotalSellAmount,
                                    BuyPrice = temp.BuyPrice,
                                    BuyQuantity = temp.BuyQuantity,
                                    BuyAmount = temp.TotalBuyAmount
                                };
                            else
                                quotes = new Quotes
                                {
                                    Time = temp.Date,
                                    Price = temp.Price,
                                    Volume = temp.Volume
                                };
                            chart.Enqueue(quotes);
                        }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, code);
                }
            return chart;
        }
        protected string GetStrategy()
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    return db.Codes.First(c => c.Info.Equals(db.Codes.Where(o => o.Code.Length == 8 && o.Code.Substring(0, 3).Equals(kospi200f) && o.Code.Substring(5, 3).Equals(futures)).Max(o => o.Info))).Code;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return string.Empty;
        }
        protected Stack<Specify[]> GetStrategy(string code)
        {
            var stack = new Stack<Specify[]>();

            try
            {
                using (var db = new GoblinBatDbContext(key))
                {

                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return stack;
        }
        protected long GetRepositoryID(Specify[] specifies)
        {
            try
            {
                int index = 0;
                string[] time = new string[specifies.Length], st = new string[specifies.Length], lt = new string[specifies.Length];

                foreach (var str in specifies.OrderByDescending(o => o.Time))
                {
                    time[index] = str.Time.ToString("D4");
                    st[index] = str.Short.ToString("D4");
                    lt[index++] = str.Long.ToString("D4");
                }
                string bt = time[0], bs = st[0], bl = lt[0], nt = time[1], ns = st[1], nl = lt[1], ot = time[2], os = st[2], ol = lt[2], ht = time[3], hs = st[3], hl = lt[3], xt = time[4], xs = st[4], xl = lt[4], pt = time[5], ps = st[5], pl = lt[5], qt = time[6], qs = st[6], ql = lt[6], tt = time[7], ts = st[7], tl = lt[7], dt = time[8], ds = st[8], dl = lt[8], mt = time[9], ms = st[9], ml = lt[9], name = specifies[0].Strategy, ro = specifies[0].RollOver.ToString().Substring(0, 1), margin = (specifies[0].MarginRate * mr).ToString(), commission = (specifies[0].Commission * cr).ToString(), code = string.Concat(specifies[0].Code.Substring(0, 3), specifies[0].Code.Substring(5)), assets = (specifies[0].Assets / ar).ToString("D4");
                using (var db = new GoblinBatDbContext(key))
                {
                    var temp = db.Strategy.Where(o => o.BaseTime.Equals(bt) && o.BaseShort.Equals(bs) && o.BaseLong.Equals(bl) && o.NonaTime.Equals(nt) && o.NonaShort.Equals(ns) && o.NonaLong.Equals(nl) && o.OctaTime.Equals(ot) && o.OctaLong.Equals(ol) && o.OctaShort.Equals(os) && o.HeptaTime.Equals(ht) && o.HeptaShort.Equals(hs) && o.HeptaLong.Equals(hl) && o.HexaTime.Equals(xt) && o.HexaShort.Equals(xs) && o.HexaLong.Equals(xl) && o.PentaTime.Equals(pt) && o.PantaShort.Equals(ps) && o.PantaLong.Equals(pl) && o.QuadTime.Equals(qt) && o.QuadShort.Equals(qs) && o.QuadLong.Equals(ql) && o.TriTime.Equals(tt) && o.TriShort.Equals(ts) && o.TriLong.Equals(tl) && o.DuoTime.Equals(dt) && o.DuoShort.Equals(ds) && o.DuoLong.Equals(dl) && o.MonoTime.Equals(mt) && o.MonoShort.Equals(ms) && o.MonoLong.Equals(ml) && o.Assets.Equals(assets) && o.Code.Equals(code) && o.Commission.Equals(commission) && o.MarginRate.Equals(margin) && o.Strategy.Equals(name) && o.RollOver.Equals(ro));

                    if (temp.Any())
                        return temp.First().Index;

                    db.Strategy.AddOrUpdate(new Strategics
                    {
                        Assets = assets,
                        Code = code,
                        Commission = commission,
                        MarginRate = margin,
                        Strategy = name,
                        RollOver = ro,
                        BaseTime = bt,
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
                        PantaShort = ps,
                        PantaLong = pl,
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
                    return db.BatchSaveChanges();
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, specifies[0].Assets.ToString("N0"));
            }
            return 0;
        }
        protected CallUpStatisticalAnalysis(string key) : base(key) => this.key = key;
        readonly string key;
        const int mr = 100;
        const int cr = 1000000;
        const int ar = 10000000;
        const string date = "yyMMddHHmmss";
    }
}