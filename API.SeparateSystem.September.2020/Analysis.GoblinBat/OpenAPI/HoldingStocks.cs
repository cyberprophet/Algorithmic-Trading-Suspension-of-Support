using System;
using System.Collections.Generic;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.OpenAPI
{
    public class HoldingStocks : Holding
    {
        internal void OnReceiveTrendsInPrices(SendConsecutive e, double gap, double peek)
        {
            DateTime interval;

            switch (strategics)
            {
                case SatisfyConditionsAccordingToTrends sc:
                    interval = e.Date.Length == 6 ? new DateTime(NextOrderTime.Year, NextOrderTime.Month, NextOrderTime.Day, int.TryParse(e.Date.Substring(0, 2), out int cHour) ? cHour : DateTime.Now.Hour, int.TryParse(e.Date.Substring(2, 2), out int cMinute) ? cMinute : DateTime.Now.Minute, int.TryParse(e.Date.Substring(4), out int cSecond) ? cSecond : DateTime.Now.Second) : DateTime.Now;

                    if (sc.TradingBuyQuantity > 0 && Bid < peek * (1 - sc.TradingBuyRate) && gap > 0 && OrderNumber.ContainsValue(Bid) == false && WaitOrder && (sc.TradingBuyInterval == 0 || sc.TradingBuyInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매수, sc.Code, sc.TradingBuyQuantity, Bid, string.Empty)));
                        WaitOrder = false;

                        if (sc.TradingBuyInterval > 0)
                            NextOrderTime = MeasureTheDelayTime(sc.TradingBuyInterval * (Purchase > 0 && Bid > 0 ? Purchase / (double)Bid : 1), interval);
                    }
                    else if (Quantity > 0)
                    {
                        if (sc.TradingSellQuantity > 0 && Offer > peek * (1 + sc.TradingSellRate) && Offer > Purchase + tax * Offer && gap < 0 && OrderNumber.ContainsValue(Offer) == false && WaitOrder && (sc.TradingSellInterval == 0 || sc.TradingSellInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
                        {
                            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매도, sc.Code, sc.TradingSellQuantity, Offer, string.Empty)));
                            WaitOrder = false;

                            if (sc.TradingSellInterval > 0)
                                NextOrderTime = MeasureTheDelayTime(sc.TradingSellInterval * (Purchase > 0 && Offer > 0 ? Offer / (double)Purchase : 1), interval);
                        }
                        else if (SellPrice > 0 && sc.ReservationSellQuantity > 0 && Offer > SellPrice && OrderNumber.ContainsValue(Offer) == false && WaitOrder)
                        {
                            for (int i = 0; i < sc.ReservationSellUnit; i++)
                                SellPrice += GetQuoteUnit(SellPrice, Market);

                            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매도, sc.Code, sc.ReservationSellQuantity, Offer, string.Empty)));
                            WaitOrder = false;
                        }
                        else if (BuyPrice > 0 && sc.ReservationBuyQuantity > 0 && Bid < BuyPrice && OrderNumber.ContainsValue(Bid) == false && WaitOrder)
                        {
                            for (int i = 0; i < sc.ReservationBuyUnit; i++)
                                BuyPrice -= GetQuoteUnit(BuyPrice, Market);

                            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매수, sc.Code, sc.ReservationBuyQuantity, Bid, string.Empty)));
                            WaitOrder = false;
                        }
                        else if (SellPrice == 0 && Purchase > 0)
                            SellPrice = GetStartingPrice((1 + sc.ReservationSellRate) * Purchase, Market);

                        else if (BuyPrice == 0 && Purchase > 0)
                            BuyPrice = GetStartingPrice(Purchase * (1 - sc.ReservationBuyRate), Market);
                    }
                    break;

                case TrendsInValuation tv:
                    interval = e.Date.Length == 6 ? new DateTime(NextOrderTime.Year, NextOrderTime.Month, NextOrderTime.Day, int.TryParse(e.Date.Substring(0, 2), out int hour) ? hour : DateTime.Now.Hour, int.TryParse(e.Date.Substring(2, 2), out int minute) ? minute : DateTime.Now.Minute, int.TryParse(e.Date.Substring(4), out int second) ? second : DateTime.Now.Second) : DateTime.Now;

                    if (tv.TradingAddtionalQuantity > 0 && Bid < peek * (1 - tv.AdditionalPosition) && gap > 0 && OrderNumber.ContainsValue(Bid) == false && WaitOrder && (tv.AddtionalInterval == 0 || tv.AddtionalInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매수, tv.Code, tv.TradingAddtionalQuantity, Bid, string.Empty)));
                        WaitOrder = false;

                        if (tv.AddtionalInterval > 0)
                            NextOrderTime = MeasureTheDelayTime(tv.AddtionalInterval, interval);
                    }
                    else if (tv.TradingSubtractionalQuantity > 0 && Offer > peek * (1 + tv.SubtractionalPosition) && Offer > Purchase && gap < 0 && OrderNumber.ContainsValue(Offer) == false && WaitOrder && (tv.SubtractionalInterval == 0 || tv.SubtractionalInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
                    {
                        SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<int, string, int, int, string>((int)OpenOrderType.신규매도, tv.Code, tv.TradingSubtractionalQuantity, Offer, string.Empty)));
                        WaitOrder = false;

                        if (tv.SubtractionalInterval > 0)
                            NextOrderTime = MeasureTheDelayTime(tv.SubtractionalInterval, interval);
                    }
                    break;

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

                case TrendFollowingBasicFutures tf:
                    if (0x5A0 == (int)peek)
                    {
                        if (WaitOrder && e.Date.CompareTo(start) > 0 && e.Date.CompareTo(end) < 0 && (gap > 0 ? tf.QuantityLong - Quantity > 0 : tf.QuantityShort + Quantity > 0) && (gap > 0 ? e.Volume > tf.ReactionLong : e.Volume < -tf.ReactionShort) && (gap > 0 ? e.Volume + Secondary > e.Volume : e.Volume + Secondary < e.Volume) && OrderNumber.Count == 0)
                        {
                            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, int, string, string, int, string, string>(Code, 1, gap > 0 ? "2" : "1", ((int)Catalog.OpenAPI.OrderType.지정가).ToString(), 1, (gap > 0 ? Offer : Bid).ToString("F2"), string.Empty)));
                            WaitOrder = false;
                        }
                        Base = gap;
                    }
                    else
                    {
                        if (WaitOrder && e.Date.CompareTo(start) > 0 && e.Date.CompareTo(end) < 0 && (tf.QuantityShort + Quantity < 0 && Base < 0 || Base > 0 && Quantity - tf.QuantityLong > 0) && Revenue / Math.Abs(Quantity) > 0x927C)
                        {
                            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, int, string, string, int, string, string>(Code, 1, Quantity > 0 ? "1" : "2", ((int)Catalog.OpenAPI.OrderType.시장가).ToString(), 1, string.Empty, string.Empty)));
                            WaitOrder = false;
                        }
                        Secondary = gap;
                    }
                    return;
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
        public override int Cash
        {
            get; protected internal set;
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
            if (param.Length == 0x20 && double.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out double price))
            {
                Current = price;
                Revenue = (long)((price - Purchase) * Quantity * TransactionMultiplier);
                Rate = (price / Purchase - 1) * (Quantity > 0 ? 1 : -1);
            }
            else if (int.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out int current))
            {
                Current = current;
                Revenue = (current - Purchase) * Quantity;
                Rate = current / (double)Purchase - 1;
            }
            SendStocks?.Invoke(this, new SendHoldingStocks(Code, Quantity, Purchase, Current, Revenue, Rate, Base, Secondary, AdjustTheColorAccordingToTheCurrentSituation(WaitOrder, OrderNumber.Count)));
        }
        public override void OnReceiveBalance(string[] param)
        {
            if (param.Length == 0x13 && long.TryParse(param[7], out long active) && double.TryParse(param[0xC].StartsWith("-") ? param[0xC].Substring(1) : param[0xC], out double offer) && double.TryParse(param[0xD].StartsWith("-") ? param[0xD].Substring(1) : param[0xD], out double bid) && double.TryParse(param[5].StartsWith("-") ? param[5].Substring(1) : param[5], out double unit) && double.TryParse(param[0x10], out double transaction) && int.TryParse(param[4], out int amount) && double.TryParse(param[3].StartsWith("-") ? param[3].Substring(1) : param[3], out double price))
            {
                var classification = param[9].Equals("1") ? -1 : 1;
                Current = price;
                Quantity = amount * classification;
                Purchase = unit;
                Revenue = (long)((price - unit) * classification * amount * transaction);
                Rate = price / unit - 1;
                Bid = bid;
                Offer = offer;
                WaitOrder = true;
                SendBalance?.Invoke(this, new SendSecuritiesAPI((long)(active * transaction * MarginRate * classification * price)));
            }
            else if (long.TryParse(param[9], out long available) && int.TryParse(param[7], out int purchase) && int.TryParse(param[5].StartsWith("-") ? param[5].Substring(1) : param[5], out int current) && int.TryParse(param[6], out int quantity))
            {
                Current = current;
                Quantity = quantity;
                Purchase = purchase;
                Revenue = (current - purchase) * quantity;
                Rate = current / (double)purchase - 1;
                WaitOrder = true;
                SendBalance?.Invoke(this, new SendSecuritiesAPI(available * current));
            }
            SendBalance?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, string, int, dynamic, dynamic, long, double>(param[1].StartsWith("A") ? param[1].Substring(1) : param[1], param[param[1].Length == 8 ? 2 : 4], Quantity, Purchase, Current, Revenue, Rate)));
        }
        public override void OnReceiveConclusion(string[] param)
        {
            Cash = 0;

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
                    if (param[12].EndsWith(cancellantion) && OrderNumber.TryGetValue(param[11], out dynamic price))
                        Cash = (price < Current && price is int ? price : 0) * (int.TryParse(param[7], out int volume) ? volume : 0);

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
        public override int BuyPrice
        {
            protected internal get; set;
        }
        public override int SellPrice
        {
            protected internal get; set;
        }
        protected internal override bool Market
        {
            get; set;
        }
        protected internal override DateTime NextOrderTime
        {
            get; set;
        }
        public HoldingStocks(SatisfyConditionsAccordingToTrends strategics) : base(strategics)
        {
            if (StartProgress(strategics.Code) > 0)
                consecutive.Dispose();

            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;
            NextOrderTime = DateTime.Now;
            consecutive.Connect(this);
        }
        public HoldingStocks(TrendsInValuation strategics) : base(strategics)
        {
            if (StartProgress(strategics.Code) > 0)
                consecutive.Dispose();

            OrderNumber = new Dictionary<string, dynamic>();
            this.strategics = strategics;
            NextOrderTime = DateTime.Now;
            consecutive.Connect(this);
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
        const double tax = 25e-4 + 15e-5 + 15e-5;
        const string start = "090000";
        const string end = "153500";
        const string conclusion = "체결";
        const string acceptance = "접수";
        const string confirmation = "확인";
        const string cancellantion = "취소";
        const string correction = "정정";
        readonly dynamic strategics;
        public event EventHandler<SendConsecutive> SendConsecutive;
        public override event EventHandler<SendSecuritiesAPI> SendBalance;
        public override event EventHandler<SendHoldingStocks> SendStocks;
    }
}