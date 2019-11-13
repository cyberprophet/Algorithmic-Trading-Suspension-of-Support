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
            DayOfWeek dt = DateTime.Now.AddDays(1 - DateTime.Now.Day).DayOfWeek;
            int check = dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 3 : 2;

            return usWeekNumber > check || usWeekNumber == check && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday)) ? DateTime.Now.AddMonths(1).ToString("yyyyMM") : DateTime.Now.ToString("yyyyMM");
        }
        protected string Retention(string code)
        {
            foreach (string retention in code.Substring(0, 3).Equals("101") ? Retrieve.Get().ReadCSV(Array.Find(Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, @"..\"), "*.csv", SearchOption.AllDirectories), o => o.Contains("Tick")), new List<string>(2097152)) : Retrieve.Get().ReadCSV(Array.Find(Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, @"..\"), string.Concat(code, ".csv"), SearchOption.AllDirectories), o => o.Contains(code)), new List<string>(1280)))
                code = retention.Substring(0, 12);

            return code;
        }
        protected readonly IEnumerable[] catalog =
        {
            new Opt50001(),
            new OPW20010(),
            new OPW20007()
        };
        protected const string it = "Information that already Exists";
    }
}