using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    internal class CCEBQ10500 : Query, IQuerys, IEvents<Deposit>, IMessage<NotifyIconText>
    {
        internal CCEBQ10500() : base()
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
            Send.Invoke(this, new Deposit(new string[]
            {
                temp[8],
                temp[9],
                temp[10],
                temp[17],
                temp[18],
                string.Empty,
                temp[19],
                temp[20],
                string.Empty,
                temp[21],
                temp[22],
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                temp[14],
                temp[15],
                temp[16],
                (long.Parse(temp[8]) + long.Parse(temp[13])).ToString(),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
            }));
        }
        public void QueryExcute()
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
            {
                foreach (var param in GetInBlocks(GetType().Name))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data);

                SendErrorMessage(Request(false));
            }
        }
        public event EventHandler<Deposit> Send;
        public event EventHandler<NotifyIconText> SendMessage;
    }
}