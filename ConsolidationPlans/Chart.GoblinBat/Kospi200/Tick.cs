using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShareInvest.Chart
{
    public class Tick : Fetch, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.csv", SearchOption.AllDirectories), arr;

            foreach (string file in Array.FindAll(files, o => o.Contains(type)))
            {
                list = ReadCSV(file, new List<string>(2097152));

                foreach (string val in list)
                    yield return val;

                arr = list.Last().Split(',');

                Futures.Get().Retention = arr[0];
            }
        }
        public Tick(int type)
        {
            this.type = type == 0 ? @"TickChart\Kospi200" : @"TickChart\Kosdaq150";
        }
        private readonly string type;
    }
}