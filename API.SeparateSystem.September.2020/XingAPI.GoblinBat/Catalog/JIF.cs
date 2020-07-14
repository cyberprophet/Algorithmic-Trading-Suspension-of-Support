using System;

using ShareInvest.Catalog.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class JIF : Real, IReals
    {
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                SetFieldData(GetInBlock(), Enum.GetName(typeof(J), J.jangubun), ((int)Attribute.Common).ToString());
                AdviseRealData();
            }
        }
        protected internal override void OnReceiveRealData(string szTrCode) => Connect.GetInstance().OnReceiveChapterOperatingInformation(GetFieldData(OutBlock, Enum.GetName(typeof(J), J.jangubun)), GetFieldData(OutBlock, Enum.GetName(typeof(J), J.jstatus)));
    }
}