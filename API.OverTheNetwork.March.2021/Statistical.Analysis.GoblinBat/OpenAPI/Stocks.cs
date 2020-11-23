using System.Collections.Generic;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical.OpenAPI
{
    public class Stocks : Analysis
    {
        public override bool Market
        {
            get; set;
        }
        public override dynamic SellPrice
        {
            protected internal get; set;
        }
        public override dynamic BuyPrice
        {
            protected internal get; set;
        }
        public override int GetQuoteUnit(int price, bool info) => base.GetQuoteUnit(price, info);
        public override int GetStartingPrice(int price, bool info) => base.GetStartingPrice(price, info);
        public override void AnalyzeTheConclusion(string[] param)
        {

        }
        public override void AnalyzeTheQuotes(string[] param)
        {

        }
        public override (IEnumerable<Collect>, uint, uint, string) SortTheRecordedInformation => base.SortTheRecordedInformation;
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
        public override Queue<Collect> Collection
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
        public override Balance Balance
        {
            get; set;
        }
        public override Interface.IStrategics Strategics
        {
            get; set;
        }
        public override dynamic Bid
        {
            get; set;
        }
    }
}