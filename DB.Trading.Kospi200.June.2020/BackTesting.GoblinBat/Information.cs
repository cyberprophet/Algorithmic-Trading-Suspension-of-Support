using System;
using ShareInvest.GoblinBatContext;
using ShareInvest.Catalog;
using ShareInvest.Models;

namespace ShareInvest.Strategy
{
    partial class Information : CallUpGoblinBat
    {
        internal Information(string key) : base(key)
        {

        }
        internal void Operate(Chart ch, int quantity)
        {
            if (quantity != 0)
            {
                Quantity += quantity;
                Commission += (int)((quantity > 0 ? ch.Price + Const.ErrorRate : ch.Price - Const.ErrorRate) * Const.TransactionMultiplier * Const.Commission);
                Liquidation = ch.Price;
                PurchasePrice = quantity > 0 ? ch.Price + Const.ErrorRate : ch.Price - Const.ErrorRate;
                Amount = Quantity;
                CumulativeRevenue += (long)(Liquidation * Const.TransactionMultiplier);
            }
        }
        internal void Save(Chart ch, Specify specify)
        {
            Revenue = CumulativeRevenue - Commission;
            SetStorage(new Logs
            {
                Code = specify.Code,
                Assets = specify.Assets,
                Strategy = specify.Strategy,
                Date = int.Parse(ch.Date.ToString().Substring(0, 6)),
                Unrealized = (long)(Quantity == 0 ? 0 : (Quantity > 0 ? ch.Price - PurchasePrice : PurchasePrice - ch.Price) * Const.TransactionMultiplier * Math.Abs(Quantity)),
                Revenue = Revenue - TodayRevenue,
                Cumulative = CumulativeRevenue - Commission
            });
            TodayRevenue = Revenue;
        }
        internal int Quantity
        {
            get; private set;
        }
        internal double PurchasePrice
        {
            get
            {
                return purchase;
            }
            private set
            {
                if (Math.Abs(Amount) < Math.Abs(Quantity) && Quantity != 0)
                {
                    purchase = (purchase * Math.Abs(Amount) + value) / Math.Abs(Quantity);
                    Amount = Quantity;
                }
                else if (Quantity == 0)
                    purchase = 0;
            }
        }
        double Liquidation
        {
            get
            {
                return liquidation;
            }
            set
            {
                if (Amount > Quantity && Quantity > -1)
                    liquidation = value - PurchasePrice;

                else if (Amount < Quantity && Quantity < 1)
                    liquidation = PurchasePrice - value;

                else
                    liquidation = 0;
            }
        }
        int Commission
        {
            get; set;
        }
        int Amount
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
        double purchase;
        double liquidation;
    }
    enum Port
    {
        Collecting = 67,
        Trading = 84
    }
}