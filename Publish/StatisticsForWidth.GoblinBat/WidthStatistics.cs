using System;
using System.Collections.Generic;
using System.IO;
using ShareInvest.AutoMessageBox;
using ShareInvest.Chart;
using ShareInvest.SecondaryIndicators;

namespace ShareInvest.StatisticsForWidth
{
    public class WidthStatistics
    {
        public WidthStatistics()
        {
            path = string.Concat(Environment.CurrentDirectory, @"\Width\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), @"\");
            tick = new List<double>();

            foreach (string val in new Fetch())
            {
                string[] arr = val.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                tick.Add(double.Parse(arr[1]));
            }
            MakeTickEMA(tick, new EMA());
        }
        private void MakeTickEMA(List<double> list, EMA ema)
        {
            int i, j, count;

            for (j = 0; j < longLength.Length; j++)
                for (i = 0; i < shortLength.Length; i++)
                {
                    List<double[]> widthTick = new List<double[]>(2097152);
                    List<double> shortTick = new List<double>(2097152);
                    List<double> longTick = new List<double>(2097152);

                    foreach (double price in list)
                    {
                        double[] temp = new double[3];
                        count = shortTick.Count;
                        shortTick.Add(count > 0 ? ema.Make(shortLength[i], count, price, shortTick[count - 1]) : ema.Make(price));
                        longTick.Add(count > 0 ? ema.Make(longLength[j], count, price, longTick[count - 1]) : ema.Make(price));
                        temp[0] = price;
                        temp[1] = shortTick[count] - longTick[count];
                        temp[2] = price - shortTick[count];
                        widthTick.Add(temp);
                    }
                    Analysis(string.Concat(i.ToString("D2"), j.ToString("D2")), widthTick);
                }
        }
        private void Analysis(string name, List<double[]> widthTick)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                using StreamWriter sw = new StreamWriter(string.Concat(path, name, ".csv"));
                foreach (double[] tick in widthTick)
                    if (tick[1] != 0)
                        sw.WriteLine(string.Concat(tick[0], ',', tick[1], ',', tick[2]));
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
        }
        private readonly string path;
        private readonly int[] shortLength = { 30, 45, 60 };
        private readonly int[] longLength = { 720, 1440, 2160 };
        private readonly List<double> tick;
    }
}