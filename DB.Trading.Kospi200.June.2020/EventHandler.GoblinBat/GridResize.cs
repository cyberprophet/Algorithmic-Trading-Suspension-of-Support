using System;

namespace ShareInvest.EventHandler
{
    public class GridResize : EventArgs
    {
        public int ReSize
        {
            get; private set;
        }
        public int Count
        {
            get; private set;
        }
        public GridResize(int size, int count)
        {
            ReSize = size + 25;
            Count = count;
        }
    }
}