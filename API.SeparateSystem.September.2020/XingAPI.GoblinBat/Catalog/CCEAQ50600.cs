using System;
using System.Text;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    class CCEAQ50600 : Query, IQuerys<SendSecuritiesAPI>
    {
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
            Send?.Invoke(this, new SendSecuritiesAPI(szMessage));
        }
        protected internal override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            var temp = new StringBuilder[enumerable.Count];
            string str = string.Empty;

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();

                for (int i = 0; i < GetBlockCount(param.Block); i++)
                    if (enumerable.Count < 11)
                    {
                        if (temp[i] == null)
                            temp[i] = new StringBuilder();

                        temp[i] = temp[i].Append(GetFieldData(param.Block, param.Field, i)).Append(';');
                    }
            }
            /*
            foreach (var sb in temp)
                if (sb != null)
                {
                    var param = sb.ToString().Split(';');
                    str += string.Concat(param[0], ';', API.CodeList[param[0]], ';', param[2], ';', param[4], ';', param[5], ';', param[6], ';', param[8], '*');

                    if (param[0].Length == 8 && param[0].Substring(0, 3).Equals("101"))
                    {
                        API.Quantity = param[2].Equals("1") ? -int.Parse(param[4]) : int.Parse(param[4]);
                        API.AvgPurchase = param[5].Substring(0, 6);
                    }
                }
                else if (API.Quantity == 0)
                    API.AvgPurchase = "000.00";

            Send?.Invoke(this, new Balance(str.Split('*')));
            */
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