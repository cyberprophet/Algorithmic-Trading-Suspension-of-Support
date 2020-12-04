using System;
using System.Collections.Generic;
using System.Linq;

using ShareInvest.SecondaryIndicators;

namespace ShareInvest
{
	public static class Strategics
	{
		public static long Cash
		{
			get; set;
		}
		internal static string SetOrder(string code, int type, int price, int quantity, string hoga, string number)
			 => string.Concat(code, ';', type, ';', price, ';', quantity, ';', hoga, ';', number);
		internal static IEnumerable<Interface.IStrategics> SetStrategics(string[] param)
		{
			foreach (var str in param)
				if (str[2] == '|')
				{
					var cs = str.Split('|');

					if (Enum.TryParse(cs[0], out Interface.Strategics strategics))
						switch (strategics)
						{
							case Interface.Strategics.SC:
								if (cs.Length == 0x11
									&& int.TryParse(cs[2], out int vShort) && int.TryParse(cs[3], out int vLong) && int.TryParse(cs[4], out int vTrend)
									&& int.TryParse(cs[5], out int su) && int.TryParse(cs[6], out int sq) && double.TryParse(cs[7], out double vSubtraction)
									&& int.TryParse(cs[8], out int au) && int.TryParse(cs[9], out int aq) && double.TryParse(cs[0xA], out double vAddition)
									&& int.TryParse(cs[0xB], out int si) && int.TryParse(cs[0xC], out int tsq) && double.TryParse(cs[0xD], out double sp)
									&& int.TryParse(cs[0xE], out int ai) && int.TryParse(cs[0xF], out int taq) && double.TryParse(cs[0x10], out double ap))
									yield return new Catalog.SatisfyConditionsAccordingToTrends
									{
										Code = cs[1],
										Short = vShort,
										Long = vLong,
										Trend = vTrend,
										ReservationSellUnit = su,
										ReservationSellQuantity = sq,
										ReservationSellRate = vSubtraction * 1e-2,
										ReservationBuyUnit = au,
										ReservationBuyQuantity = aq,
										ReservationBuyRate = vAddition * 1e-2,
										TradingSellInterval = si * 1e+3,
										TradingSellQuantity = tsq,
										TradingSellRate = sp * 1e-2,
										TradingBuyInterval = ai * 1e+3,
										TradingBuyQuantity = taq,
										TradingBuyRate = ap * 1e-2
									};
								break;
						}
				}
		}
		internal static Dictionary<DateTime, double> AnalyzeFinancialStatements(List<Catalog.FinancialStatement> statement, char[] analysis)
		{
			List<long> sale = new List<long>(), oper = new List<long>(), netincome = new List<long>(), flow = new List<long>();
			var financial_statement = new Dictionary<DateTime, Tuple<long, long, long, long>>();
			var dictionary = new Dictionary<DateTime, double>();
			var list = new List<long>[] { sale, oper, netincome, flow };
			var count = 0;
			var now = DateTime.Now;

			foreach (var fs in statement)
			{
				long sales = 0, operation = 0, net = 0, cash = 0;
				var date = fs.Date.Substring(0, 5).Split('.');

				for (int i = 0; i < analysis.Length; i++)
					if (analysis[i].Equals('T'))
						switch (i)
						{
							case 0:
								sales = long.TryParse(fs.Revenues, out long revenues) ? revenues : long.MinValue;
								break;

							case 1:
								operation = long.TryParse(fs.IncomeFromOperations, out long operations) ? operations : long.MinValue;
								break;

							case 2:
								net = long.TryParse(fs.NetIncome, out long income) ? income : long.MinValue;
								break;

							case 3:
								cash = long.TryParse(fs.OperatingActivities, out long activities) ? activities : long.MinValue;
								break;

							default:
								continue;
						}
				if (int.TryParse(date[0], out int year) && int.TryParse(date[1], out int month))
					financial_statement[Base.IsTheSecondThursday(new DateTime(0x7D0 + year, month, DateTime.DaysInMonth(year + 0x7D0, month), 0xF, 0x1E, 0))]
						= new Tuple<long, long, long, long>(sales, operation, net, cash);
			}
			if (financial_statement.Count > 0)
			{
				foreach (var kv in financial_statement.OrderBy(o => o.Key))
				{
					if (analysis[0].Equals('T'))
						sale.Add(kv.Value.Item1);

					if (analysis[1].Equals('T'))
						oper.Add(kv.Value.Item2);

					if (analysis[2].Equals('T'))
						netincome.Add(kv.Value.Item3);

					if (analysis[3].Equals('T'))
						flow.Add(kv.Value.Item4);
				}
				for (int i = 0; i < analysis.Length; i++)
					if (analysis[i].Equals('T') && list[i].Count > 0)
						count++;

				foreach (var item in list)
					if (item.Count > 0 && item.TrueForAll(o => o == 0 || o == long.MinValue) == false)
					{
						var normal = new Normalization(item);
						var index = 0;

						foreach (var kv in financial_statement.OrderBy(o => o.Key))
						{
							if (item[index] > long.MinValue)
							{
								if (dictionary.TryGetValue(kv.Key, out double normalize))
									dictionary[kv.Key] = normalize + normal.Normalize(item[index]) / count;

								else
									dictionary[kv.Key] = normal.Normalize(item[index]) / count;
							}
							index++;
						}
					}
				if (dictionary.Count > 4 && dictionary.Any(o => o.Key.Year == now.AddYears(now.Month > 3 ? 2 : 1).Year) == false)
				{
					var temp = EstimateThePrice(dictionary, now.AddYears(3).Year);
					var normal = new Normalization(temp);
					dictionary.Clear();

					foreach (var kv in temp.OrderBy(o => o.Key))
						if (kv.Value > normal.Min && kv.Value < normal.Max)
							dictionary[kv.Key] = normal.Normalize(kv.Value);
				}
				if (dictionary.Count > 4 && dictionary.Any(o => o.Key.Year == now.AddYears(now.Month > 3 ? 2 : 1).Year))
					try
					{
						return EstimateThePrice(dictionary, now);
					}
					catch (Exception ex)
					{
						Base.SendMessage(ex.StackTrace, typeof(Strategics));
					}
			}
			return null;
		}		
		static Dictionary<DateTime, double> EstimateThePrice(Dictionary<DateTime, double> estimate, int year)
		{
			var dictionary = new Dictionary<DateTime, double>(estimate);
			List<double> xs = new List<double>(), ys = new List<double>();

			foreach (var kv in estimate.OrderBy(o => o.Key))
				if (kv.Value > 0 && kv.Value < 1)
				{
					ys.Add(kv.Value);
					xs.Add(correct * kv.Key.Ticks);
				}
			var regression = new LinearRegressionLine(xs.ToArray(), ys.ToArray());
			var last = estimate.Last().Key;

			while (dictionary.Last().Key.Year < year)
			{
				last = last.AddMonths(6);
				dictionary[last] = regression.GetValueAt(correct * last.Ticks);
			}
			return dictionary;
		}
		static Dictionary<DateTime, double> EstimateThePrice(Dictionary<DateTime, double> estimate, DateTime now)
		{
			var dictionary = new Dictionary<DateTime, double>(estimate);
			double[] xs = new double[estimate.Count], ys = new double[estimate.Count];
			int index = 0;

			foreach (var kv in estimate.OrderBy(o => o.Key))
			{
				ys[index] = kv.Value;
				xs[index++] = correct * kv.Key.Ticks;
			}
			var interpolate = new PeriodicSpline(xs, ys, 0x5A1).Integrate().Interpolate();

			for (index = 0; index < interpolate.Item1.Length; index++)
			{
				var time = new DateTime((long)(revert * interpolate.Item1[index]));

				if (dictionary.ContainsKey(time) == false && now.AddDays(now.Hour < 0x11 ? -6 : -5).CompareTo(time) < 0)
					dictionary[time] = interpolate.Item2[index];
			}
			return dictionary;
		}
		const long revert = 0xE8D4A51000;
		const double correct = 1e-12;
	}
}