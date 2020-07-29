using System;

using ShareInvest.EventHandler;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class JIF : Real, IQuerys<SendSecuritiesAPI>
    {
        public void QueryExcute()
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                SetFieldData(GetInBlock(), Enum.GetName(typeof(J), J.jangubun), ((int)Attribute.Common).ToString());
                AdviseRealData();
            }
        }
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            string jangubun = GetFieldData(OutBlock, Enum.GetName(typeof(J), J.jangubun)), jstatus = GetFieldData(OutBlock, Enum.GetName(typeof(J), J.jstatus));

            if (int.TryParse(jangubun, out int gubun) && int.TryParse(jstatus, out int status))
            {
                Send?.Invoke(this, new SendSecuritiesAPI(gubun, status));
                Connect.GetInstance().OnReceiveChapterOperatingInformation(gubun, status);
            }
        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}