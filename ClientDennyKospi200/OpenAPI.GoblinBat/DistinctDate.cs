using System;

namespace ShareInvest.OpenAPI
{
    public class DistinctDate
    {
        protected string GetDistinctDate(int usWeekNumber)
        {
            return (usWeekNumber > 2 || DateTime.Now.DayOfWeek.Equals("Friday") && usWeekNumber == 2) ? DateTime.Now.AddMonths(1).ToString("yyyyMM") : DateTime.Now.ToString("yyyyMM");
        }
    }
}