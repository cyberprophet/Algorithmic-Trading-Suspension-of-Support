using System;

namespace ShareInvest.Indicators
{
    public class BollingerBands
    {
        public BollingerBands(double sigma, int period, int percent, int variable)
        {
            this.percent = percent;
            this.variable = variable * 0.01D;
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
                double standard = StandardDeviation(ema), upper = ema + standard, bottom = ema - standard, width = (upper - bottom) / ema, per = (price - bottom) / (upper - bottom);

                if (per < 0)
                    per = -per + 1;

                if (Before < width && per > 1)
                {
                    Repeat += percent * per * width;
                    Before = width;

                    return over;
                }
                if (per > 0.45 - variable && per < 0.55 + variable)
                    Repeat = 0;

                Before = width;

                return over > Repeat ? over - Repeat : 0;
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
        private double Repeat
        {
            get; set;
        }
        private readonly int period;
        private readonly int percent;
        private readonly double variable;
        private readonly double sigma;
        private readonly double[] standard;
    }
}