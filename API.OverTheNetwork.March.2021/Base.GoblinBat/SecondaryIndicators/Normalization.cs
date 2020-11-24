using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareInvest.SecondaryIndicators
{
    public class Normalization
    {
        public double Normalize(long param)
        {
            double end = 0D, top = 1D;

            return end + (param - min) * (top - end) / (max - min);
        }
        public double Normalize(double param)
        {
            uint end = 0, top = 1;

            return end + (param - Min) * (top - end) / (Max - Min);
        }
        public Normalization(List<long> list)
        {
            max = list.Max();
            min = list.Where(o => o > long.MinValue).Min();
        }
        public Normalization(Dictionary<DateTime, double> dictionary)
        {
            Max = dictionary.Max(o => o.Value);
            Min = dictionary.Min(o => o.Value);
        }
        public double Max
        {
            get;
        }
        public double Min
        {
            get;
        }
        readonly long max;
        readonly long min;
    }
}