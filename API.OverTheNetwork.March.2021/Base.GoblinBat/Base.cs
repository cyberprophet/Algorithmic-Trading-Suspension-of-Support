using System;
using System.Diagnostics;
using System.Globalization;

namespace ShareInvest
{
    public static class Base
    {
        [Conditional("DEBUG")]
        public static void SendMessage(Type type, string message) => Debug.WriteLine(string.Concat(type.Name, '_', message));
        [Conditional("DEBUG")]
        public static void SendMessage(Type type, string code, string message) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', message));
        [Conditional("DEBUG")]
        public static void SendMessage(Type type, string code, int status) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', status));
        public static string DistinctDate
        {
            get
            {
                DayOfWeek dt = DateTime.Now.AddDays(1 - DateTime.Now.Day).DayOfWeek;
                int check = dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 3 : 2, usWeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

                return usWeekNumber > check || usWeekNumber == check && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday)) ? DateTime.Now.AddMonths(1).ToString(distinctDate) : DateTime.Now.ToString(distinctDate);
            }
        }
        const string distinctDate = "yyyyMM";
    }
}