using System;
using System.Text;

using ShareInvest.EventHandler;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Maturity
{
    class T8432 : Query, IQuerys<SendSecuritiesAPI>
    {
        protected internal override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            string[] code = null, name = null, price = null;

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();
                var length = GetBlockCount(param.Block);

                switch (enumerable.Count)
                {
                    case 7:
                        code = new string[length];
                        break;

                    case 8:
                        name = new string[length];
                        break;

                    case 0:
                        price = new string[length];
                        break;
                }
                for (int i = 0; i < length; i++)
                    switch (enumerable.Count)
                    {
                        case 7:
                            code[i] = GetFieldData(param.Block, param.Field, i);
                            break;

                        case 8:
                            name[i] = GetFieldData(param.Block, param.Field, i);
                            break;

                        case 0:
                            price[i] = GetFieldData(param.Block, param.Field, i);
                            break;
                    }
            }
            Send?.Invoke(this, new SendSecuritiesAPI(new StringBuilder(code[0]).Append(';').Append(code[1])));
        }
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage) => base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
        public void QueryExcute()
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                foreach (var param in GetInBlocks(GetType().Name))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data);

                SendErrorMessage(GetType().Name, Request(false));
            }
        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}