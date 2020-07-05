using System;

using ShareInvest.EventHandler;

namespace ShareInvest.Catalog
{
    public abstract class Holding
    {
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
        public abstract void OnReceiveEvent(string[] param);
        public abstract void OnReceiveBalance(string[] param);
        public abstract void OnReceiveConclusion(string[] param);
        public abstract event EventHandler<SendSecuritiesAPI> SendBalance;
        public abstract event EventHandler<SendHoldingStocks> SendStocks;
        protected internal const string conclusion = "체결";
        protected internal const string acceptance = "접수";
        protected internal const string confirmation = "확인";
        protected internal const string cancellantion = "취소";
        protected internal const string correction = "정정";
    }
}