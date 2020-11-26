using System;
using System.Collections.Generic;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.XingAPI
{
    public class HoldingStocks : Holding
    {
        internal void OnReceiveTrendsInPrices(SendConsecutive e, double gap, int minute)
        {
            switch (strategics)
            {
                case TrendFollowingBasicFutures tf:
                    if (minute == 0x5A0)
                    {
                        if (WaitOrder && (e.Date.CompareTo(cme) > 0 || e.Date.CompareTo(eurex) < 0 || e.Date.CompareTo(start) > 0 && e.Date.CompareTo(end) < 0)
                            && (gap > 0 ? tf.QuantityLong - Quantity > 0 : tf.QuantityShort + Quantity > 0)
                            && (gap > 0 ? e.Volume > tf.ReactionLong : e.Volume < -tf.ReactionShort)
                            && (gap > 0 ? e.Volume + Secondary > e.Volume : e.Volume + Secondary < e.Volume) && OrderNumber.Count == 0)
                        {
                            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Catalog.XingAPI.Order
                            {
                                FnoIsuNo = Code,
                                BnsTpCode = gap > 0 ? "2" : "1",
                                FnoOrdprcPtnCode
                                    = e.Date.CompareTo(start) > 0 && e.Date.CompareTo(end) < 0 ? ((int)Catalog.XingAPI.FnoOrdprcPtnCode.지정가).ToString("D2") : ((int)Catalog.XingAPI.ErxPrcCndiTpCode.지정가).ToString("D1"),
                                OrdPrc = (gap > 0 ? Offer : Bid).ToString("F2"),
                                OrdQty = "1"
                            }));
                            WaitOrder = false;
                        }
                        Base = gap;
                    }
                    else
                    {
                        if (WaitOrder && (e.Date.CompareTo(cme) > 0 || e.Date.CompareTo(eurex) < 0 || e.Date.CompareTo(start) > 0 && e.Date.CompareTo(end) < 0)
                            && (tf.QuantityShort + Quantity < 0 && Base < 0 || Base > 0 && Quantity - tf.QuantityLong > 0)
                            && Revenue / Math.Abs(Quantity) > 0x927C)
                        {
                            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Catalog.XingAPI.Order
                            {
                                FnoIsuNo = Code,
                                BnsTpCode = Quantity > 0 ? "1" : "2",
                                FnoOrdprcPtnCode
                                    = e.Date.CompareTo(start) > 0 && e.Date.CompareTo(end) < 0 ? ((int)Catalog.XingAPI.FnoOrdprcPtnCode.시장가).ToString("D2") : ((int)Catalog.XingAPI.ErxPrcCndiTpCode.시장가).ToString("D1"),
                                OrdPrc = (Quantity > 0 ? Bid : Offer).ToString("F2"),
                                OrdQty = "1"
                            }));
                            WaitOrder = false;
                        }
                        Secondary = gap;
                    }
                    break;
            }
        }
        public override void OnReceiveBalance(string[] param)
        {
            var cme = param.Length > 0x1C;

            if (param[cme ? 0x33 : 0xB].Length == 8 && int.TryParse(param[cme ? 0x53 : 0xE], out int quantity)
                && double.TryParse(param[cme ? 0x52 : 0xD], out double current)
                && int.TryParse(param[cme ? 0x2D : 9], out int number) && OrderNumber.Remove(number.ToString()))
            {
                var gb = param[cme ? 0x37 : 0x14];
                Current = current;
                Purchase
                    = gb.Equals("2") && Quantity >= 0 ? ((Purchase ?? 0D) * Quantity + current * quantity) / (quantity + Quantity) : (gb.Equals("1") && Quantity <= 0 ? (current * quantity - (Purchase ?? 0D) * Quantity) / (quantity - Quantity) : (Purchase ?? 0D));
                Quantity += gb.Equals("1") ? -quantity : quantity;
                Revenue = (long)(current - Purchase) * Quantity * TransactionMultiplier;
                Rate = (Quantity > 0 ? current / (double)Purchase : Purchase / (double)current) - 1;
            }
            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, string, int, dynamic, dynamic, long, double>(param[cme ? 0x33 : 0xB], param[cme ? 0x34 : 0xB], Quantity, Purchase, Current, Revenue, Rate)));
            SendBalance?.Invoke(this, new SendSecuritiesAPI(true));
        }
        public override void OnReceiveConclusion(string[] param)
        {
            switch (param[param.Length - 1])
            {
                case "EU0":
                case "EU1":
                    switch (param[37])
                    {
                        case "HO01":
                            Console.WriteLine(param[37] + "_" + param.Length);
                            break;

                        case "CH01":
                            Console.WriteLine(param[37] + "_" + param.Length);
                            break;
                    }
                    break;
            }
            if (param.Length == 0x2E && uint.TryParse(param[0xA], out uint order) && OrderNumber.Remove(order.ToString())
                && param[0xD].Equals("2") && uint.TryParse(param[9], out uint number) && double.TryParse(param[0x10], out double price))
                OrderNumber[number.ToString()] = price;

            else if ((param[8].Equals(Enum.GetName(typeof(TR), TR.SONBT001)) || param[8].Equals(Enum.GetName(typeof(TR), TR.CONET801)))
                && double.TryParse(param[0x3C], out double nPrice))
            {
                OrderNumber[param[0x2D]] = nPrice;
                SendBalance?.Invoke(this, new SendSecuritiesAPI(param[0x67], param[0x6C]));
                WaitOrder = true;
            }
            else if (param.Length > 0x38 && uint.TryParse(param[0x2F], out uint oNum) && OrderNumber.Remove(oNum.ToString())
                && param[0x38].Equals("1") && uint.TryParse(param[0x2D], out uint nNum) && double.TryParse(param[0x3C], out double oPrice))
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
                Revenue = (long)((current - (Purchase ?? 0D)) * Quantity * TransactionMultiplier);
                Rate = (Quantity > 0 ? current / (double)Purchase : Purchase / (double)current) - 1;

                if (OrderNumber.Count > 0 && strategics is TrendFollowingBasicFutures
                    && OrderNumber.ContainsValue(Bid) == false && OrderNumber.ContainsValue(Offer) == false)
                    foreach (var kv in OrderNumber)
                        if (kv.Value < Bid || kv.Value > Offer)
                            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Catalog.XingAPI.Order
                            {
                                FnoIsuNo = Code,
                                OrgOrdNo = kv.Key,
                                OrdQty = "1"
                            }));
            }
            if (param[0].CompareTo(end) > 0 && param[0].CompareTo(cme) < 0 && RollOver == false)
            {
                var quantity = Math.Abs(Quantity);
                RollOver = true;

                while (quantity > 0)
                {
                    SendBalance?.Invoke(this, new SendSecuritiesAPI(new Catalog.XingAPI.Order
                    {
                        FnoIsuNo = Code,
                        BnsTpCode = Quantity > 0 ? "1" : "2",
                        FnoOrdprcPtnCode = ((int)Catalog.XingAPI.FnoOrdprcPtnCode.시장가).ToString("D2"),
                        OrdPrc = Purchase.ToString("F2"),
                        OrdQty = "1"
                    }));
                    quantity--;
                }
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
        public override int Cash
        {
            get; protected internal set;
        }
        public override int BuyPrice
        {
            protected internal get; set;
        }
        public override int SellPrice
        {
            protected internal get; set;
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
        protected internal override DateTime NextOrderTime
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
            RollOver = strategics.RollOver;

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
        bool RollOver
        {
            get; set;
        }
        internal override Dictionary<DateTime, double> EstimatedPrice
        {
            get; set;
        }
        const string start = "090000";
        const string end = "153500";
        const string cme = "180000";
        const string eurex = "290000";
        readonly dynamic strategics;
        public event EventHandler<SendConsecutive> SendConsecutive;
        public override event EventHandler<SendSecuritiesAPI> SendBalance;
        public override event EventHandler<SendHoldingStocks> SendStocks;
    }
}