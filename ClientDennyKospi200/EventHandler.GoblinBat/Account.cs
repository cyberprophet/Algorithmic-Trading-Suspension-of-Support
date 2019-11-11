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
        public Account(string account)
        {
            if (account.Contains("-"))
            {
                AccNo = account;

                return;
            }
            AccountCategory = account.Split(';');
        }
    }
}