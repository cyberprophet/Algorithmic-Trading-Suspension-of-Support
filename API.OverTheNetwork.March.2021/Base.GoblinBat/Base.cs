using System;
using System.Diagnostics;
using System.Globalization;

namespace ShareInvest
{
    public static class Base
    {
        [Conditional("DEBUG")]
        public static void ChangePropertyToDebugMode() => IsDebug = true;
        public static void SendMessage(string sender, object convey, string message, object param, Type type)
            => Console.WriteLine(string.Concat(type.Name, '_', sender, '_', convey, '_', message, '_', param));
        public static void SendMessage(string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', message));
        [Conditional("DEBUG")]
        public static void SendMessage(Type type, string message) => Debug.WriteLine(string.Concat(type.Name, '_', message));
        [Conditional("DEBUG")]
        public static void SendMessage(Type type, string code, string message) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', message));
        public static void SendMessage(string code, string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', code, '_', message));
        [Conditional("DEBUG")]
        public static void SendMessage(Type type, string code, int status) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', status));
        public static void SendMessage(string code, int status, Type type) => Console.WriteLine(string.Concat(type.Name, '_', code, '_', status));
        public static void SendMessage(DateTime now, string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', now, '_', message));
        public static string GetRemainingTime(TimeSpan span) => span.Days == 0 ?
            string.Concat("장시작 ", span.Hours, "시간 ", span.Minutes, "분 ", span.Seconds, "초 전. . .") :
            string.Concat("장시작 ", span.Days, "일 ", span.Hours, "시간 ", span.Minutes, "분 ", span.Seconds, "초 전. . .");
        public static DateTime IsTheSecondThursday(DateTime now)
        {
            var month = now.AddDays(1 - now.Day);
            var dt = month.DayOfWeek;

            return month.AddDays((dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 2 : 1) * 7 + (DayOfWeek.Thursday - dt));
        }
        public static string[] FindTheNearestQuarter(DateTime now)
        {
            DateTime near;
            var quarter = new string[6];

            switch (now.Month)
            {
                case 1:
                case 4:
                case 7:
                case 0xA:
                    near = IsTheSecondThursday(new DateTime(now.Year, now.AddMonths(2).Month, DateTime.DaysInMonth(now.Year, now.Month)));
                    break;

                case 2:
                case 5:
                case 8:
                case 0xB:
                    near = IsTheSecondThursday(new DateTime(now.Year, now.AddMonths(1).Month, DateTime.DaysInMonth(now.Year, now.Month)));
                    break;

                case 3:
                case 6:
                case 9:
                    near = IsTheSecondThursday(new DateTime(now.Year, now.AddMonths(3).Month, DateTime.DaysInMonth(now.Year, now.Month)));
                    break;

                case 0xC:
                    near = IsTheSecondThursday(new DateTime(now.AddYears(1).Year, now.AddMonths(-9).Month, DateTime.DaysInMonth(now.Year, now.Month)));
                    break;

                default:
                    return null;
            }
            for (int i = 0; i < quarter.Length; i++)
            {
                if (i > 0)
                    near = IsTheSecondThursday(near.AddMonths(3).AddDays(0xB));

                quarter[i] = near.ToString(DateFormat);
            }
            return quarter;
        }
        public static string DistinctDate
        {
            get
            {
                DayOfWeek dt = DateTime.Now.AddDays(1 - DateTime.Now.Day).DayOfWeek;
                int check = dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 3 : 2,
                    usWeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) -
                    CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

                return usWeekNumber > check || usWeekNumber == check && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) ||
                    DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday)) ? DateTime.Now.AddMonths(1).ToString(distinctDate) : DateTime.Now.ToString(distinctDate);
            }
        }
        public static bool IsDebug
        {
            get; private set;
        }
        public static string FullDateFormat => "yyMMddHHmmss";
        public static string DateFormat => "yyMMdd";
        public static string TransactionSuspension => transaction_suspension;
        public static string[] Holidays => new string[] { "201231", "201225", "201009", "201002", "201001", "200930", "200817", "200505", "200501", "200430", "200415" };
        const string distinctDate = "yyyyMM";
        const string transaction_suspension = "거래정지";
    }
}