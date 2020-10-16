using System;
using System.Threading.Tasks;

using ShareInvest.Analysis;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class EU0 : Real, IReals
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            string[] arr = Enum.GetNames(typeof(CMO)), temp = new string[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, arr[i]);

            if (Connect.HoldingStock.TryGetValue(temp[0x33], out Holding hs))
                new Task(() => hs.OnReceiveConclusion(temp)).Start();

            var name = GetType().Name;
            SendMessage(string.Concat(8, name, temp[8]));
            SendMessage(string.Concat(37, name, temp[37]));
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
                AdviseRealData();
        }
    }
}