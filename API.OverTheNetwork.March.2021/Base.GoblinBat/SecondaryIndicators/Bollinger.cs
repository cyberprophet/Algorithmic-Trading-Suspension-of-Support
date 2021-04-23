using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareInvest.SecondaryIndicators
{
	public static class Bollinger
	{
		public static (double, double, double, double, double) CalculateBands(int current, int[] collection, uint sigma)
		{
			double deviation, standard, upper, lower, total = 0D, average = collection.Average();

			foreach (var cur in collection)
			{
				deviation = cur - average;
				total += deviation * deviation;
			}
			standard = Math.Sqrt(total / collection.Length) * sigma;
			upper = average + standard;
			lower = average - standard;

			return (average, upper, lower, (current - lower) / (upper - lower), (upper - lower) / average);
		}
		public static (double, double) Calculate(Dictionary<int, long> summary)
		{
			var cal = new long[summary.Count];
			long sum = 0L, index = 0L;

			foreach (var kv in summary)
			{
				cal[index++] = kv.Key * kv.Value;
				sum += kv.Value;
			}
			double mean = cal.Sum() / (double)sum, deviation, total = 0D;

			foreach (var kv in summary)
				for (index = 0; index < kv.Value; index++)
				{
					deviation = kv.Key - mean;
					total += deviation * deviation;
				}
			return (mean, Math.Sqrt(total / sum));
		}
	}
}