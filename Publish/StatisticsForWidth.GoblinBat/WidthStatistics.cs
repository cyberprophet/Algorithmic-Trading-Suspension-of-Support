using System.Collections.Generic;
using ShareInvest.Chart;
using ShareInvest.SecondaryIndicators;

namespace ShareInvest.StatisticsForWidth
{
    public class WidthStatistics
    {
        public WidthStatistics()
        {
            tick = new List<double>();

            foreach (string val in new Fetch("Tick"))
            {
                string[] arr = val.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                tick.Add(double.Parse(arr[1]));
            }
            MakeTickEMA(tick, new EMA(), new double[2], new Dictionary<string, List<double[]>>(10485760));
        }
        private void MakeTickEMA(List<double> list, EMA ema, double[] temp, Dictionary<string, List<double[]>> dic)
        {
            int i, j, count;

            for (j = 0; j < 100; j++)
                for (i = 0; i < 100; i++)
                {
                    if (j <= i)
                        continue;

                    widthTick = new List<double[]>(2097152);
                    shortTick = new List<double>(2097152);
                    longTick = new List<double>(2097152);

                    foreach (double price in list)
                    {
                        count = shortTick.Count;
                        shortTick.Add(count > 0 ? ema.Make(i, count, price, shortTick[count - 1]) : ema.Make(price));
                        longTick.Add(count > 0 ? ema.Make(j, count, price, longTick[count - 1]) : ema.Make(price));
                        temp[0] = price;
                        temp[1] = shortTick[count] - longTick[count];
                        widthTick.Add(temp);
                    }
                    dic[string.Concat(i.ToString("D2"), j.ToString("D2"))] = widthTick;
                    shortTick = null;
                    longTick = null;
                    widthTick = null;
                }
        }
        private List<double> shortTick;
        private List<double> longTick;
        private List<double[]> widthTick;
        private readonly List<double> tick;
    }
}