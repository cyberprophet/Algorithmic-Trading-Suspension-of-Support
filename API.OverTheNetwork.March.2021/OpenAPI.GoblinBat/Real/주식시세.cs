using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식시세 : Real
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            string price = API.GetCommRealData(e.sRealKey, Fid[0]), str_offer = API.GetCommRealData(e.sRealKey, Fid[3]), str_bid = API.GetCommRealData(e.sRealKey, Fid[4]);

            if (int.TryParse(str_offer[0] == '-' ? str_offer[1..] : str_offer, out int offer) && int.TryParse(str_bid[0] == '-' ? str_bid[1..] : str_bid, out int bid) && int.TryParse(price[0] == '-' ? price[1..] : price, out int current))
                Send?.Invoke(this, new SendSecuritiesAPI(e.sRealKey, current, offer, bid));
        }
        protected internal override int[] Fid => new int[] { 10, 11, 12, 27, 28, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 311, 567, 568 };
    }
}