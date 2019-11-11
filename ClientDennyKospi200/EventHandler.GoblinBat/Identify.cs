using System;
using System.Globalization;
using ShareInvest.Interface;

namespace ShareInvest.EventHandler
{
    public class Identify : EventArgs
    {
        public Identify(string message, DateTime remaining)
        {
            Remaining = string.Concat(message, remaining.ToString("D", new CultureInfo("en-US")));
        }
        public Identify(IConfirm id, string confirm)
        {
            Confirm = string.Concat(id.Confirm, confirm);
        }
        public string Confirm
        {
            get; private set;
        }
        public string Remaining
        {
            get; private set;
        }
    }
}