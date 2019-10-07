using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ShareInvest.BackTest
{
    public class Enumerate : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            string[] arr, files = Directory.GetFiles(Environment.CurrentDirectory + @"\Log\" + DateTime.Now.ToString("yyMMdd"), "*.csv", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                arr = file.Split('.');
                arr = arr[0].Split(separator);

                foreach (string val in ReadCSV(file, new List<string>()))
                    yield return string.Concat(arr[arr.Length - 1], ",", val);
            }
        }
        private List<string> ReadCSV(string file, List<string> list)
        {
            try
            {
                using (sr = new StreamReader(file))
                {
                    if (sr != null)
                        while (sr.EndOfStream == false)
                            list.Add(sr.ReadLine());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return list;
        }
        private const char separator = '\\';
        private StreamReader sr;
    }
}