using System;

namespace ShareInvest.EventHandler
{
    public class NotifyIconText : EventArgs
    {
        public string Count
        {
            get; private set;
        }
        public NotifyIconText(int count)
        {
            Count = count.ToString();
        }
    }
}