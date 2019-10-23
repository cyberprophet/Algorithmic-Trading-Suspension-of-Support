using System;
using ShareInvest.Communicate;

namespace ShareInvest.EventHandler
{
    public class Identify : EventArgs
    {
        public Identify(string confirm)
        {
            Confirm = confirm;
        }
        public Identify(string message, string remaining)
        {
            Remaining = string.Concat(message, remaining);
        }
        public Identify(IConfirm id, IStrategy st, string confirm)
        {
            Confirm = string.Concat(st.Strategy, ".", id.Confirm, confirm);
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