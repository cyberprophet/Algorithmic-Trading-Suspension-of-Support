using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ShareInvest.Communication;

namespace ShareInvest.Options
{
    public class Hedge
    {
        public Hedge(IStrategy strategy)
        {
            this.strategy = strategy;
            conclusion = new StringBuilder(64);
        }
        public void Operate(bool check, string time, double price, int quantity)
        {
            DateTime now = GetTime(time);
            string code = string.Empty, date = GetDistinctDate(now, CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now.AddDays(1 - now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            double temp = 0;
            ulong conclusion = ulong.Parse(time);

            if (check == false && strategy.Repository.ContainsKey(date))
            {
                foreach (KeyValuePair<string, Dictionary<ulong, double>> kv in strategy.Repository[date])
                    if (kv.Value.TryGetValue(conclusion, out double close) && (quantity > 0 ? kv.Key.Contains("301") : kv.Key.Contains("201")) && temp < close && price * strategy.MarginRate * strategy.ErrorRate - close > 0)
                    {
                        temp = close;
                        code = kv.Key;
                    }
                this.conclusion.Append(code).Append(';').Append(temp).Append(';');
            }
            else if (check && strategy.Repository.ContainsKey(date))
            {

            }
        }
        private string FindbyCode(string code)
        {
            int temp = int.Parse(code.Substring(5)), point = int.Parse(code.Substring(7));

            return string.Concat(code.Substring(0, 5), code.Substring(0, 3).Equals("201") ? temp + (point == 2 || point == 7 ? 3 : 2) : temp - (point == 2 || point == 7 ? 2 : 3));
        }
        private string GetDistinctDate(DateTime now, int usWeekNumber)
        {
            DayOfWeek dt = now.AddDays(1 - now.Day).DayOfWeek;
            int check = dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 3 : 2;

            return usWeekNumber > check || usWeekNumber == check && (now.DayOfWeek.Equals(DayOfWeek.Friday) || now.DayOfWeek.Equals(DayOfWeek.Saturday)) ? now.AddMonths(1).ToString("yyyyMM") : now.ToString("yyyyMM");
        }
        private DateTime GetTime(string time)
        {
            return new DateTime(2000 + int.Parse(time.Substring(0, 2)), int.Parse(time.Substring(2, 2)), int.Parse(time.Substring(4, 2)), int.Parse(time.Substring(6, 2)), int.Parse(time.Substring(8, 2)), int.Parse(time.Substring(10, 2)));
        }
        private readonly IStrategy strategy;
        private readonly StringBuilder conclusion;
    }
}