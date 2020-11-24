using System;
using System.Collections.Generic;

using ShareInvest.Catalog.Models;
using ShareInvest.Interface;

namespace ShareInvest.Statistical.Indicators
{
    public class TrendsToCashflow : Analysis
    {
        public Catalog.Strategics.Statistics StartProgress(double commission)
        {
            Commission = commission;

            return new Catalog.Strategics.Statistics { };
        }
        public override bool Collector
        {
            get; set;
        }
        public override bool Wait
        {
            get; set;
        }
        public override string Code
        {
            get; set;
        }
        public override dynamic Current
        {
            get; set;
        }
        public override dynamic Offer
        {
            get; set;
        }
        public override dynamic Bid
        {
            get; set;
        }
        public override Balance Balance
        {
            get; set;
        }
        public override IStrategics Strategics
        {
            get; set;
        }
        public override Queue<Collect> Collection
        {
            get; set;
        }
        public override void AnalyzeTheConclusion(string[] param) => throw new NotImplementedException();
        public override void AnalyzeTheQuotes(string[] param) => throw new NotImplementedException();
    }
}