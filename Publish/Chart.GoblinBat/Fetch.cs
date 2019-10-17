using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ShareInvest.RetrieveInformation;

namespace ShareInvest.Chart
{
    public class Fetch : Retrieve, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (string val in ReadCSV(Array.Find(files, o => o.Contains(chart)), type[chart]))
                yield return val;
        }
        public Fetch(string chart)
        {
            this.chart = chart;
        }
        private readonly Dictionary<string, List<string>> type = new Dictionary<string, List<string>>()
        {
            {"Day", new List<string>(256)},
            {"Tick", new List<string>(2097152)}
        };
        private readonly string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.csv", SearchOption.AllDirectories);
        private readonly string chart;
    }
}