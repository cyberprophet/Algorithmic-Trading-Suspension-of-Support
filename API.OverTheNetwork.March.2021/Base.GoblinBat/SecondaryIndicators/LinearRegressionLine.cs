using System;
using System.Linq;

namespace ShareInvest.SecondaryIndicators
{
	public class LinearRegressionLine
	{
		public double Slope => Math.Atan(slope) * (0xB4 / Math.PI);
		public LinearRegressionLine(double[] xs, double[] ys)
		{
			if (xs.Length == ys.Length && xs.Length > 2)
			{
				this.xs = xs;
				this.ys = ys;
				(slope, offset, rate) = GetCoefficients();
			}
		}
		public double GetValueAt(double x) => offset + slope * x + (slope < 0 ? rate : -rate);
		(double, double, double) GetCoefficients()
		{
			double sumXYResidual = 0, sumXSquareResidual = 0, meanX = xs.Average(), meanY = ys.Average(), ssTot = 0, ssRes = 0;

			for (int i = 0; i < xs.Length; i++)
			{
				sumXYResidual += (xs[i] - meanX) * (ys[i] - meanY);
				sumXSquareResidual += (xs[i] - meanX) * (xs[i] - meanX);
			}
			double slope = sumXYResidual / sumXSquareResidual, offset = meanY - slope * meanX;

			for (int i = 0; i < ys.Length; i++)
			{
				double thisY = ys[i], distanceFromMeanSquared = Math.Pow(thisY - meanY, 2), modelY = slope * xs[i] + offset, distanceFromModelSquared = Math.Pow(thisY - modelY, 2);
				ssTot += distanceFromMeanSquared;
				ssRes += distanceFromModelSquared;
			}
			return (slope, offset, Math.Pow(1 - ssRes / ssTot, 2));
		}
		readonly double[] xs;
		readonly double[] ys;
		readonly double slope;
		readonly double offset;
		readonly double rate;
	}
}