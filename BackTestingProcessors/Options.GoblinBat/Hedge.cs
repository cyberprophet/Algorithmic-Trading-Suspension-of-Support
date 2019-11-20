using System;
using System.Collections.Generic;
using System.Globalization;
using ShareInvest.Communication;

namespace ShareInvest.Options
{
    public class Hedge
    {
        public long OptionsRevenue
        {
            get; private set;
        }
        public Hedge(IStrategy strategy)
        {
            this.strategy = strategy;
        }
        public void Operate(bool check, string time, double price, int quantity)
        {
            DateTime now = GetTime(time);
            string code = string.Empty, date = GetDistinctDate(now, CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now.AddDays(1 - now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            double temp = 0;

            if (strategy.Repository.ContainsKey(date))
            {
                for (int i = 0; i < strategy.Hedge; i++)
                {
                    if (i == 0)
                    {
                        foreach (KeyValuePair<string, Dictionary<string, double>> kv in strategy.Repository[date])
                            if (kv.Value.TryGetValue(time, out double close) && (quantity > 0 ? kv.Key.Contains("301") : kv.Key.Contains("201")) && temp < close && price * strategy.MarginRate * strategy.ErrorRate - close > 0)
                            {
                                temp = close;
                                code = kv.Key;
                            }
                    }
                    else if (i > 0)
                    {
                        strategy.Repository[date].TryGetValue(code, out Dictionary<string, double> dic);

                        if (dic.TryGetValue(time, out double close) && price * strategy.MarginRate * strategy.ErrorRate - close > 0)
                            temp = close;
                    }
                    if (temp > 0)
                    {
                        if (check == false)
                            OptionsRevenue += (long)(strategy.TransactionMultiplier * temp) + (temp < 0.42 ? (long)(0.0014 * strategy.TransactionMultiplier * temp) + 13 : (long)(0.0015 * strategy.TransactionMultiplier * temp));

                        else if (check)
                            OptionsRevenue -= (long)(strategy.TransactionMultiplier * temp) - (temp < 0.42 ? (long)(0.0014 * strategy.TransactionMultiplier * temp) + 13 : (long)(0.0015 * strategy.TransactionMultiplier * temp));

                        if (i + 2 > strategy.Hedge)
                            break;
                    }
                    if (!code.Equals(string.Empty))
                    {
                        code = FindbyCode(code);
                        temp = 0;
                    }
                }
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
    }
}