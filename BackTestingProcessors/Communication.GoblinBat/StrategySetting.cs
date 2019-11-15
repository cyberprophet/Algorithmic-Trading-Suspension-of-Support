using System;

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
        public int EstimatedTime()
        {
            int count = 0;

            foreach (int hedge in Enum.GetValues(typeof(IStrategySetting.Hedge)))
                foreach (int ld in LongDay)
                    foreach (int lt in LongTick)
                        foreach (int sd in ShortDay)
                            foreach (int st in ShortTick)
                                foreach (int r in Reaction)
                                {
                                    if (st >= lt || sd >= ld)
                                        continue;

                                    count++;
                                }
            return count;
        }
    }
}