using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주문체결 : Chejan
    {
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveChejanData(_DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            var param = base.OnReceiveChejanData(e, fid);

            if (Connect.HoldingStock.TryGetValue(param[3].Length == 8 ? param[3] : param[3].Substring(1), out Holding hs))
                new Task(() =>
                {
                    hs.OnReceiveConclusion(param);
                    Connect.Cash += hs.Cash;
                }).Start();
        }
        readonly int[] fid = new int[] { 9201, 9203, 9205, 9001, 912, 913, 302, 900, 901, 902, 903, 904, 905, 906, 907, 908, 909, 910, 911, 10, 27, 28, 914, 915, 938, 939, 919, 920, 921, 922, 923 };
    }
}