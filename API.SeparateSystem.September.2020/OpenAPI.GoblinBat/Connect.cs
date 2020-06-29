using ShareInvest.DelayRequest;

namespace ShareInvest.OpenAPI
{
    class Connect
    {
        internal static Connect GetInstance(AxKHOpenAPILib.AxKHOpenAPI axAPI)
        {
            if (API == null && axAPI.CommConnect() == 0)
                API = new Connect(axAPI);

            return API;
        }
        static Connect API
        {
            get; set;
        }
        Connect(AxKHOpenAPILib.AxKHOpenAPI axAPI)
        {
            this.axAPI = axAPI;
            request = Delay.GetInstance(0xCD);
            request.Run();
        }
        readonly Delay request;
        readonly AxKHOpenAPILib.AxKHOpenAPI axAPI;
    }
}