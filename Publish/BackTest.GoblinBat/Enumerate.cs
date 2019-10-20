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
            string[] arr, files = Directory.GetFiles(string.Concat(Environment.CurrentDirectory, @"\Log\", DateTime.Now.ToString("yyMMdd")), "*.csv", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                arr = file.Split('\\');
                arr = arr[arr.Length - 1].Split('.');

                foreach (string val in ReadCSV(file, new List<string>()))
                    yield return string.Concat(arr[0], ",", val);
            }
        }
    }
}