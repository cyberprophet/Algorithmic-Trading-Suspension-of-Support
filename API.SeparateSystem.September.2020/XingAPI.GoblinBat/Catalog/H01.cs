using System;
using System.Threading.Tasks;

using ShareInvest.Analysis;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class H01 : Real, IReals
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            string[] arr = Enum.GetNames(typeof(H1)), temp = new string[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, arr[i]);

            if (Connect.HoldingStock.TryGetValue(temp[0xB], out Holding hs))
                new Task(() => hs.OnReceiveConclusion(temp)).Start();
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
                AdviseRealData();
        }
    }
}