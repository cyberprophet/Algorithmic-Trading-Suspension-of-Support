using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace ShareInvest
{
	public static class Base
	{
		public static readonly Dictionary<string, string> rename = new Dictionary<string, string>();
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, string message) => Debug.WriteLine(string.Concat(type.Name, '_', message));
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, string code, string message) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', message));
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, string code, int status) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', status));
		[Conditional("DEBUG")]
		public static void SendMessage(Type type, object code, object message) => Debug.WriteLine(string.Concat(type.Name, '_', code, '_', message));
		public static void SendMessage(string sender, object convey, string message, object param, Type type)
			=> Console.WriteLine(string.Concat(type.Name, '_', sender, '_', convey, '_', message, '_', param));
		public static void SendMessage(string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', message));
		public static void SendMessage(Type type, string message, Catalog.OpenAPI.Conclusion conclusion)
		{
			Console.WriteLine(type.FullName);
			Console.WriteLine(message);
			Console.WriteLine(conclusion.Account);
			Console.WriteLine(conclusion.OrderNumber);
			Console.WriteLine(conclusion.AdminNumber);
			Console.WriteLine(conclusion.Code);
			Console.WriteLine(conclusion.OrderBusinessClassification);
			Console.WriteLine(conclusion.OrderState);
			Console.WriteLine(conclusion.Name);
			Console.WriteLine(conclusion.OrderQuantity);
			Console.WriteLine(conclusion.OrderPrice);
			Console.WriteLine(conclusion.UnsettledQuantity);
			Console.WriteLine(conclusion.TotalExecutionAmount);
			Console.WriteLine(conclusion.OriginalOrderNumber);
			Console.WriteLine(conclusion.OrderClassification);
			Console.WriteLine(conclusion.SalesClassification);
			Console.WriteLine(conclusion.TradingClassification);
			Console.WriteLine(conclusion.Time);
			Console.WriteLine(conclusion.ConclusionNumber);
			Console.WriteLine(conclusion.ConclusionPrice);
			Console.WriteLine(conclusion.ConclusionQuantity);
			Console.WriteLine(conclusion.CurrentPrice);
			Console.WriteLine(conclusion.OfferPrice);
			Console.WriteLine(conclusion.BidPrice);
			Console.WriteLine(conclusion.UnitConclusionPrice);
			Console.WriteLine(conclusion.UnitConclusionQuantity);
			Console.WriteLine(conclusion.Commission);
			Console.WriteLine(conclusion.Tax);
			Console.WriteLine(conclusion.ReasonForRejection);
			Console.WriteLine(conclusion.ScreenNumber);
			Console.WriteLine(conclusion.TerminalNumber);
			Console.WriteLine(conclusion.CreditClassification);
			Console.WriteLine(conclusion.LoanDate);
		}
		public static void SendMessage(Type type, string message, Catalog.OpenAPI.Balance balance)
		{
			Console.WriteLine(type.FullName);
			Console.WriteLine(message);
			Console.WriteLine(string.Concat("계좌번호_", balance.Account));
			Console.WriteLine(string.Concat("종목코드_업종코드_", balance.Code));
			Console.WriteLine(string.Concat("신용구분_", balance.CreditClassification));
			Console.WriteLine(string.Concat("대출일_", balance.LoanDate));
			Console.WriteLine(string.Concat("종목명_", balance.Name));
			Console.WriteLine(string.Concat("현재가_", balance.Current));
			Console.WriteLine(string.Concat("보유수량_", balance.Quantity));
			Console.WriteLine(string.Concat("매입단가_", balance.Purchase));
			Console.WriteLine(string.Concat("총매입가_", balance.TotalPurchasePrice));
			Console.WriteLine(string.Concat("주문가능수량_", balance.QuantityAvailable));
			Console.WriteLine(string.Concat("당일순매수량_", balance.NetPurchaseOnTheDay));
			Console.WriteLine(string.Concat("매도_매수구분_", balance.TradingClassification));
			Console.WriteLine(string.Concat("당일총매도손익_", balance.TotalSalesOnTheDay));
			Console.WriteLine(string.Concat("예수금_", balance.Deposit));
			Console.WriteLine(string.Concat("매도호가_", balance.Offer));
			Console.WriteLine(string.Concat("매수호가_", balance.Bid));
			Console.WriteLine(string.Concat("기준가_", balance.ReferencePrice));
			Console.WriteLine(string.Concat("손익율_", balance.Rate));
			Console.WriteLine(string.Concat("신용금액_", balance.CreditAmount));
			Console.WriteLine(string.Concat("신용이자_", balance.CreditInterest));
			Console.WriteLine(string.Concat("만기일_", balance.ExpirationDate));
			Console.WriteLine(string.Concat("당일실현손익_유가_", balance.RealizedOnTheDay));
			Console.WriteLine(string.Concat("당일실현손익률_유가_", balance.RealizedRateOnTheDay));
			Console.WriteLine(string.Concat("당일실현손익_신용_", balance.RealizedOnTheDayCredit));
			Console.WriteLine(string.Concat("당일실현손익률_신용_", balance.RealizedRateOnTheDayCredit));
			Console.WriteLine(string.Concat("담보대출수량_", balance.LoanQuantity));
			Console.WriteLine(string.Concat("ExtraItem_", balance.ExtraItem));
		}
		public static void SendMessage(string code, string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', code, '_', message));
		public static void SendMessage(string code, int status, Type type) => Console.WriteLine(string.Concat(type.Name, '_', code, '_', status));
		public static void SendMessage(DateTime now, string message, Type type) => Console.WriteLine(string.Concat(type.Name, '_', now, '_', message));
		public static void SendMessage(string name)
		{
			foreach (var process in Process.GetProcessesByName(name))
				process.Kill();
		}
		public static string GetRemainingTime(TimeSpan span) => span.Days == 0 ?
			string.Concat("장시작 ", span.Hours, "시간 ", span.Minutes, "분 ", span.Seconds, "초 전. . .") :
			string.Concat("장시작 ", span.Days, "일 ", span.Hours, "시간 ", span.Minutes, "분 ", span.Seconds, "초 전. . .");
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

					if (Array.Exists(SAT, o => o.Equals(now.ToString(DateFormat))) && now.Hour < 0x12 && uint.TryParse(date, out uint sat))
						return (sat - 0x2710).ToString("D6");

					break;

				case 0xF:
					if (Array.Exists(SAT, o => o.Equals(date.Substring(0, 6))) && date.Substring(6, 2).CompareTo("18") < 0 && ulong.TryParse(date, out ulong convert))
						return (convert - 0x989680).ToString("D15");

					break;
			}
			return date;
		}
		public static bool CheckIfMarketDelay(DateTime now) => Array.Exists(SAT, o => o.Equals(now.ToString(DateFormat)));
		public static DateTime MeasureTheDelayTime(double delay, DateTime time) => time.AddMilliseconds(delay);
		public static DateTime MeasureTheDelayTime(int delay, DateTime time) => time.AddSeconds(delay);
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
			get
			{
				ChangePropertyToDebugMode();

				return ToDebug;
			}
		}
		public static double Tax => tax;
		public static string FullDateFormat => "yyMMddHHmmss";
		public static string DateFormat => "yyMMdd";
		public static string LongDateFormat => "yyyyMMdd";
		public static string TransactionSuspension => transaction_suspension;
		public static string Transmit => transmit;
		public static string Start => start;
		public static string[] Holidays => new[] { "201231", "201225", "201009", "201002", "201001", "200930", "200817", "200505", "200501", "200430", "200415" };
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
		static string[] SAT => new[] { "201203" };
		const string distinctDate = "yyyyMM";
		const string unique = "200611";
		const string start = "0859";
		const string transmit = "1529";
		const string transaction_suspension = "거래정지";
		const double tax = 2.5e-3;
	}
}