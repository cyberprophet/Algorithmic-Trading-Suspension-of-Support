using System.IO;
using System.Text;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
    class 선물호가잔량 : Real
    {
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var index = 0;
            var sb = new StringBuilder(API.GetCommRealData(e.sRealKey, Fid[index]));

            for (index = 0; index < 0x30; index++)
                sb.Append(';').Append(API.GetCommRealData(e.sRealKey, Fid[index + 3]));

            if (sb.Length > 0x1F)
                Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', sb));
        }
        internal override StreamWriter Server
        {
            get; set;
        }
        protected internal override int[] Fid => new int[] { 21, 27, 28, 41, 61, 81, 101, 51, 71, 91, 111, 42, 62, 82, 102, 52, 72, 92, 112, 43, 63, 83, 103, 53, 73, 93, 113, 44, 64, 84, 104, 54, 74, 94, 114, 45, 65, 85, 105, 55, 75, 95, 115, 121, 122, 123, 125, 126, 127, 137, 128, 13, 23, 238, 200, 201, 291, 293, 294, 295 };
    }
}