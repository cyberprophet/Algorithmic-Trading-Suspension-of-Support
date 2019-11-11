using System;

namespace ShareInvest.EventHandler
{
    public class DialogClose : EventArgs
    {
        public int Close
        {
            get; private set;
        }
        public DialogClose(int close)
        {
            Close = close;
        }
    }
}