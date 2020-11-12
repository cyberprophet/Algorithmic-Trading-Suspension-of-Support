using System;
using System.Text;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식호가잔량 : Real
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var index = 0;
            var sb = new StringBuilder(API.GetCommRealData(e.sRealKey, Fid[index]));

            for (index = 0; index < 0x40; index++)
                sb.Append(';').Append(API.GetCommRealData(e.sRealKey, Fid[index + 1]));

            Send?.Invoke(this, new SendSecuritiesAPI(e.sRealKey, sb));
        }
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        protected internal override int[] Fid => new int[] { 21, 41, 61, 81, 51, 71, 91, 42, 62, 82, 52, 72, 92, 43, 63, 83, 53, 73, 93, 44, 64, 84, 54, 74, 94, 45, 65, 85, 55, 75, 95, 46, 66, 86, 56, 76, 96, 47, 67, 87, 57, 77, 97, 48, 68, 88, 58, 78, 98, 49, 69, 89, 59, 79, 99, 50, 70, 90, 60, 80, 100, 121, 122, 125, 126, 23, 24, 128, 129, 138, 139, 200, 201, 238, 291, 292, 293, 294, 295, 621, 631, 622, 632, 623, 633, 624, 634, 625, 635, 626, 636, 627, 637, 628, 638, 629, 639, 630, 640, 13, 299, 215, 216 };
    }
}