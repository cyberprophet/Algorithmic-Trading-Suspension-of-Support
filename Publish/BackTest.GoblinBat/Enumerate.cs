using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ShareInvest.RetrieveInformation;

namespace ShareInvest.BackTest
{
    public class Enumerate : Retrieve, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            string[] arr;

            foreach (string val in Directory.GetDirectories(Environment.CurrentDirectory, @"\Log\"))
            {
                arr = val.Split('\\');
                int recent = int.Parse(arr[arr.Length - 1]);

                if (recent > RecentDate)
                    RecentDate = recent;
            }
            foreach (string file in Directory.GetFiles(string.Concat(Environment.CurrentDirectory, @"\Log\", RecentDate), "*.csv", SearchOption.AllDirectories))
            {
                arr = file.Split('\\');
                arr = arr[arr.Length - 1].Split('.');

                foreach (string val in ReadCSV(file, new List<string>()))
                    yield return string.Concat(arr[0], ",", val);
            }
        }
        private int RecentDate
        {
            get; set;
        }
    }
}