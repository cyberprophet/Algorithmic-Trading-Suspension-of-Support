using System;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    internal class CFOAT00100 : Query, IOrders, IMessage<NotifyIconText>
    {
        internal CFOAT00100() : base()
        {
            Console.WriteLine(GetType().Name);
        }
        protected override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
            SendMessage?.Invoke(this, new NotifyIconText(szMessage));
        }
        protected override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            var temp = new string[enumerable.Count];

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();

                for (int i = 0; i < GetBlockCount(param.Block); i++)
                    temp[temp.Length - enumerable.Count - 1] = GetFieldData(param.Block, param.Field, i);
            }
            for (int i = 0; i < temp.Length; i++)
                Console.WriteLine(i + "\t" + temp[i]);
        }
        public void QueryExcute(Order order)
        {
            var secret = new Secret();
            var name = GetType().Name;

            if (LoadFromResFile(secret.GetResFileName(name)))
            {
                foreach (var param in GetInBlocks(secret.GetData(name, order)))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data);

                SendErrorMessage(Request(false));
            }
        }
        public event EventHandler<NotifyIconText> SendMessage;
    }
}