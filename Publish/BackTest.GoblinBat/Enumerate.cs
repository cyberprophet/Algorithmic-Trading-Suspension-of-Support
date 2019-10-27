using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ShareInvest.AutoMessageBox;

namespace ShareInvest.BackTest
{
    public class Enumerate : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            string[] arr;

            try
            {
                foreach (string val in Directory.GetDirectories(string.Concat(Environment.CurrentDirectory, @"\Log\")))
                {
                    arr = val.Split('\\');
                    int recent = int.Parse(arr[arr.Length - 1]);

                    if (recent > RecentDate)
                        RecentDate = recent;
                }
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
            foreach (string file in Directory.GetFiles(string.Concat(Environment.CurrentDirectory, @"\Log\", RecentDate), "*.csv", SearchOption.AllDirectories))
            {
                arr = file.Split('\\');
                arr = arr[arr.Length - 1].Split('.');

                foreach (string val in ReadCSV(file, new List<string>()))
                    yield return string.Concat(arr[0], ",", val);
            }
        }
        private List<string> ReadCSV(string file, List<string> list)
        {
            try
            {
                using StreamReader sr = new StreamReader(file);
                if (sr != null)
                    while (sr.EndOfStream == false)
                        list.Add(sr.ReadLine());
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
            return list;
        }
        private int RecentDate
        {
            get; set;
        }
    }
}