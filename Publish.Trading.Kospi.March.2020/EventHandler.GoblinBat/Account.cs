using System;

namespace ShareInvest.EventHandler
{
    public class Account : EventArgs
    {
        public string[] AccountCategory
        {
            get; private set;
        }
        public string AccNo
        {
            get; private set;
        }
        public string ID
        {
            get; private set;
        }
        public string Name
        {
            get; private set;
        }
        public string Server
        {
            get; private set;
        }
        public Account(string account, string id, string name, string server)
        {
            AccountCategory = account.Split(';');
            ID = id;
            Name = name;
            Server = server;
        }
        public Account(string account, string id, string server)
        {
            AccNo = account;
            ID = string.Concat(id.Substring(0, 1).ToUpper(), id.Substring(1));
            Server = server;
        }
    }
}