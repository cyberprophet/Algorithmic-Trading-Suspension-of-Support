using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Catalog.OpenAPI;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.OpenAPI.Catalog;

namespace ShareInvest.OpenAPI
{
    class Connect : ISendSecuritiesAPI
    {
        internal void InputValueRqData(TR param) => request.RequestTrData(new Task(() =>
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                axAPI.SetInputValue(count[i], value[i]);

            SendErrorMessage(axAPI.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo));
        }));
        void SendErrorMessage(int error)
        {
            if (error < 0 && new Error().Message.TryGetValue(error, out string param))
                Send?.Invoke(this, new SendSecuritiesAPI(param));
        }
        Connect(AxKHOpenAPI axAPI)
        {
            this.axAPI = axAPI;
            request = Delay.GetInstance(0xCD);
            request.Run();
        }
        static Connect API
        {
            get; set;
        }
        internal static HashSet<TR> TR
        {
            get; private set;
        }
        internal static HashSet<Real> Real
        {
            get; private set;
        }
        internal static Dictionary<string, Chejan> Chejan
        {
            get; private set;
        }
        internal static Connect GetInstance(AxKHOpenAPI axAPI)
        {
            if (API == null && axAPI.CommConnect() == 0)
            {
                API = new Connect(axAPI);
                TR = new HashSet<TR>();
                Real = new HashSet<Real>() { new 주식체결 { API = axAPI } };
                Chejan = new Dictionary<string, Chejan>() { { ((int)ChejanType.주문체결).ToString(), new 주문체결 { API = axAPI } }, { ((int)ChejanType.잔고).ToString(), new 잔고 { API = axAPI } }, { ((int)ChejanType.파생잔고).ToString(), new 파생잔고 { API = axAPI } } };
            }
            return API;
        }
        readonly Delay request;
        readonly AxKHOpenAPI axAPI;
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}