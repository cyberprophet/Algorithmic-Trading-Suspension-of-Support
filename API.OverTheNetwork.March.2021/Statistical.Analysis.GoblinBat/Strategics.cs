using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareInvest.Statistical
{
    public static class Strategics
    {
        public static Stack<string> SetReservation()
        {
            var stack = new Stack<string>();

            Parallel.ForEach(Reservation, new Action<Analysis>((r) =>
            {
                
            }));
            return stack;
        }
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