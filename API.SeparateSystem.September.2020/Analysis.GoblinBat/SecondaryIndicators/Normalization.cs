using System;
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
        internal double Normalize(double param)
        {
            uint end = 0, top = 1;

            return end + (param - Min) * (top - end) / (Max - Min);
        }
        internal Normalization(List<long> list)
        {
            max = list.Max();
            min = list.Where(o => o > long.MinValue).Min();
        }
        internal Normalization(Dictionary<DateTime, double> dictionary)
        {
            Max = dictionary.Max(o => o.Value);
            Min = dictionary.Min(o => o.Value);
        }
        internal double Max
        {
            get;
        }
        internal double Min
        {
            get;
        }
        readonly long max;
        readonly long min;
    }
}