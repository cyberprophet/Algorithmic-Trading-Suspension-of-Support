using System;

using ShareInvest.Catalog.Models;

namespace ShareInvest.EventHandler
{
    public class SendSecuritiesAPI : EventArgs
    {
        public SendSecuritiesAPI(short error) => Convey = error;
        public SendSecuritiesAPI(Codes codes) => Convey = codes;
        public SendSecuritiesAPI(Tuple<string, string> tuple) => Convey = tuple;
        public SendSecuritiesAPI(string message) => Convey = message;
        public SendSecuritiesAPI(string name, string[] param) => Convey = new Tuple<string, string[]>(name, param);
        public SendSecuritiesAPI(string code, string name, string retention, string price, int market) => Convey = new Tuple<string, string, string, string, int>(code, name, retention, price, market);
        public object Convey
        {
            get; private set;
        }
    }
}