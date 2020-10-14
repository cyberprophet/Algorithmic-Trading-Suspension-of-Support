using System;
using System.Threading.Tasks;

using ShareInvest.Analysis;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class EU2 : Real, IReals
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            string[] arr = Enum.GetNames(typeof(CMO)), temp = new string[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, arr[i]);

            if (Connect.HoldingStock.TryGetValue(temp[0x33], out Holding hs))
                new Task(() => hs.OnReceiveConclusion(temp)).Start();
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
                AdviseRealData();
        }
    }
}