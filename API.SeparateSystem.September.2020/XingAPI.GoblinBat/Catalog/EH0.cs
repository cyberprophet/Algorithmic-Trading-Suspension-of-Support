using System;
using System.Text;

using ShareInvest.Analysis;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class EH0 : Real, IReals
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            var sb = new StringBuilder();

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

            if (Connect.HoldingStock.TryGetValue(str[0x22], out Holding hs) && double.TryParse(str[5], out double bid) && double.TryParse(str[4], out double offer))
            {
                hs.Offer = offer;
                hs.Bid = bid;
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