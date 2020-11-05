using System;
using System.Text;
using System.Threading.Tasks;

using ShareInvest.Analysis;
using ShareInvest.Analysis.XingAPI;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class EC0 : Real, IReals
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            string[] array = Enum.GetNames(typeof(EC)), temp = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
                temp[i] = GetFieldData(OutBlock, array[i]);

            if (Connect.HoldingStock.TryGetValue(temp[temp.Length - 2], out Holding hs))
                new Task(() => hs.OnReceiveEvent(temp)).Start();

            if (Connect.Collection != null && Connect.Collection.TryGetValue(temp[temp.Length - 2], out Collect collect))
                collect.ToCollect(string.Concat(temp[temp.Length - 1], collect.GetTime(temp[temp.Length - 1][temp[temp.Length - 1].Length - 1]).ToString("D3")), new StringBuilder(temp[4]).Append(';').Append(string.Concat(temp[8], temp[9])));
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