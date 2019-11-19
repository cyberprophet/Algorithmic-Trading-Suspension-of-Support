using System;
using System.Collections.Generic;
using System.Globalization;
using ShareInvest.Communication;
using ShareInvest.RetrieveOptions;

namespace ShareInvest.Options
{
    public class Hedge
    {
        public Hedge(IStrategy strategy)
        {
            this.strategy = strategy;
            bal = new List<Balance>(64);
            con = new List<Conclusion>(64);
        }
        public void Operate(bool check, string time, double price, int quantity)
        {
            DateTime now = GetTime(time);
            string date = GetDistinctDate(now, CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);

            if (check == false && strategy.Repository.ContainsKey(date))
                con.Add(new Conclusion(date, time, quantity, price));

            else if (check && strategy.Repository.ContainsKey(date))
                bal.Add(new Balance(date, time, quantity, price));
        }
        public long SettleProfits()
        {
            /*
            foreach (KeyValuePair<string, List<OptionsRepository>> kv in strategy.Repository[date])
            {
                if (kv.Value.Exists(o => o.Date.Equals(time)) && (quantity > 0 ? kv.Key.Contains("301") : kv.Key.Contains("201")))
                {
                    double close = kv.Value.Find(o => o.Date.Equals(time)).Price;

                    if (temp < close && price * strategy.MarginRate * strategy.ErrorRate - close > 0)
                    {
                        temp = close;
                        code = kv.Key;
                    }
                }
            }
            */
            return 0;
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
        private readonly List<Conclusion> con;
        private readonly List<Balance> bal;
        private readonly IStrategy strategy;
    }
}