using System;
using System.Threading.Tasks;

using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class NC0 : Real, IReals
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            string[] array = Enum.GetNames(typeof(C)), temp = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
                temp[i] = GetFieldData(OutBlock, array[i]);

            if (Connect.HoldingStock.TryGetValue(temp[temp.Length - 1], out Holding hs))
                new Task(() => hs.OnReceiveEvent(temp)).Start();
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                var param = GetInBlock(code);
                SetFieldData(param.Block, param.Field, param.Data);
                AdviseRealData();
            }
        }
    }
}