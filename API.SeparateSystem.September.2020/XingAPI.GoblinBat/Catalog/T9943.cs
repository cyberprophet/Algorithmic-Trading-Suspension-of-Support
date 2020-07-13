using System;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    class T9943 : Query, IQuerys<SendSecuritiesAPI>
    {
        protected internal override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            string[] code = null, name = null;

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();
                var length = GetBlockCount(param.Block);

                switch (enumerable.Count)
                {
                    case 0:
                        for (int i = 0; i < code.Length; i++)
                            Send?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, string>(code[i], name[i])));

                        return;

                    case 1:
                        code = new string[length];
                        break;

                    case 2:
                        name = new string[length];
                        break;
                }
                for (int i = 0; i < length; i++)
                    switch (enumerable.Count)
                    {
                        case 1:
                            code[i] = GetFieldData(param.Block, param.Field, i);
                            break;

                        case 2:
                            name[i] = GetFieldData(param.Block, param.Field, i);
                            break;
                    }
            }
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