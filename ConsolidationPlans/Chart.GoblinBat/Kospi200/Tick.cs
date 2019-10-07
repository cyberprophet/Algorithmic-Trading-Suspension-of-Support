using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShareInvest.Chart.Kospi200
{
    public class Tick : Fetch, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.csv", SearchOption.AllDirectories), arr;

            foreach (string file in Array.FindAll(files, o => o.Contains("TickChart")))
            {
                list = ReadCSV(file, new List<string>(2097152));

                foreach (string val in list)
                    yield return val;

                arr = list.Last().Split(',');

                Futures.Get().Retention = arr[0];
            }
        }
    }
}