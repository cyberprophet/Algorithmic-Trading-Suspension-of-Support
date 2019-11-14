using System.Collections.Generic;
using ShareInvest.Interface.x64;
using ShareInvest.SecondaryIndicators.x64;

namespace ShareInvest.Analysize.x64
{
    public class Strategy
    {
        public Strategy(IStrategy st)
        {
            ema = new EMA();
            shortEMA = new List<double>(32768);
            longEMA = new List<double>(32768);
            shortDay = new List<double>(512);
            longDay = new List<double>(512);
            shortTick = new List<double>(2097152);
            longTick = new List<double>(2097152);
            this.st = st;
        }
        private readonly IStrategy st;
        private readonly EMA ema;
        private readonly List<double> shortEMA;
        private readonly List<double> longEMA;
        private readonly List<double> shortDay;
        private readonly List<double> longDay;
        private readonly List<double> shortTick;
        private readonly List<double> longTick;
    }
}