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
            if (games.Count > 0 && SetStatisticalStorage(games) == false)
                Message = new Secrets().Message;
        }
        void SetConclusion(double price)
        {
            Commission += (uint)(price * Const.TransactionMultiplier * game.Commission);
            var liquidation = SetLiquidation(price);
            PurchasePrice = SetPurchasePrice(price);
            CumulativeRevenue += (long)(liquidation * Const.TransactionMultiplier);
            Amount = Quantity;
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
        internal bool SendClearingOrder(uint number)
        {
            if (SellOrder.ContainsValue(number) && SellOrder.Remove(SellOrder.First(o => o.Value == number).Key) && Residue.Remove(number))
                return true;

            if (BuyOrder.ContainsValue(number) && BuyOrder.Remove(BuyOrder.First(o => o.Value == number).Key) && Residue.Remove(number))
                return true;

            return false;
        }
        internal bool SendCorrectionOrder(string price, uint number, int residue)
        {
            if (SellOrder.ContainsValue(number) && SellOrder.Remove(SellOrder.First(o => o.Value == number).Key) && Residue.Remove(number))
            {
                SellOrder[price] = Count;
                Residue[Count++] = residue;

                return true;
            }
            if (BuyOrder.ContainsValue(number) && BuyOrder.Remove(BuyOrder.First(o => o.Value == number).Key) && Residue.Remove(number))
            {
                BuyOrder[price] = Count;
                Residue[Count++] = residue;

                return true;
            }
            return false;
        }
        internal bool SendNewOrder(string price, string classification, int residue)
        {
            switch (classification)
            {
                case Analysis.sell:
                    SellOrder[price] = Count;
                    Residue[Count++] = residue;
                    return true;

                case Analysis.buy:
                    BuyOrder[price] = Count;
                    Residue[Count++] = residue;
                    return true;
            }
            return false;
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
            games.Enqueue(new Models.ImitationGames
            {
                Assets = game.Assets,
                Code = game.Code,
                Commission = game.Commission,
                MarginRate = game.MarginRate,
                Strategy = game.Strategy,
                RollOver = game.RollOver,
                BaseTime = game.BaseTime,
                BaseShort = game.BaseShort,
                BaseLong = game.BaseLong,
                NonaTime = game.NonaTime,
                NonaShort = game.NonaShort,
                NonaLong = game.NonaLong,
                OctaTime = game.OctaTime,
                OctaShort = game.OctaShort,
                OctaLong = game.OctaLong,
                HeptaTime = game.HeptaTime,
                HeptaShort = game.HeptaShort,
                HeptaLong = game.HeptaLong,
                HexaTime = game.HexaTime,
                HexaShort = game.HexaShort,
                HexaLong = game.HexaLong,
                PentaTime = game.PentaTime,
                PentaShort = game.PentaShort,
                PentaLong = game.PentaLong,
                QuadTime = game.QuadTime,
                QuadShort = game.QuadShort,
                QuadLong = game.QuadLong,
                TriTime = game.TriTime,
                TriShort = game.TriShort,
                TriLong = game.TriLong,
                DuoTime = game.DuoTime,
                DuoShort = game.DuoShort,
                DuoLong = game.DuoLong,
                MonoTime = game.MonoTime,
                MonoShort = game.MonoShort,
                MonoLong = game.MonoLong,
                Date = date,
                Unrealized = unrealized,
                Revenue = revenue,
                Cumulative = CumulativeRevenue - Commission,
                Fees = (int)(Commission - TodayCommission),
                Statistic = Accumulative
            });
            TodayCommission = (int)Commission;
            TodayRevenue = Revenue;
            SellOrder.Clear();
            BuyOrder.Clear();
            Residue.Clear();
            Count = 0;
        }
        internal int Quantity
        {
            get; set;
        }
        internal double PurchasePrice
        {
            get; private set;
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
        public string Message
        {
            get; private set;
        }
        public BackTesting(Models.ImitationGames game, string key) : base(key)
        {
            this.game = game;
            Residue = new Dictionary<uint, int>();
            SellOrder = new Dictionary<string, uint>();
            BuyOrder = new Dictionary<string, uint>();
            Judge = new Dictionary<uint, double>();
            Parallel.ForEach(Retrieve.GetCatalog(game), new Action<Catalog.XingAPI.Specify>((param) =>
            {
                switch (param.Strategy)
                {
                    case basic:
                        new Base(this, param);
                        break;

                    case bantam:
                        new Bantam(this, param);
                        break;

                    case feather:
                        new Feather(this, param);
                        break;
                }
            }));
            games = new Queue<Models.ImitationGames>();
            StartProgress();
        }
        const string basic = "Base";
        const string bantam = "Bantam";
        const string feather = "Feather";
        readonly Models.ImitationGames game;
        readonly Queue<Models.ImitationGames> games;
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Quotes> SendQuotes;
    }
}