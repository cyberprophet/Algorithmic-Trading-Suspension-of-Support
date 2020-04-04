using System;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    class CCEAT00200 : Query, IOrders, IMessage<NotifyIconText>, IStates<State>
    {
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
            SendMessage?.Invoke(this, new NotifyIconText(szMessage));
        }
        protected internal override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            var temp = new string[enumerable.Count];

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();

                for (int i = 0; i < GetBlockCount(param.Block); i++)
                    temp[temp.Length - enumerable.Count - 1] = GetFieldData(param.Block, param.Field, i);
            }
            SendState?.Invoke(this, new State(API.OnReceiveBalance, API.SellOrder.Count, API.Quantity, API.BuyOrder.Count, API.AvgPurchase, API.MaxAmount));
        }
        public void QueryExcute(Order order)
        {
            var secret = new Secret();
            string name = GetType().Name;

            if (LoadFromResFile(secret.GetResFileName(name)) && (API.SellOrder.ContainsKey(order.OrgOrdNo) || API.BuyOrder.ContainsKey(order.OrgOrdNo)))
            {
                foreach (var param in GetInBlocks(secret.GetData(name, order)))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data);

                SendErrorMessage(name, Request(false));
            }
        }
        internal CCEAT00200() : base() => Console.WriteLine(GetType().Name);
        public event EventHandler<NotifyIconText> SendMessage;
        public event EventHandler<State> SendState;
    }
}