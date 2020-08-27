using System;
using System.Collections.Generic;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.XingAPI
{
    public class HoldingStocks : Holding
    {
        internal void OnReceiveTrendsInPrices(double gap, int minute)
        {
            if (minute == 0x5A0)
                Base = gap;

            else
                Secondary = gap;
        }
        public override void OnReceiveBalance(string[] param)
        {
            var cme = param.Length > 0x1C;

            if (param[cme ? 0x33 : 0xB].Length == 8 && int.TryParse(param[cme ? 0x53 : 0xE], out int quantity) && double.TryParse(param[cme ? 0x52 : 0xD], out double current) && int.TryParse(param[cme ? 0x2D : 9], out int number) && OrderNumber.Remove(number.ToString()))
            {
                var gb = param[cme ? 0x37 : 0x14];
                Current = current;
                Purchase = gb.Equals("2") && Quantity >= 0 ? (Purchase * Quantity + current * quantity) / (quantity + Quantity) : (gb.Equals("1") && Quantity <= 0 ? (current * quantity - Purchase * Quantity) / (quantity - Quantity) : Purchase);
                Quantity += gb.Equals("1") ? -quantity : quantity;
                Revenue = (long)(current - Purchase) * Quantity * transactionMutiplier;
                Rate = (Quantity > 0 ? current / (double)Purchase : Purchase / (double)current) - 1;
            }
            WaitOrder = true;
            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, string, int, dynamic, dynamic, long, double>(param[cme ? 0x33 : 0xB], param[cme ? 0x34 : 0xB], Quantity, Purchase, Current, Revenue, Rate)));
        }
        public override void OnReceiveConclusion(string[] param)
        {
            if (param.Length == 0x2E && uint.TryParse(param[0xA], out uint order) && OrderNumber.Remove(order.ToString()) && param[0xD].Equals("2") && uint.TryParse(param[9], out uint number) && double.TryParse(param[0x10], out double price))
                OrderNumber[number.ToString()] = price;

            else if ((param[8].Equals(Enum.GetName(typeof(TR), TR.SONBT001)) || param[8].Equals(Enum.GetName(typeof(TR), TR.CONET801))) && double.TryParse(param[0x3C], out double nPrice))
            {
                OrderNumber[param[0x2D]] = nPrice;
                SendBalance?.Invoke(this, new SendSecuritiesAPI(param[0x67], param[0x6C]));
            }
            else if (uint.TryParse(param[0x2F], out uint oNum) && OrderNumber.Remove(oNum.ToString()) && param[0x38].Equals("1") && uint.TryParse(param[0x2D], out uint nNum) && double.TryParse(param[0x3C], out double oPrice))
                OrderNumber[nNum.ToString()] = oPrice;
        }
        public override void OnReceiveEvent(string[] param)
        {
            if (int.TryParse(string.Concat(param[8], param[9]), out int volume))
                SendConsecutive?.Invoke(this, new SendConsecutive(new Charts
                {
                    Date = param[0],
                    Price = param[4],
                    Volume = volume
                }));
            if (double.TryParse(param[4], out double current))
            {
                Current = current;
                Revenue = (long)((current - Purchase) * Quantity * transactionMutiplier);
                Rate = (Quantity > 0 ? current / (double)Purchase : Purchase / (double)current) - 1;
            }
            SendStocks?.Invoke(this, new SendHoldingStocks(Code, Quantity, Purchase, Current, Revenue, Rate, Base, Secondary, AdjustTheColorAccordingToTheCurrentSituation(WaitOrder, OrderNumber.Count)));
        }
        public override int GetQuoteUnit(int price, bool info) => base.GetQuoteUnit(price, info);
        public override int GetStartingPrice(int price, bool info) => base.GetStartingPrice(price, info);
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
        public HoldingStocks(TrendsInStockPrices strategics) : base(strategics)
        {
            if (StartProgress(strategics.Code) > 0)
                consecutive.Dispose();

            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;
            consecutive.Connect(this);
        }
        readonly dynamic strategics;
        public event EventHandler<SendConsecutive> SendConsecutive;
        public override event EventHandler<SendSecuritiesAPI> SendBalance;
        public override event EventHandler<SendHoldingStocks> SendStocks;
    }
}