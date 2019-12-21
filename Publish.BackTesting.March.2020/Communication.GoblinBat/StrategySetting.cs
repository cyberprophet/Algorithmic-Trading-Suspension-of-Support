using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
        public List<string> EstimatedTime(List<string> list, int count)
        {
            int baseTick, sigma, percent, max, sd, ld;

            foreach (int h in Hedge)
            {
                foreach (int t in Time)
                {
                    foreach (int m in Max)
                    {
                        foreach (int p in Percent)
                        {
                            foreach (int s in Sigma)
                            {
                                foreach (int b in Base)
                                {
                                    foreach (int r in Reaction)
                                    {
                                        foreach (int q in Quantity)
                                        {
                                            foreach (int shortTick in ShortTick)
                                            {
                                                foreach (int longTick in LongTick)
                                                {
                                                    foreach (int shortDay in ShortDay)
                                                    {
                                                        foreach (int longDay in LongDay)
                                                        {
                                                            if (shortTick >= longTick || shortDay > 0 && longDay > 0 && shortDay >= longDay)
                                                                continue;

                                                            if (b < 1 || s < 1 || p < 1 || m < 1)
                                                            {
                                                                baseTick = 0;
                                                                sigma = 0;
                                                                percent = 0;
                                                                max = 0;
                                                            }
                                                            else
                                                            {
                                                                baseTick = b;
                                                                sigma = s;
                                                                percent = p;
                                                                max = m;
                                                            }
                                                            if (shortDay < 1 || longDay < 1)
                                                            {
                                                                sd = 0;
                                                                ld = 0;
                                                            }
                                                            else
                                                            {
                                                                sd = shortDay;
                                                                ld = longDay;
                                                            }
                                                            Application.DoEvents();
                                                            list.Add(string.Concat(shortTick, '^', sd, '^', longTick, '^', ld, '^', r, '^', h, '^', baseTick, '^', sigma, '^', percent, '^', max, '^', q, '^', t));

                                                            if (list.Count > count * 2)
                                                                break;
                                                        }
                                                        if (list.Count > count * 2)
                                                            break;
                                                    }
                                                    if (list.Count > count * 2)
                                                        break;
                                                }
                                                if (list.Count > count * 2)
                                                    break;
                                            }
                                            if (list.Count > count * 2)
                                                break;
                                        }
                                        if (list.Count > count * 2)
                                            break;
                                    }
                                    if (list.Count > count * 2)
                                        break;
                                }
                                if (list.Count > count * 2)
                                    break;
                            }
                            if (list.Count > count * 2)
                                break;
                        }
                        if (list.Count > count * 2)
                            break;
                    }
                    if (list.Count > count * 2)
                        break;
                }
                if (list.Count > count * 2)
                    break;
            }
            return list.Distinct().ToList();
        }
    }
}