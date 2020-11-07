using System;

namespace ShareInvest.EventHandler
{
    public class SendSecuritiesAPI : EventArgs
    {
        public SendSecuritiesAPI()
        {

        }
        public object Convey
        {
            get; private set;
        }
    }
}