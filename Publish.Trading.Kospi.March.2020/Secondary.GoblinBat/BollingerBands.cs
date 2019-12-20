using System;

namespace ShareInvest.Secondary
{
    public class BollingerBands
    {
        public BollingerBands(double sigma, int period, double percent, double max)
        {
            this.max = max;
            this.percent = new double[2] { 1 - percent, percent };
            this.sigma = sigma;
            this.period = period;
            standard = new double[period];
        }
        public double GetJudgingOverHeating(double over, double price, double ema)
        {
            standard[Count++] = ema;

            if (Count > period - 1)
            {
                Count = 0;
                Fill = true;
            }
            if (Fill)
            {
                double temp, standard = StandardDeviation(ema), upper = ema + standard, bottom = ema - standard, width = (upper - bottom) / ema, per = (price - bottom) / (upper - bottom);

                if ((per - percent[1] > 0 || per - percent[0] < 0) && width > Before)
                    repeat = repeat > 0 ? repeat-- : 0;

                else if (per - percent[1] < 0 && per - percent[0] > 0 && width < Before)
                    repeat++;

                Before = width;
                temp = max * repeat;
                repeat = over > temp ? repeat : 0;

                return repeat > 0 ? over - temp : 0;
            }
            return 0;
        }
        private double StandardDeviation(double ema)
        {
            int i;
            double total = 0, deviation;

            for (i = 0; i < period; i++)
            {
                deviation = standard[i] - ema;
                total += deviation * deviation;
            }
            return Math.Sqrt(total / period) * sigma;
        }
        private uint Count
        {
            get; set;
        }
        private bool Fill
        {
            get; set;
        }
        private double Before
        {
            get; set;
        }
        private uint repeat;
        private readonly int period;
        private readonly double max;
        private readonly double sigma;
        private readonly double[] percent;
        private readonly double[] standard;
    }
}