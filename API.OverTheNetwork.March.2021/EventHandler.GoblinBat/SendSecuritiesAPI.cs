using System;
using System.Text;

using ShareInvest.Catalog.Models;

namespace ShareInvest.EventHandler
{
    public class SendSecuritiesAPI : EventArgs
    {
        public SendSecuritiesAPI(short error) => Convey = error;
        public SendSecuritiesAPI(Codes codes) => Convey = codes;
        public SendSecuritiesAPI(Priority priority) => Convey = priority;
        public SendSecuritiesAPI(Tuple<string, string> tuple) => Convey = tuple;
        public SendSecuritiesAPI(Tuple<string, string, string> operation) => Convey = operation;
        public SendSecuritiesAPI(string message) => Convey = message;
        public SendSecuritiesAPI(string[] accounts) => Convey = accounts;
        public SendSecuritiesAPI(string code, StringBuilder sb) => Convey = new Tuple<string, StringBuilder>(code, sb);
        public SendSecuritiesAPI(string code, string time, string price, string volume) => Convey = new Tuple<string, string, string, string>(code, time, price, volume);
        public SendSecuritiesAPI(string code, string name, string retention, string price, int market) => Convey = new Tuple<string, string, string, string, int>(code, name, retention, price, market);
        public object Convey
        {
            get; private set;
        }
    }
}