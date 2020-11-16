using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식시세 : Real
    {
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override StreamWriter Server
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            string price = API.GetCommRealData(e.sRealKey, Fid[0]), offer = API.GetCommRealData(e.sRealKey, Fid[3]), bid = API.GetCommRealData(e.sRealKey, Fid[4]);

            if (string.IsNullOrEmpty(price) == false && string.IsNullOrEmpty(offer) == false && string.IsNullOrEmpty(bid) == false)
                Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', price, ';', offer, ';', bid));
        }
        protected internal override int[] Fid => new int[] { 10, 11, 12, 27, 28, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 311, 567, 568 };
    }
}