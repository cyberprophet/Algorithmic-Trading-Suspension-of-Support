using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ShareInvest.Chart
{
    public class Daily : Fetch, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.csv", SearchOption.AllDirectories);

            foreach (string file in Array.FindAll(files, o => o.Contains(type)))
                foreach (string val in ReadCSV(file, new List<string>(256)))
                    yield return val;
        }
        public Daily(int type)
        {
            this.type = type == 0 ? @"DailyChart\Kospi200" : @"DailyChart\Kosdaq150";
        }
        private readonly string type;
    }
}