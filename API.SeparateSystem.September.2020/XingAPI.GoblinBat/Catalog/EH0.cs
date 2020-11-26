using System;
using System.Text;

using ShareInvest.Analysis;
using ShareInvest.Analysis.XingAPI;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class EH0 : Real, IReals
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            StringBuilder sb = new StringBuilder(), collection = new StringBuilder();

            foreach (var param in Enum.GetValues(typeof(EH)))
            {
                int temp = (int)param, i = temp % 2 == 1 ? 5 : 1;

                if (temp < 7)
                    while (i > 0 && i < 6)
                        sb.Append(GetFieldData(OutBlock, string.Concat(param, temp % 2 == 1 ? i-- : i++))).Append(';');

                else
                    sb.Append(GetFieldData(OutBlock, param.ToString())).Append(';');
            }
            var str = sb.ToString().Split(';');

            if (Connect.HoldingStock.TryGetValue(str[0x22], out Holding hs)
                && double.TryParse(str[5], out double bid)
                && double.TryParse(str[4], out double offer))
            {
                hs.Offer = offer;
                hs.Bid = bid;
            }
            if (Connect.Collection != null && Connect.Collection.TryGetValue(str[0x22], out Collect collect))
            {
                for (int index = 0; index < str.Length; index++)
                    if (index > 1 && index < 8 || index > 0xB && index < 0x12 || index == 0x1E || index == 0x1F)
                        collection.Append(str[index]).Append(';');

                string hotime = GetFieldData(OutBlock, string.Concat(EH.hotime, 1)),
                    time = string.Concat(hotime, collect.GetTime(hotime[hotime.Length - 1]).ToString("D3"));
                collect.ToCollect(time, collection.Remove(collection.Length - 1, 1));
            }
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