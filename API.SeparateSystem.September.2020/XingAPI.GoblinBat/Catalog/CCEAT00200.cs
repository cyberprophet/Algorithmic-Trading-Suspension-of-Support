using System;
using System.Threading.Tasks;

using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class CCEAT00200 : Query, IOrders<SendSecuritiesAPI>
    {
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            Send?.Invoke(this, new SendSecuritiesAPI(szMessage.Trim()));
            base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
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
            Delay.Milliseconds = 0x3E8 / GetTRCountPerSec(szTrCode);
        }
        public void QueryExcute(IOrders order) => Connect.GetInstance().Request.RequestTrData(new Task(() =>
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                foreach (var param in GetInBlocks(Secrecy.GetData(this, order)))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data);

                SendErrorMessage(GetType().Name, Request(false));
            }
        }));
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}