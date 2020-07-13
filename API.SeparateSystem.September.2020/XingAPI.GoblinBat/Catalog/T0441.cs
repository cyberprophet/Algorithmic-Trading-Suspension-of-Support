using System;
using System.Text;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    class T0441 : Query, IQuerys<SendSecuritiesAPI>
    {
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
            Send?.Invoke(this, new SendSecuritiesAPI(szMessage.Trim()));
        }
        protected internal override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            var temp = new StringBuilder[enumerable.Count];

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();

                for (int i = 0; i < GetBlockCount(param.Block); i++)
                    if (enumerable.Count < 13)
                    {
                        if (temp[i] == null)
                            temp[i] = new StringBuilder();

                        temp[i] = temp[i].Append(GetFieldData(param.Block, param.Field, i)).Append(';');
                    }
            }
            foreach (var sb in temp)
                if (sb != null)
                {
                    var param = sb.ToString().Split(';');
                    Send?.Invoke(this, new SendSecuritiesAPI(new string[] { param[0], param[0], param[6], param[2], param[4], param[9], param[11], param[12] }));
                }
        }
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