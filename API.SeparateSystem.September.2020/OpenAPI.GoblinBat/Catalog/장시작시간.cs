using System;

using AxKHOpenAPILib;

using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
    class 장시작시간 : Real, ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        public event EventHandler<SendSecuritiesAPI> Send;
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var param = base.OnReceiveRealData(e, fid);
            int arg = int.MinValue;

            if (int.TryParse(param[1].Substring(0, 2), out int conclusion))
            {
                switch (param[0])
                {
                    case "3":
                        arg = (int)Operation.장시작;
                        DeadLine = true;
                        Delay.Milliseconds = 0xC9;
                        break;

                    case "e" when DeadLine:
                        arg = (int)Operation.선옵_장마감전_동시호가_종료;
                        DeadLine = false;
                        Delay.Milliseconds = 0xE11;
                        break;

                    case "8":
                        arg = (int)Operation.장마감;
                        DeadLine = false;
                        Delay.Milliseconds = 0xE12;
                        break;
                }
                if (arg > 0)
                    Send?.Invoke(this, new SendSecuritiesAPI(arg, conclusion));
            }
            SendMessage(string.Concat(DeadLine, '_', param[0], '_', param[1], '_', param[2]));
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