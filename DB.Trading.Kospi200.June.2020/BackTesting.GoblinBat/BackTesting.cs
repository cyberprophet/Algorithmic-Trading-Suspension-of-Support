using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.EventHandler.BackTesting;
using ShareInvest.GoblinBatContext;
using ShareInvest.Message;
using ShareInvest.Strategy.Statistics;

namespace ShareInvest.Strategy
{
    public class BackTesting : CallUpStatisticalAnalysis
    {
        int Amount
        {
            get; set;
        }
        int Accumulative
        {
            get; set;
        }
        int TodayCommission
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
        long Revenue
        {
            get; set;
        }
        long TodayRevenue
        {
            get; set;
        }
        double PurchasePrice
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
                    SetConclusion(price);
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
                    SetConclusion(price);
                }
            }
        }
        internal void SetStatisticalStorage(string date, double price, bool over)
        {
            if (over)
                while (Quantity != 0)
                {
                    Quantity += Quantity > 0 ? -1 : 1;
                    SetConclusion(price);
                }
            Revenue = CumulativeRevenue - Commission;
            long revenue = Revenue - TodayRevenue, unrealized = (long)(Quantity == 0 ? 0 : (Quantity > 0 ? price - PurchasePrice : PurchasePrice - price) * Const.TransactionMultiplier * Math.Abs(Quantity));
            Accumulative = revenue + unrealized > 0 ? ++Accumulative : revenue + unrealized < 0 ? --Accumulative : 0;
            queue.Enqueue(new Models.Memorize
            {
                Index = index,
                Date = date,
                Code = string.Concat(code.Substring(0, 3), code.Substring(5)),
                Unrealized = unrealized.ToString(),
                Revenue = revenue.ToString(),
                Cumulative = (CumulativeRevenue - Commission).ToString(),
                Commission = (Commission - TodayCommission).ToString(),
                Statistic = Accumulative
            });
            TodayCommission = (int)Commission;
            TodayRevenue = Revenue;
            SellOrder.Clear();
            BuyOrder.Clear();
        }
        void SetConclusion(double price)
        {
            Commission += (uint)(price * Const.TransactionMultiplier * commission);
            var liquidation = SetLiquidation(price);
            PurchasePrice = SetPurchasePrice(price);
            CumulativeRevenue += (long)(liquidation * Const.TransactionMultiplier);
            Amount = Quantity;
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
            queue = new Queue<Models.Memorize>();
            Residue = new Dictionary<uint, int>();
            SellOrder = new Dictionary<string, uint>();
            BuyOrder = new Dictionary<string, uint>();
            Judge = new Dictionary<uint, double>();
            commission = specifies[0].Commission;
            code = specifies[0].Code;
            index = specifies[0].Index > 0 ? specifies[0].Index : GetRepositoryID(specifies);
            Parallel.ForEach(specifies, new Action<Catalog.XingAPI.Specify>((param) =>
            {
                switch (param.Strategy)
                {
                    case basic:
                        new Base(this, param);
                        break;
                }
            }));
            switch (index)
            {
                case 1:
                    index = GetRepositoryID(specifies);
                    break;

                case 0:
                    return;
            }
            if (index > 0)
            {
                StartProgress();
                SetStatisticalStorage(queue).Wait();
            }
        }
        const string basic = "Base";
        readonly double commission;
        readonly string code;
        readonly long index;
        readonly Queue<Models.Memorize> queue;
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Quotes> SendQuotes;
    }
}