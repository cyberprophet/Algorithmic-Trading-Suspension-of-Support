using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ShareInvest.Communication;

namespace ShareInvest.Options
{
    public class Hedge
    {
        public long OptionsRevenue
        {
            get; private set;
        }
        public Hedge(IStrategy strategy, Dictionary<string, uint> balance)
        {
            this.strategy = strategy;
            this.balance = balance;
        }
        public void Operate(bool check, string time, double price, int quantity)
        {
            DateTime now = GetTime(time);
            string code = string.Empty, date = GetDistinctDate(now, CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now.AddDays(1 - now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            double temp = 0;

            if (!date.Equals(Date))
            {
                Date = date;
                balance.Clear();
            }
            if (strategy.Repository.ContainsKey(date))
            {
                foreach (KeyValuePair<string, Dictionary<string, double>> kv in strategy.Repository[date])
                    if (kv.Value.TryGetValue(time, out double close) && (quantity > 0 ? kv.Key.Contains("301") : kv.Key.Contains("201")) && temp < close && price * strategy.MarginRate * rate[strategy.Hedge] - close > 0)
                    {
                        temp = close;
                        code = FindbyCode(kv.Key);
                    }
                temp = 0;

                if (strategy.Repository[date].TryGetValue(code, out Dictionary<string, double> find))
                {
                    foreach (KeyValuePair<string, double> kv in find)
                        if (time.Contains(kv.Key) && temp < kv.Value && price * strategy.MarginRate * rate[strategy.Hedge] - kv.Value > 0)
                            temp = kv.Value;

                    if (temp > 0 && check == false)
                    {
                        OptionsRevenue += (long)(strategy.TransactionMultiplier * temp) + (temp < 0.42 ? (long)(0.0014 * strategy.TransactionMultiplier * temp) + 13 : (long)(0.0015 * strategy.TransactionMultiplier * temp));
                        balance[code] = balance.ContainsKey(code) ? balance[code] + 1 : 1;
                    }
                    else if (check && balance.Count > 0)
                    {
                        uint amount = 0;

                        foreach (KeyValuePair<string, uint> kv in balance)
                            if (kv.Value > amount)
                            {
                                amount = kv.Value;
                                code = kv.Key;
                            }
                        balance[code] = balance[code] - 1;

                        if (strategy.Repository[date].TryGetValue(code, out Dictionary<string, double> revenue))
                        {
                            temp = FindRevenue(revenue, time, 12);
                            OptionsRevenue -= (long)(strategy.TransactionMultiplier * temp) - (temp < 0.42 ? (long)(0.0014 * strategy.TransactionMultiplier * temp) + 13 : (long)(0.0015 * strategy.TransactionMultiplier * temp));
                        }
                    }
                    while (balance.ContainsValue(0))
                        balance.Remove(balance.First(o => o.Value < 1).Key);
                }
            }
        }
        private double FindRevenue(Dictionary<string, double> revenue, string time, int index)
        {
            double temp = revenue.FirstOrDefault(o => o.Key.Substring(0, index).Equals(time.Substring(0, index))).Value;

            if (temp == 0)
                return FindRevenue(revenue, time, index - 1);

            return temp;
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
        private string Date
        {
            get; set;
        }
        private readonly Dictionary<int, double> rate = new Dictionary<int, double>()
        {
            {0, 0 },
            {1, 0.05 },
            {2, 0.1 },
            {3, 0.135 },
            {4, 0.17 },
            {5, 0.205 }
        };
        private readonly Dictionary<string, uint> balance;
        private readonly IStrategy strategy;
    }
}