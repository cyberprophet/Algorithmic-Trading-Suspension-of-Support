using System;
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
	}
}