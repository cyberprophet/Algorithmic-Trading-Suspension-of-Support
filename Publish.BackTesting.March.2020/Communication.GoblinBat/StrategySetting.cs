using System;
using System.Threading.Tasks;

namespace ShareInvest.Communication
{
    public class StrategySetting : IStrategySetting
    {
        public long Capital
        {
            get; set;
        }
        public int[] LongDay
        {
            get; set;
        }
        public int[] LongTick
        {
            get; set;
        }
        public int[] ShortDay
        {
            get; set;
        }
        public int[] ShortTick
        {
            get; set;
        }
        public int[] Reaction
        {
            get; set;
        }
        public int[] Hedge
        {
            get; set;
        }
        public int[] Base
        {
            get; set;
        }
        public int[] Sigma
        {
            get; set;
        }
        public int[] Percent
        {
            get; set;
        }
        public int[] Max
        {
            get; set;
        }
        public int[] Quantity
        {
            get; set;
        }
        public int[] Time
        {
            get; set;
        }
        public int EstimatedTime()
        {
            int count = 0;

            foreach (int h in Hedge)
                foreach (int t in Time)
                    foreach (int m in Max)
                        foreach (int p in Percent)
                            foreach (int s in Sigma)
                                foreach (int b in Base)
                                    foreach (int r in Reaction)
                                        foreach (int q in Quantity)
                                            foreach (int shortTick in ShortTick)
                                                foreach (int longTick in LongTick)
                                                    foreach (int shortDay in ShortDay)
                                                        foreach (int longDay in LongDay)
                                                            if (shortTick < longTick && shortDay < longDay && int.MaxValue.Equals(count++))
                                                                return 0;
            return count;
        }
    }
}