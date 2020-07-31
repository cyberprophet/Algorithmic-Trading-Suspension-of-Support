using System;
using System.Collections.Generic;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.OpenAPI
{
    public class HoldingStocks : Holding
    {
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
            if (int.TryParse(param[1], out int current))
            {
                Current = current;
                Revenue = (current - Purchase) * Quantity;
                Rate = current / (double)Purchase - 1;
            }
            SendStocks?.Invoke(this, new SendHoldingStocks(Code, Quantity, Purchase, Current, Revenue, Rate));
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
            if (Code.Equals(param[3]) && int.TryParse(param[19], out int current))
            {
                switch (param[5])
                {
                    case conclusion:
                        WaitOrder = false;
                        break;

                    case acceptance:
                        WaitOrder = true;
                        break;

                    case confirmation:
                        if (param[12].Substring(3).Equals(cancellantion) || param[12].Substring(3).Equals(correction))
                            WaitOrder = true;

                        break;
                }
                Current = current;
            }
        }
        public HoldingStocks(TrendsInStockPrices strategics) : base(strategics)
        {
            OrderNumber = new Dictionary<string, dynamic>();

            foreach (var con in Consecutive)
                con.Connect(this);
        }
        public HoldingStocks(TrendFollowingBasicFutures strategics) : base(strategics)
        {
            OrderNumber = new Dictionary<string, dynamic>();

            foreach (var con in Consecutive)
                con.Connect(this);
        }
        public new event EventHandler<SendConsecutive> Send;
        public override event EventHandler<SendSecuritiesAPI> SendBalance;
        public override event EventHandler<SendHoldingStocks> SendStocks;
    }
}