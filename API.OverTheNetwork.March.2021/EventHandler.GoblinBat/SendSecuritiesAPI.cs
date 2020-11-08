using System;

namespace ShareInvest.EventHandler
{
    public class SendSecuritiesAPI : EventArgs
    {
        public SendSecuritiesAPI(string message) => Convey = message;
        public SendSecuritiesAPI(string name, string[] param) => Convey = new Tuple<string, string[]>(name, param);
        public object Convey
        {
            get; private set;
        }
    }
}