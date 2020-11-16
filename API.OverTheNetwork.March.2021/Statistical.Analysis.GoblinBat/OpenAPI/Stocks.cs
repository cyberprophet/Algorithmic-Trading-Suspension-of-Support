using System.Collections.Generic;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical.OpenAPI
{
    public class Stocks : Analysis
    {
        public override void AnalyzeTheConclusion(string[] param)
        {

        }
        public override void AnalyzeTheQuotes(string[] param)
        {

        }
        public override bool Collector
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
        public override dynamic Bid
        {
            get; set;
        }
    }
}