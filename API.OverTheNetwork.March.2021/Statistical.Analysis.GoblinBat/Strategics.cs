using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareInvest.Statistical
{
    public static class Strategics
    {
        public static (Stack<string>, Stack<string>) SetReservation()
        {
            Stack<string> sell_stack = new Stack<string>(), buy_stack = new Stack<string>();
            Parallel.ForEach(Reservation, new Action<Analysis>((r) =>
            {
                switch (r.Strategics)
                {
                    case Catalog.SatisfyConditionsAccordingToTrends sc:
                        int sell, buy, price = r.Current, upper = (int)(price * 1.3), lower = (int)(price * 0.7), bPrice, sPrice, quantity = r.Balance.Quantity;
                        var stock = r.Market;

                        if (sc.ReservationSellQuantity > 0)
                        {
                            sell = (int)(r.Balance.Purchase * (1 + sc.ReservationSellRate));
                            sPrice = r.GetStartingPrice(sell, stock);
                            sPrice = sPrice < lower ? lower + r.GetQuoteUnit(sPrice, stock) : sPrice;
                            r.SellPrice = sPrice;

                            while (sPrice < upper && quantity-- > 0)
                            {
                                Base.SendMessage(r.Code, sPrice.ToString("C0"), typeof(Strategics));
                                sell_stack.Push(SetOrder(r.Code, (int)Interface.OpenAPI.OrderType.신규매도, sPrice, sc.ReservationSellQuantity, ((int)Interface.OpenAPI.HogaGb.지정가).ToString("D2"), string.Empty));

                                for (int i = 0; i < sc.ReservationSellUnit; i++)
                                    sPrice += r.GetQuoteUnit(sPrice, stock);
                            }
                        }
                        if (sc.ReservationBuyQuantity > 0)
                        {
                            buy = (int)(r.Balance.Purchase * (1 - sc.ReservationBuyRate));
                            r.BuyPrice = r.GetStartingPrice(buy, stock);
                            bPrice = r.GetStartingPrice(lower, stock);

                            while (bPrice < upper && bPrice < buy && Cash > bPrice * (1.5e-4 + 1))
                            {
                                buy_stack.Push(SetOrder(r.Code, (int)Interface.OpenAPI.OrderType.신규매수, bPrice, sc.ReservationBuyQuantity, ((int)Interface.OpenAPI.HogaGb.지정가).ToString("D2"), string.Empty));
                                Cash -= (long)(bPrice * (1.5e-4 + 1));
                                Base.SendMessage(r.Code, string.Concat(bPrice.ToString("C0"), Cash.ToString("C0")), typeof(Strategics));

                                for (int i = 0; i < sc.ReservationBuyUnit; i++)
                                    bPrice += r.GetQuoteUnit(bPrice, stock);
                            }
                        }
                        break;
                }
            }));
            return (sell_stack, buy_stack);
        }
        public static string SetOrder(string code, int type, int price, int quantity, string hoga, string number)
            => string.Concat(code, ';', type, ';', price, ';', quantity, ';', hoga, ';', number);
        public static IEnumerable<Interface.IStrategics> SetStrategics(string[] param)
        {
            foreach (var str in param)
                if (str[2] == '|')
                {
                    var cs = str.Split('|');

                    if (Enum.TryParse(cs[0], out Interface.Strategics strategics))
                        switch (strategics)
                        {
                            case Interface.Strategics.SC:
                                if (cs.Length == 0x11
                                    && int.TryParse(cs[2], out int vShort) && int.TryParse(cs[3], out int vLong) && int.TryParse(cs[4], out int vTrend)
                                    && int.TryParse(cs[5], out int su) && int.TryParse(cs[6], out int sq) && double.TryParse(cs[7], out double vSubtraction)
                                    && int.TryParse(cs[8], out int au) && int.TryParse(cs[9], out int aq) && double.TryParse(cs[0xA], out double vAddition)
                                    && int.TryParse(cs[0xB], out int si) && int.TryParse(cs[0xC], out int tsq) && double.TryParse(cs[0xD], out double sp)
                                    && int.TryParse(cs[0xE], out int ai) && int.TryParse(cs[0xF], out int taq) && double.TryParse(cs[0x10], out double ap))
                                    yield return new Catalog.SatisfyConditionsAccordingToTrends
                                    {
                                        Code = cs[1],
                                        Short = vShort,
                                        Long = vLong,
                                        Trend = vTrend,
                                        ReservationSellUnit = su,
                                        ReservationSellQuantity = sq,
                                        ReservationSellRate = vSubtraction * 1e-2,
                                        ReservationBuyUnit = au,
                                        ReservationBuyQuantity = aq,
                                        ReservationBuyRate = vAddition * 1e-2,
                                        TradingSellInterval = si * 1e+3,
                                        TradingSellQuantity = tsq,
                                        TradingSellRate = sp * 1e-2,
                                        TradingBuyInterval = ai * 1e+3,
                                        TradingBuyQuantity = taq,
                                        TradingBuyRate = ap * 1e-2
                                    };
                                break;
                        }
                }
        }
        public static long Cash
        {
            get; set;
        }
        public static readonly Queue<Analysis> Reservation = new Queue<Analysis>();
    }
}