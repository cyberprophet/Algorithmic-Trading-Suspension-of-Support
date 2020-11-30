using System;
using System.Collections.Generic;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
    public class Options : Analysis
    {
        public override void OnReceiveDrawChart(object sender, SendConsecutive e)
        {

        }
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
        public override double Capital
        {
            get; protected internal set;
        }
        public override Balance Balance
        {
            get; set;
        }
        public override Interface.IStrategics Strategics
        {
            get; set;
        }
        public override Queue<Collect> Collection
        {
            get; set;
        }
        public override Dictionary<string, dynamic> OrderNumber
        {
            get; set;
        }
        protected internal override Stack<double> Short
        {
            get; set;
        }
        protected internal override Stack<double> Long
        {
            get; set;
        }
        protected internal override Stack<double> Trend
        {
            get; set;
        }
        protected internal override Tuple<int, int, int> Line
        {
            get; set;
        }
        protected internal override DateTime NextOrderTime
        {
            get; set;
        }
        protected internal override string DateChange
        {
            get; set;
        }
        protected internal override bool GetCheckOnDate(string date)
        {
            return true;
        }
        protected internal override bool GetCheckOnDeadline(string time)
        {
            return true;
        }
    }
}