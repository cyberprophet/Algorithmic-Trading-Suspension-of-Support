using System;
using System.Linq;

namespace ShareInvest.Secondary
{
    public class BollingerBands
    {
        public BollingerBands(int mid, int sigma)
        {
            MidPeriod = mid;
            Sigma = sigma;
        }
        public double StandardDeviation(int period, double sma, double[] standard_deviation)
        {
            int i;
            double total = 0;

            for (i = 0; i < period; i++)
                total += Math.Pow(Math.Abs(standard_deviation[i] - sma), Sigma);

            return Math.Sqrt(total / period);
        }
        public double MovingAverage(int period, double[] sma)
        {
            return sma.Sum() / period;
        }
        public double Width(double sma, double upper, double bottom)
        {
            return (upper - bottom) / sma;
        }
        public double Percent(double price, double upper, double bottom)
        {
            return (price - bottom) / (upper - bottom) * 100;
        }
        public double BottomLine(double sma, double sd)
        {
            return sma - Sigma * sd;
        }
        public double UpperLimit(double sma, double sd)
        {
            return sma + Sigma * sd;
        }
        public int MidPeriod
        {
            get; private set;
        }
        private int Sigma
        {
            get; set;
        }
    }
}