using System;
using ShareInvest.Interface;

namespace ShareInvest.EventHandler
{
    public class Account : EventArgs, IAccount
    {
        public string[] AccountCategory
        {
            get; private set;
        }
        public string AccNo
        {
            get; private set;
        }
        public long BasicAssets
        {
            get;
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
        public Account(string account, string id)
        {
            AccNo = account;
            ID = id;
        }
    }
}