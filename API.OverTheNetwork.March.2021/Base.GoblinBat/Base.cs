using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace ShareInvest
{
	public static class Base
	{
		public static readonly Dictionary<string, string> rename = new();
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, string code, int quantity, int sell, int buy, double purchase, object current, Interface.IStrategics strategics)
		{
			var str = string.Empty;

			switch (strategics)
			{
				case Catalog.LongPosition lp:
					str = string.Concat('_', new DateTime(lp.Date), '_', lp.Underweight.ToString("P3"), '_', lp.Overweight.ToString("C0"));
					break;

				case Catalog.Scenario scenario:
					str = string.Concat('_', scenario.Date, '_', scenario.Short, '_', scenario.Long);
					break;
			}
			Debug.WriteLine(string.Concat(type.Name, '_', code, '_', quantity, '_', sell, '_', buy, '_', purchase, '_', current, str));
		}
		[Conditional("DEBUG")]
		public static void SendMessage(Type sender, object convey, object message, object param) => Debug.WriteLine(string.Concat(sender.Name, '_', convey, '_', message, '_', param));
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, string code, string date, string remove) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', date, '_', remove));
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, string message) => Debug.WriteLine(string.Concat(type.Name, '_', message));
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, string code, string message) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', message));
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, string code, int status) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', status));
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, object code, object message) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', message));
		[Conditional("DEBUG")]
		public static void SendMessage(string sender, object convey, string message, object param, Type type) => Console.WriteLine(string.Concat(type.Name, '_', sender, '_', convey, '_', message, '_', param));
		[Conditional("DEBUG")]
		public static void SendMessage(string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', message));
		[Conditional("DEBUG")]
		public static void SendMessage(string code, string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', code, '_', message));
		[Conditional("DEBUG")]
		public static void SendMessage(string code, int status, Type type) => Console.WriteLine(string.Concat(type.Name, '_', code, '_', status));
		[Conditional("DEBUG")]
		public static void SendMessage(DateTime now, string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', now, '_', message));
		public static void SendMessage(string name)
		{
			try
			{
				foreach (var process in Process.GetProcessesByName(name))
					process.Kill();
			}
			catch (Exception ex)
			{
				SendMessage(typeof(Base), name, ex.StackTrace);
			}
		}
		public static string GetRemainingTime(TimeSpan span) => span.Days == 0 ? string.Concat("장시작 ", span.Hours, "시간 ", span.Minutes, "분 ", span.Seconds, "초 전. . .") : string.Concat("장시작 ", span.Days, "일 ", span.Hours, "시간 ", span.Minutes, "분 ", span.Seconds, "초 전. . .");
		public static DateTime IsTheSecondThursday(DateTime now)
		{
			var month = now.AddDays(1 - now.Day);
			var dt = month.DayOfWeek;

			return month.AddDays((dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 2 : 1) * 7 + (DayOfWeek.Thursday - dt));
		}
		public static int GetStartingPrice(int price, bool info) => price switch
		{
			int n when n >= 0 && n < 0x3E8 => price,
			int n when n >= 0x3E8 && n < 0x1388 => (price / 5 + 1) * 5,
			int n when n >= 0x1388 && n < 0x2710 => (price / 0xA + 1) * 0xA,
			int n when n >= 0x2710 && n < 0xC350 => (price / 0x32 + 1) * 0x32,
			int n when n >= 0x186A0 && n < 0x7A120 && info => (price / 0x1F4 + 1) * 0x1F4,
			int n when n >= 0x7A120 && info => (price / 0x3E8 + 1) * 0x3E8,
			_ => (price / 0x64 + 1) * 0x64,
		};
		public static int GetQuoteUnit(int price, bool info) => price switch
		{
			int n when n >= 0 && n < 0x3E8 => 1,
			int n when n >= 0x3E8 && n < 0x1388 => 5,
			int n when n >= 0x1388 && n < 0x2710 => 0xA,
			int n when n >= 0x2710 && n < 0xC350 => 0x32,
			int n when n >= 0x186A0 && n < 0x7A120 && info => 0x1F4,
			int n when n >= 0x7A120 && info => 0x3E8,
			_ => 0x64,
		};
		public static ulong MakeKey(ulong index, int type, string code)
		{
			var letter = 1;
			var exist = code.ToCharArray();

			if (Array.Exists(exist, o => char.IsLetter(o)))
			{
				for (int i = 0; i < exist.Length; i++)
					if (char.IsLetter(exist[i]))
						code = code.Replace(exist[i], '0');

				letter = 2;
			}
			return uint.TryParse(code, out uint count) ? index - count - (uint)(type * 0xF4240 * letter) : index;
		}
		public static string CallUpTheChart(Catalog.Models.Codes cm, string start)
		{
			if (string.IsNullOrEmpty(start) && DateTime.TryParseExact(cm.MaturityMarketCap, ConvertDateTime(cm.MaturityMarketCap.Length), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
				return Array.Exists(new string[] { "111Q8000" }, o => o.Equals(cm.Code)) ? unique : date.AddYears(-3).ToString(ConvertDateTime(6));

			return start;
		}
		public static string[] FindTheNearestQuarter(DateTime now)
		{
			DateTime near;
			var quarter = new string[6];

			switch (now.Month)
			{
				case 1 or 4 or 0xA:
					near = IsTheSecondThursday(new DateTime(now.Year, now.AddMonths(2).Month, DateTime.DaysInMonth(now.Year, now.Month)));
					break;

				case 7:
					near = IsTheSecondThursday(new DateTime(now.Year, now.AddMonths(2).Month, DateTime.DaysInMonth(now.Year, now.Month) - 1));
					break;

				case 2 or 0xB:
					near = IsTheSecondThursday(new DateTime(now.Year, now.AddMonths(1).Month, DateTime.DaysInMonth(now.Year, now.Month)));
					break;

				case 5 or 8:
					near = IsTheSecondThursday(new DateTime(now.Year, now.AddMonths(1).Month, DateTime.DaysInMonth(now.Year, now.Month) - 1));
					break;

				case 3:
					near = IsTheSecondThursday(new DateTime(now.Year, now.AddMonths(3).Month, DateTime.DaysInMonth(now.Year, now.Month) - 1));
					break;

				case 6 or 9:
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
		public static string GetOrderNumber(int type)
		{
			if (Count++ > 0x270E)
				Count = 0;

			return string.Concat(type, Count.ToString("D4"));
		}
		public static string CheckTheSAT(string date)
		{
			switch (date.Length)
			{
				case 6:
					var now = DateTime.Now;

					if (CheckIfMarketDelay(now, 1) && now.Hour < 0x12 && uint.TryParse(date, out uint sat))
						return (sat - 0x2710).ToString("D6");

					break;

				case 0xF:
					if (Array.FindIndex(SAT, o => o.Equals(date.Substring(0, 6))) % 2 == 1 && date.Substring(6, 2).CompareTo("18") < 0 && ulong.TryParse(date, out ulong convert))
						return (convert - 0x989680).ToString("D15");

					break;
			}
			return date;
		}
		public static string IsServer(bool server) => server ? "210225" : "210224";
		public static string TellTheClientConnectionStatus(string name, bool is_connected) => $"{name} is connected on {is_connected}";
		public static string ConvertFormat(string account) => $"{account.Substring(0, 4)}­ ─ ­{account.Substring(4, 4)}";
		public static bool CheckIfMarketDelay(DateTime now) => Array.Exists(SAT, o => o.Equals(now.ToString(DateFormat)));
		public static bool CheckIfMarketDelay(DateTime now, int check) => Array.FindIndex(SAT, o => o.Equals(now.ToString(DateFormat))) % 2 == check;
		public static DateTime MeasureTheDelayTime(double delay, DateTime time) => time.AddMilliseconds(delay);
		public static DateTime MeasureTheDelayTime(int delay, DateTime time) => time.AddSeconds(delay);
		public static string GetUrl(string code) => string.Concat(url, code);
		public static string GetUrl(string[] param) => string.Concat(@"https://www.google.com/search?q=", param[0], '+', param[1], '+', param[2], '+', '-', param[^1]);
		public static string ChangeFormat(double param) => Math.Abs(param).ToString("P2");
		public static string ChangeFormat(string date, string format)
		{
			if (DateTime.TryParseExact(date, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime time))
				return TimeFormat.Equals(format) ? time.ToShortTimeString() : time.ToShortDateString();

			return string.Empty;
		}
		public static string Command => "return (window.scrollY + window.innerHeight) / document.body.clientHeight * 100";
		public static string DistinctDate
		{
			get
			{
				var dt = DateTime.Now.AddDays(1 - DateTime.Now.Day).DayOfWeek;
				int check = dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 3 : 2, usWeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

				return usWeekNumber > check || usWeekNumber == check && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday)) ? DateTime.Now.AddMonths(1).ToString(distinctDate) : DateTime.Now.ToString(distinctDate);
			}
		}
		public static bool IsDebug
		{
			get
			{
				ChangePropertyToDebugMode();

				return ToDebug;
			}
		}
		public static ConsoleColor ChangeColor(double param) => param > 0 ? ConsoleColor.Red : ConsoleColor.Blue;
		public static string[] Contents => new[] { "Name", "Rate", "Average", "Major Stocks", "Base Date" };
		public static string[] Title => new[] { "테마명", "전일대비", "최근3일\n등락률", "주도주", "기준일" };
		public static string[] Stocks => new[] { "Code", "Name", "Price", "Rate", "Volume", "Location" };
		public static string[] Explicate => new[] { "종목코드ㆍ체결시각ㆍ경사도", "기업개요", "체결가격", "전일대비\n등락률", "전일대비\n거래증감률", "볼린저밴드（σ :２）" };
		public static double Tax => tax;
		public static string FullDateFormat => "yyMMddHHmmss";
		public static string DateFormat => "yyMMdd";
		public static string LongDateFormat => "yyyyMMdd";
		public static string TimeFormat => "HHmmss";
		public static string TransactionSuspension => transaction_suspension;
		public static string Margin => margin;
		public static string Transmit => transmit;
		public static string Start => start;
		public static string Empty => empty;
		public static string End => end;
		public static string PriceEmpty => price_empty;
		public static string[] Holidays => new[] { "211231", "210922", "210921", "210920", "210519", "210505", "210301", "210212", "210211", "210101", "201231", "201225", "201009", "201002", "201001", "200930", "200817", "200505", "200501", "200430", "200415" };
		public static int Tradable => 0x5910 / 0x19;
		static string ConvertDateTime(int length) => length switch
		{
			6 => DateFormat,
			8 => LongDateFormat,
			_ => null,
		};
		[Conditional("DEBUG")]
		static void ChangePropertyToDebugMode() => ToDebug = true;
		static bool ToDebug
		{
			get; set;
		}
		static uint Count
		{
			get; set;
		}
		static string[] SAT => new[] { "210104", "201203" };
		const string end = "210105";
		const string price_empty = "Empty";
		const string distinctDate = "yyyyMM";
		const string unique = "200611";
		const string start = "0859";
		const string transmit = "1529";
		const string transaction_suspension = "거래정지";
		const string margin = "증거금";
		const string empty = "empty";
		const string url = @"https://finance.naver.com/item/fchart.nhn?code=";
		const double tax = 25e-4 + 15e-5 + 15e-5;
	}
}