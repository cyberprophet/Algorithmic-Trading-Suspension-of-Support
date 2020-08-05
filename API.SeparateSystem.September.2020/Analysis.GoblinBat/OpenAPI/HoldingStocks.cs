using System;
using System.Collections.Generic;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.OpenAPI
{
    public class HoldingStocks : Holding
    {
        internal void OnReceiveTrendsInStockPrices(double gap, double peek)
        {
            switch (strategics)
            {
                case TrendsInStockPrices ts:
                    if (ts.Setting.Equals(Interface.Setting.Short) == false && Current < peek * (1 - ts.AdditionalPurchase) && gap > 0 && OrderNumber.ContainsValue(Current) == false && WaitOrder)
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매수, ts.Code, ts.Quantity, Current, string.Empty)));
                        WaitOrder = false;
                    }
                    else if (ts.Setting.Equals(Interface.Setting.Long) == false && Current > peek * ts.RealizeProfit && gap < 0 && OrderNumber.ContainsValue(Current) == false && WaitOrder)
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매도, ts.Code, ts.Quantity, Current, string.Empty)));
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
            if (long.TryParse(param[9], out long available) && Code.Equals(param[1]) && int.TryParse(param[7], out int purchase) && int.TryParse(param[5], out int current) && int.TryParse(param[6], out int quantity))
            {
                Current = current;
                Quantity = quantity;
                Purchase = purchase;
                Revenue = (current - purchase) * quantity;
                Rate = current / (double)purchase - 1;
                WaitOrder = true;
                SendBalance?.Invoke(this, new SendSecuritiesAPI(available * current));
            }
            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, string, int, dynamic, dynamic, long, double>(param[1], param[4], Quantity, Purchase, Current, Revenue, Rate)));
        }
        public override void OnReceiveConclusion(string[] param)
        {
            if (Code.Equals(param[3]))
            {
                switch (param[5])
                {
                    case conclusion:
                        if (OrderNumber.Remove(param[1]))
                            WaitOrder = false;

                        break;

                    case acceptance:
                        if (param[8].Contains(".") == false && int.TryParse(param[8], out int sPrice))
                            OrderNumber[param[1]] = sPrice;

                        else if (param[8].Contains(".") && double.TryParse(param[8], out double fPrice))
                            OrderNumber[param[1]] = fPrice;

                        WaitOrder = true;
                        break;

                    case confirmation:
                        if ((param[12].Substring(3).Equals(cancellantion) || param[12].Substring(3).Equals(correction)) && OrderNumber.Remove(param[11]))
                            WaitOrder = true;

                        break;
                }
                if (param[19].Contains(".") == false && int.TryParse(param[19], out int sCurrent))
                    Current = sCurrent;

                else if (param[19].Contains(".") && double.TryParse(param[19], out double fCurrent))
                    Current = fCurrent;
            }
        }
        public HoldingStocks(TrendsInStockPrices strategics) : base(strategics)
        {
            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;
            consecutive.Connect(this);
        }
        public HoldingStocks(TrendFollowingBasicFutures strategics) : base(strategics)
        {
            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;

            foreach (var con in Consecutive)
                con.Connect(this);
        }
        readonly dynamic strategics;
        public event EventHandler<SendConsecutive> SendConsecutive;
        public override event EventHandler<SendSecuritiesAPI> SendBalance;
        public override event EventHandler<SendHoldingStocks> SendStocks;
    }
}