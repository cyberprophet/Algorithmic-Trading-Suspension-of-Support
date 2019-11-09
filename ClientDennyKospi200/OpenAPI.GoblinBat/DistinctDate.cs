using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ShareInvest.Catalog;
using ShareInvest.RetrieveInformation;

namespace ShareInvest.OpenAPI
{
    public class DistinctDate
    {
        protected string GetDistinctDate(int usWeekNumber)
        {
            return (usWeekNumber > 2 || DateTime.Now.DayOfWeek.Equals("Friday") && usWeekNumber == 2) ? DateTime.Now.AddMonths(1).ToString("yyyyMM") : DateTime.Now.ToString("yyyyMM");
        }
        protected string Retention(string code)
        {
            if (!code.Substring(0, 3).Equals("101"))
            {
                foreach (string retention in Retrieve.Get().ReadCSV(Array.Find(Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, @"..\"), string.Concat(code, ".csv"), SearchOption.AllDirectories), o => o.Contains(code)), new List<string>(256)))
                    code = retention.Substring(0, 12);

                return code;
            }
            foreach (string retention in Retrieve.Get().ReadCSV(Array.Find(Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, @"..\"), "*.csv", SearchOption.AllDirectories), o => o.Contains("Tick")), new List<string>(2097152)))
                code = retention.Substring(0, 12);

            return code;
        }
        protected readonly IEnumerable[] catalog =
        {
            new Opt50001()
        };
        protected const string it = "Information that already Exists";
    }
}