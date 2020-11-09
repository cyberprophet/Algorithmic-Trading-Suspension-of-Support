using System;

using ShareInvest.Catalog.Models;

namespace ShareInvest.EventHandler
{
    public class SendSecuritiesAPI : EventArgs
    {
        public SendSecuritiesAPI(Codes codes) => Convey = codes;
        public SendSecuritiesAPI(Tuple<string, string> tuple) => Convey = tuple;
        public SendSecuritiesAPI(string message) => Convey = message;
        public SendSecuritiesAPI(string name, string[] param) => Convey = new Tuple<string, string[]>(name, param);
        public object Convey
        {
            get; private set;
        }
    }
}