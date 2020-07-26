using AxKHOpenAPILib;

using ShareInvest.DelayRequest;

namespace ShareInvest.OpenAPI.Catalog
{
    class 장시작시간 : Real
    {
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var param = base.OnReceiveRealData(e, fid);

            switch (param[0])
            {
                case string n when n.Equals("3") && DeadLine == false:
                    DeadLine = true;
                    Delay.Milliseconds = 0xC9;
                    break;

                case string n when n.Equals("e") && DeadLine:
                    DeadLine = false;
                    Delay.Milliseconds = 0xE11;
                    break;
            }
        }
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        bool DeadLine
        {
            get; set;
        }
        readonly int[] fid = new int[] { 215, 20, 214 };
    }
}