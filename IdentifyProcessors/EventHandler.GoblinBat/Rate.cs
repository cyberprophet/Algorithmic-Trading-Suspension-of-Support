using System;

namespace ShareInvest.EventHandler.x64
{
    public class Rate : EventArgs
    {
        public int Result
        {
            get; private set;
        }
        public Rate(int result)
        {
            Result = result;
        }
    }
}