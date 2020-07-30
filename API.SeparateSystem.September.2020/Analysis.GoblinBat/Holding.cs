using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis
{
    public abstract class Holding
    {
        public Holding(TrendFollowingBasicFutures strategics)
        {
            TF = strategics;
            Short = new Stack<double>();
            Long = new Stack<double>();
            Parallel.ForEach(strategics.SetCatalog(strategics), new Action<TrendFollowingBasicFutures>(catalog => new Consecutive(catalog, this)));
        }
        public Holding(TrendsInStockPrices strategics)
        {
            TS = strategics;
        }
        public abstract string Code
        {
            get; set;
        }
        public abstract int Quantity
        {
            get; set;
        }
        public abstract dynamic Purchase
        {
            get; set;
        }
        public abstract dynamic Current
        {
            get; set;
        }
        public abstract long Revenue
        {
            get; set;
        }
        public abstract double Rate
        {
            get; set;
        }
        public abstract bool WaitOrder
        {
            get; set;
        }
        public abstract Dictionary<string, dynamic> OrderNumber
        {
            get;
        }
        public abstract void OnReceiveEvent(string[] param);
        public abstract void OnReceiveBalance(string[] param);
        public abstract void OnReceiveConclusion(string[] param);
        public abstract event EventHandler<SendSecuritiesAPI> SendBalance;
        public abstract event EventHandler<SendHoldingStocks> SendStocks;
        protected internal TrendFollowingBasicFutures TF
        {
            get;
        }
        protected internal TrendsInStockPrices TS
        {
            get;
        }
        internal Stack<double> Short
        {
            get;
        }
        internal Stack<double> Long
        {
            get;
        }
        protected internal const string conclusion = "체결";
        protected internal const string acceptance = "접수";
        protected internal const string confirmation = "확인";
        protected internal const string cancellantion = "취소";
        protected internal const string correction = "정정";
        protected internal const uint transactionMutiplier = 0x3D090;
    }
    enum TR
    {
        SONBT001 = 0,
        SONBT002 = 1,
        SONBT003 = 2,
        CONET801 = 3,
        CONET002 = 4,
        CONET003 = 5
    }
}