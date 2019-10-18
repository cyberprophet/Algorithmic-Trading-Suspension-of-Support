using System;

namespace ShareInvest.EventHandler
{
    public class ProgressRate : EventArgs
    {
        public IAsyncResult Result
        {
            get; private set;
        }
        public ProgressRate(IAsyncResult result)
        {
            Result = result;
        }

    }
}