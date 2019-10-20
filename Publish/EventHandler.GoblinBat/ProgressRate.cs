using System;

namespace ShareInvest.EventHandler
{
    public class ProgressRate : EventArgs
    {
        public int Result
        {
            get; private set;
        }
        public ProgressRate(int result)
        {
            Result = result;
        }
    }
}