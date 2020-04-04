using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShareInvest.EventHandler.BackTesting;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.Strategy
{
    public class BackTesting : CallUpStatisticalAnalysis
    {
        void StartProgress()
        {
            foreach (var quotes in Retrieve.Quotes)
            {
                if (quotes.Price != null && quotes.Volume != null)
                {
                    SendDatum?.Invoke(this, new Datum(quotes.Time, quotes.Price));

                    continue;
                }
                SendQuotes?.Invoke(this, new Quotes(quotes.Time, quotes.SellPrice, quotes.BuyPrice, quotes.SellQuantity, quotes.BuyQuantity, quotes.SellAmount, quotes.BuyAmount));
            }
        }
        internal void Max(double trend, Catalog.XingAPI.Specify specify)
        {
            Judge[specify.Time] = trend;
            double temp = 0;

            foreach (var kv in Judge)
                temp += kv.Value;

            Classification = temp == 0 ? string.Empty : temp > 0 ? Statistics.Analysis.buy : Statistics.Analysis.sell;
        }
        internal void SendClearingOrder(string number)
        {

        }
        internal void SendCorrectionOrder(string price, string number)
        {

        }
        internal void SendNewOrder(string price, string classification)
        {

        }
        internal void SendNewOrder(double price, double max, string classification)
        {

        }
        internal int Quantity
        {
            get; set;
        }
        internal double MaxAmount
        {
            get; set;
        }
        internal string AvgPurchase
        {
            get; set;
        }
        internal string Classification
        {
            get; set;
        }
        internal Dictionary<string, double> BuyOrder
        {
            get;
        }
        internal Dictionary<string, double> SellOrder
        {
            get;
        }
        internal Dictionary<uint, double> Judge
        {
            get;
        }
        public BackTesting(Catalog.XingAPI.Specify[] specifies, string key) : base(key)
        {
            SellOrder = new Dictionary<string, double>();
            BuyOrder = new Dictionary<string, double>();
            Judge = new Dictionary<uint, double>();
            Parallel.ForEach(specifies, new Action<Catalog.XingAPI.Specify>((param) => new Statistics.Analysis(this, param)));
            StartProgress();
        }
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Quotes> SendQuotes;
    }
}