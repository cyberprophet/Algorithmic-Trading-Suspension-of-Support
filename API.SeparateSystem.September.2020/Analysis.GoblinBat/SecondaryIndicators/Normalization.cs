using System.Collections.Generic;
using System.Linq;

namespace ShareInvest.Analysis.SecondaryIndicators
{
    class Normalization
    {
        internal double Normalize(long param)
        {
            double end = 0D, top = 1D;

            return end + (param - min) * (top - end) / (max - min);
        }
        internal Normalization(List<long> list)
        {
            max = list.Max();
            min = list.Where(o => o > long.MinValue).Min();
        }
        readonly dynamic max;
        readonly dynamic min;
    }
}