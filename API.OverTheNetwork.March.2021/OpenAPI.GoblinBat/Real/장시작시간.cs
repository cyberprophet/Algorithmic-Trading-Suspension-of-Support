using System;

using AxKHOpenAPILib;

using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class 장시작시간 : Real
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var param = base.OnReceiveRealData(e, Fid);
            Send?.Invoke(this, new SendSecuritiesAPI(new Tuple<string, string, string>(param[0], param[1], param[^1])));

            if (char.TryParse(param[0], out char operation))
                switch (operation)
                {
                    case '0':
                        if (param[1].Equals(reservation))
                            Delay.Milliseconds = 0xE7;

                        break;

                    case '3':
                        Delay.Milliseconds = 0xC9;
                        break;

                    case 'e':
                        Delay.Milliseconds = 0xE17;
                        break;

                    case '8':
                        Delay.Milliseconds = 0xE11;
                        break;
                }
        }
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        protected internal override int[] Fid => new int[] { 215, 20, 214 };
        const string reservation = "085500";
    }
}