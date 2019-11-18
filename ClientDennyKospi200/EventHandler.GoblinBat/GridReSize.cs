using System;

namespace ShareInvest.EventHandler
{
    public class GridReSize : EventArgs
    {
        public int ReSize
        {
            get; private set;
        }
        public GridReSize(int size)
        {
            ReSize = size + 25;
        }
    }
}