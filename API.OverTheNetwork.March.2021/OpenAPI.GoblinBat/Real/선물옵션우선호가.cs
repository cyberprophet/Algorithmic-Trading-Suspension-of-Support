using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class 선물옵션우선호가 : Real
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var param = base.OnReceiveRealData(e, Fid);

            if (string.IsNullOrEmpty(param[0]) == false && string.IsNullOrEmpty(param[1]) == false && string.IsNullOrEmpty(param[2]) == false)
            {
                if (e.sRealKey[1] == '0')
                {
                    if (double.TryParse(param[0][0] == '-' ? param[0][1..] : param[0], out double current) && double.TryParse(param[1][0] == '-' ? param[1][1..] : param[1], out double offer) && double.TryParse(param[2][0] == '-' ? param[2][1..] : param[2], out double bid))
                        Send?.Invoke(this, new SendSecuritiesAPI(e.sRealKey, current, offer, bid));

                    return;
                }
                if (int.TryParse(param[0][0] == '-' ? param[0][1..] : param[0], out int price) && int.TryParse(param[1][0] == '-' ? param[1][1..] : param[1], out int sq) && int.TryParse(param[2][0] == '-' ? param[2][1..] : param[2], out int bq))
                    Send?.Invoke(this, new SendSecuritiesAPI(e.sRealKey, price, sq, bq));
            }
        }
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        protected internal override int[] Fid => new int[] { 10, 27, 28 };
    }
}