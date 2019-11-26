using System;
using System.Globalization;

namespace ShareInvest.EventHandler
{
    public class Identify : EventArgs
    {
        public Identify(string message, DateTime remaining)
        {
            Remaining = string.Concat(message, remaining.ToString("D", new CultureInfo("en-US")));
        }
        public Identify(string confirm)
        {
            Confirm = confirm.Contains("되었습니다") ? string.Concat(confirm, ".") : confirm;
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