using System;
using System.Threading;
using System.Threading.Tasks;

using ShareInvest.EventHandler;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class T8437 : Query, IQuerys<SendSecuritiesAPI>
    {
        protected internal override void OnReceiveData(string szTrCode)
        {
            SendSecuritiesAPI api = null;
            var enumerable = GetOutBlocks();
            string[] code = null, name = null, tradeUnit = null, atm = null;

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();
                var length = GetBlockCount(param.Block);

                switch (enumerable.Count)
                {
                    case 3:
                        code = new string[length];
                        break;

                    case 4:
                        name = new string[length];
                        break;

                    case 1:
                        tradeUnit = new string[length];
                        break;

                    case 0:
                        atm = new string[length];
                        break;
                }
                for (int i = 0; i < length; i++)
                    switch (enumerable.Count)
                    {
                        case 3:
                            code[i] = GetFieldData(param.Block, param.Field, i);
                            break;

                        case 4:
                            name[i] = GetFieldData(param.Block, param.Field, i);
                            break;

                        case 1:
                            tradeUnit[i] = GetFieldData(param.Block, param.Field, i);
                            break;

                        case 0:
                            atm[i] = GetFieldData(param.Block, param.Field, i);
                            break;
                    }
            }
            if (Repeat < gubun.Length)
                new Task(() =>
                {
                    Thread.Sleep(0x3ED / GetTRCountPerSec(szTrCode) + Repeat);
                    QueryExcute(gubun[Repeat++]);
                }).Start();
            switch (Repeat)
            {
                case 1:
                case 3:
                    api = new SendSecuritiesAPI(new Tuple<string, string>(code[0], name[0]));
                    break;

                case 4:
                    break;
            }
            if (api != null)
                Send?.Invoke(this, api);
        }
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage) => base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
        public void QueryExcute() => QueryExcute(gubun[Repeat++]);
        void QueryExcute(string property)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                foreach (var param in GetInBlocks(GetType().Name, property))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data);

                SendErrorMessage(GetType().Name, Request(false));
            }
        }
        int Repeat
        {
            get; set;
        }
        readonly string[] gubun = new string[] { "NF", "NC", "NM", "NO" };
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}