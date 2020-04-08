using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShareInvest.Catalog;
using ShareInvest.EventHandler.BackTesting;
using ShareInvest.GoblinBatContext;
using ShareInvest.Strategy.Statistics;

namespace ShareInvest.Strategy
{
    public class BackTesting : CallUpStatisticalAnalysis
    {
        int Amount
        {
            get; set;
        }
        uint Count
        {
            get; set;
        }
        uint Commission
        {
            get; set;
        }
        long CumulativeRevenue
        {
            get; set;
        }
        double PurchasePrice
        {
            get; set;
        }
        Catalog.XingAPI.Specify Specify
        {
            get; set;
        }
        Dictionary<uint, int> Residue
        {
            get;
        }
        void StartProgress()
        {
            foreach (var quotes in Retrieve.Quotes)
            {
                if (quotes.Price != null && quotes.Volume != null)
                {
                    SendDatum?.Invoke(this, new Datum(quotes.Time, quotes.Price, quotes.Volume));

                    continue;
                }
                SendQuotes?.Invoke(this, new Quotes(quotes.Time, quotes.SellPrice, quotes.BuyPrice, quotes.SellQuantity, quotes.BuyQuantity, quotes.SellAmount, quotes.BuyAmount));
            }
        }
        double Max(double max, XingAPI.Classification classification)
        {
            int num = 1;

            foreach (var kv in Judge)
                switch (classification)
                {
                    case XingAPI.Classification.Sell:
                        if (kv.Value > 0)
                            num += 2;

                        break;

                    case XingAPI.Classification.Buy:
                        if (kv.Value < 0)
                            num += 2;

                        break;
                }
            return max * num * 0.1;
        }
        double SetPurchasePrice(double price)
        {
            if (Math.Abs(Amount) < Math.Abs(Quantity) && Quantity != 0)
                PurchasePrice = (PurchasePrice * Math.Abs(Amount) + price) / Math.Abs(Quantity);

            return Quantity == 0 ? 0 : PurchasePrice;
        }
        double SetLiquidation(double price)
        {
            if (Amount > Quantity && Quantity > -1)
                return price - PurchasePrice;

            else if (Amount < Quantity && Quantity < 1)
                return PurchasePrice - price;

            else
                return 0;
        }
        internal void Max(double trend, Catalog.XingAPI.Specify specify)
        {
            Judge[specify.Time] = trend;
            double temp = 0;

            foreach (var kv in Judge)
                temp += kv.Value;

            Classification = temp == 0 ? string.Empty : temp > 0 ? Analysis.buy : Analysis.sell;
        }
        internal void SendClearingOrder(uint number)
        {
            if (SellOrder.ContainsValue(number) && SellOrder.Remove(SellOrder.First(o => o.Value == number).Key) && Residue.Remove(number))
                return;

            if (BuyOrder.ContainsValue(number) && BuyOrder.Remove(BuyOrder.First(o => o.Value == number).Key) && Residue.Remove(number))
                return;
        }
        internal void SendCorrectionOrder(string price, uint number, int residue)
        {
            if (SellOrder.ContainsValue(number) && SellOrder.Remove(SellOrder.First(o => o.Value == number).Key) && Residue.Remove(number))
            {
                SellOrder[price] = Count;
                Residue[Count++] = residue;

                return;
            }
            if (BuyOrder.ContainsValue(number) && BuyOrder.Remove(BuyOrder.First(o => o.Value == number).Key) && Residue.Remove(number))
            {
                BuyOrder[price] = Count;
                Residue[Count++] = residue;
            }
        }
        internal void SendNewOrder(string price, string classification, int residue)
        {
            switch (classification)
            {
                case Analysis.sell:
                    SellOrder[price] = Count;
                    Residue[Count++] = residue;
                    break;

                case Analysis.buy:
                    BuyOrder[price] = Count;
                    Residue[Count++] = residue;
                    break;
            }
        }
        internal void SendNewOrder(double[] param, string classification, int residue)
        {
            var check = classification.Equals(Analysis.buy);
            var price = param[5];
            var key = price.ToString("F2");

            if (price > 0 && (check ? Quantity + BuyOrder.Count : SellOrder.Count - Quantity) < Max(Specify.Assets / (price * Const.TransactionMultiplier * Const.MarginRate200402), check ? XingAPI.Classification.Buy : XingAPI.Classification.Sell) && (check ? BuyOrder.ContainsKey(key) : SellOrder.ContainsKey(key)) == false)
                SendNewOrder(key, classification, residue);
        }
        internal void SetSellConclusion(double price, int residue)
        {
            var key = price.ToString("F2");

            if (SellOrder.TryGetValue(key, out uint number))
            {
                if (Residue[number] - residue > 0)
                {
                    Residue[number] = Residue[number] - residue;

                    return;
                }
                if (SellOrder.Remove(key) && Residue.Remove(number))
                {
                    Quantity -= 1;
                    Commission += (uint)(price * Const.TransactionMultiplier * Const.Commission);
                    var liquidation = SetLiquidation(price);
                    PurchasePrice = SetPurchasePrice(price);
                    CumulativeRevenue += (long)(liquidation * Const.TransactionMultiplier);
                    Amount = Quantity;
                }
            }
        }
        internal void SetBuyConclusion(double price, int residue)
        {
            var key = price.ToString("F2");

            if (BuyOrder.TryGetValue(key, out uint number))
            {
                if (Residue[number] + residue > 0)
                {
                    Residue[number] = Residue[number] + residue;

                    return;
                }
                if (BuyOrder.Remove(key) && Residue.Remove(number))
                {
                    Quantity += 1;
                    Commission += (uint)(price * Const.TransactionMultiplier * Const.Commission);
                    var liquidation = SetLiquidation(price);
                    PurchasePrice = SetPurchasePrice(price);
                    CumulativeRevenue += (long)(liquidation * Const.TransactionMultiplier);
                    Amount = Quantity;
                }
            }
        }
        internal int Quantity
        {
            get; set;
        }
        internal string AvgPurchase
        {
            get; set;
        }
        internal string Classification
        {
            get; set;
        }
        internal Dictionary<string, uint> BuyOrder
        {
            get;
        }
        internal Dictionary<string, uint> SellOrder
        {
            get;
        }
        internal Dictionary<uint, double> Judge
        {
            get;
        }
        public BackTesting(Catalog.XingAPI.Specify[] specifies, string key) : base(key)
        {
            Residue = new Dictionary<uint, int>();
            SellOrder = new Dictionary<string, uint>();
            BuyOrder = new Dictionary<string, uint>();
            Judge = new Dictionary<uint, double>();
            Parallel.ForEach(specifies, new Action<Catalog.XingAPI.Specify>((param) =>
            {
                if (param.Time == 1440)
                    Specify = param;

                new Analysis(this, param);
            }));
            StartProgress();
        }
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Quotes> SendQuotes;
    }
}