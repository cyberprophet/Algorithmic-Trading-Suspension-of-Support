using System;
using System.Collections.Generic;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.SecondaryIndicators
{
    public class HoldingStocks : Holding
    {
        internal void OnReceiveTrendsInStockPrices(string date, int price, double sShort, double sLong)
        {
            if (date.Substring(6, 4).CompareTo(transmit) > 0)
                SendStocks?.Invoke(this, new SendHoldingStocks(date, price, sShort, sLong));
        }
        public override void OnReceiveBalance(string[] param)
        {

        }
        public override void OnReceiveConclusion(string[] param)
        {

        }
        public override void OnReceiveEvent(string[] param)
        {

        }
        public void StartProgress()
        {
            if (StartProgress(strategics.Code) > 0)
                switch (strategics)
                {
                    case TrendsInStockPrices _:
                        consecutive.Dispose();
                        break;

                    case TrendFollowingBasicFutures _:
                        foreach (var con in Consecutive)
                            con.Dispose();

                        break;
                }
        }
        public override string FindStrategicsCode(string code) => base.FindStrategicsCode(code);
        public HoldingStocks(TrendFollowingBasicFutures strategics) : base(strategics)
        {
            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;
        }
        public HoldingStocks(TrendsInStockPrices strategics) : base(strategics)
        {
            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;
        }
        public override string Code
        {
            get; set;
        }
        public override int Quantity
        {
            get; set;
        }
        public override dynamic Purchase
        {
            get; set;
        }
        public override dynamic Current
        {
            get; set;
        }
        public override dynamic Bid
        {
            get; set;
        }
        public override dynamic Offer
        {
            get; set;
        }
        public override long Revenue
        {
            get; set;
        }
        public override double Rate
        {
            get; set;
        }
        public override double Base
        {
            get; protected internal set;
        }
        public override double Secondary
        {
            get; protected internal set;
        }
        public override bool WaitOrder
        {
            get; set;
        }
        public override dynamic FindStrategics
        {
            get;
        }
        public override Dictionary<string, dynamic> OrderNumber
        {
            get;
        }
        const string transmit = "1529";
        readonly dynamic strategics;
        public override event EventHandler<SendSecuritiesAPI> SendBalance;
        public override event EventHandler<SendHoldingStocks> SendStocks;
    }
}