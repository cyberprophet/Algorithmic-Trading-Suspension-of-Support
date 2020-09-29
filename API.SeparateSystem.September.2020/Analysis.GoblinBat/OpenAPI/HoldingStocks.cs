using System;
using System.Collections.Generic;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.OpenAPI
{
    public class HoldingStocks : Holding
    {
        internal void OnReceiveTrendsInPrices(double gap, double peek)
        {
            switch (strategics)
            {
                case TrendsInStockPrices ts:
                    if (ts.Setting.Equals(Interface.Setting.Short) == false && Bid < peek * (1 - ts.AdditionalPurchase) && gap > 0 && OrderNumber.ContainsValue(Bid) == false && WaitOrder)
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매수, ts.Code, ts.Quantity, Bid, string.Empty)));
                        WaitOrder = false;
                    }
                    else if (ts.Setting.Equals(Interface.Setting.Long) == false && Offer > peek * (1 + ts.RealizeProfit) && Offer > Purchase && gap < 0 && OrderNumber.ContainsValue(Offer) == false && WaitOrder)
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매도, ts.Code, ts.Quantity, Offer, string.Empty)));
                        WaitOrder = false;
                    }
                    break;

                case TrendToCashflow tc:
                    if (tc.TradingQuantity > 0 && Bid < peek * (1 - tc.PositionAddition) && gap > 0 && OrderNumber.ContainsValue(Bid) == false && WaitOrder)
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매수, tc.Code, tc.TradingQuantity, Bid, string.Empty)));
                        WaitOrder = false;
                    }
                    else if (tc.TradingQuantity > 0 && Offer > peek * (1 + tc.PositionRevenue) && Offer > Purchase && gap < 0 && OrderNumber.ContainsValue(Offer) == false && WaitOrder)
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매도, tc.Code, tc.TradingQuantity, Offer, string.Empty)));
                        WaitOrder = false;
                    }
                    break;
            }
            Base = peek;
            Secondary = gap;
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
        public override Dictionary<string, dynamic> OrderNumber
        {
            get;
        }
        public override void OnReceiveEvent(string[] param)
        {
            if (int.TryParse(param[6], out int volume))
                SendConsecutive?.Invoke(this, new SendConsecutive(new Charts
                {
                    Date = param[0],
                    Price = param[1].StartsWith("-") ? param[1].Substring(1) : param[1],
                    Volume = volume
                }));
            if (int.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out int current))
            {
                Current = current;
                Revenue = (current - Purchase) * Quantity;
                Rate = current / (double)Purchase - 1;
            }
            SendStocks?.Invoke(this, new SendHoldingStocks(Code, Quantity, Purchase, Current, Revenue, Rate, Base, Secondary, AdjustTheColorAccordingToTheCurrentSituation(WaitOrder, OrderNumber.Count)));
        }
        public override void OnReceiveBalance(string[] param)
        {
            if (long.TryParse(param[9], out long available) && int.TryParse(param[7], out int purchase) && int.TryParse(param[5].StartsWith("-") ? param[5].Substring(1) : param[5], out int current) && int.TryParse(param[6], out int quantity))
            {
                Current = current;
                Quantity = quantity;
                Purchase = purchase;
                Revenue = (current - purchase) * quantity;
                Rate = current / (double)purchase - 1;
                WaitOrder = true;
                SendBalance?.Invoke(this, new SendSecuritiesAPI(available * current));
            }
            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, string, int, dynamic, dynamic, long, double>(param[1].StartsWith("A") ? param[1].Substring(1) : param[1], param[4], Quantity, Purchase, Current, Revenue, Rate)));
        }
        public override void OnReceiveConclusion(string[] param)
        {
            switch (param[5])
            {
                case conclusion:
                    if (OrderNumber.Remove(param[1]))
                        WaitOrder = false;

                    break;

                case acceptance when string.Compare(param[9], "0") > 0:
                    if (param[8].Contains(".") == false && int.TryParse(param[8], out int sPrice) && sPrice > 0)
                        OrderNumber[param[1]] = sPrice;

                    else if (param[8].Contains(".") && double.TryParse(param[8], out double fPrice) && fPrice > 0)
                        OrderNumber[param[1]] = fPrice;

                    WaitOrder = true;
                    break;

                case confirmation when param[12].EndsWith(cancellantion) || param[12].EndsWith(correction):
                    WaitOrder = OrderNumber.Remove(param[11]);
                    break;
            }
            if (param[19].Contains(".") == false && int.TryParse(param[19].StartsWith("-") ? param[19].Substring(1) : param[19], out int sCurrent))
                Current = sCurrent;

            else if (param[19].Contains(".") && double.TryParse(param[19].StartsWith("-") ? param[19].Substring(1) : param[19], out double fCurrent))
                Current = fCurrent;
        }
        public override int GetQuoteUnit(int price, bool info) => base.GetQuoteUnit(price, info);
        public override int GetStartingPrice(int price, bool info) => base.GetStartingPrice(price, info);
        public override dynamic FindStrategics => strategics;
        public override dynamic Bid
        {
            get; set;
        }
        public override dynamic Offer
        {
            get; set;
        }
        protected internal override bool Market
        {
            get; set;
        }
        protected internal override DateTime NextOrderTime
        {
            get; set;
        }
        public HoldingStocks(TrendToCashflow strategics) : base(strategics)
        {
            if (StartProgress(strategics.Code) > 0)
                consecutive.Dispose();

            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;
            consecutive.Connect(this);
        }
        public HoldingStocks(TrendsInStockPrices strategics) : base(strategics)
        {
            if (StartProgress(strategics.Code) > 0)
                consecutive.Dispose();

            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;
            consecutive.Connect(this);
        }
        public HoldingStocks(TrendFollowingBasicFutures strategics) : base(strategics)
        {
            if (StartProgress(strategics.Code) > 0)
                foreach (var con in Consecutive)
                    con.Dispose();

            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;

            foreach (var con in Consecutive)
                con.Connect(this);
        }
        internal override Dictionary<DateTime, double> EstimatedPrice
        {
            get; set;
        }
        readonly dynamic strategics;
        public event EventHandler<SendConsecutive> SendConsecutive;
        public override event EventHandler<SendSecuritiesAPI> SendBalance;
        public override event EventHandler<SendHoldingStocks> SendStocks;
    }
}