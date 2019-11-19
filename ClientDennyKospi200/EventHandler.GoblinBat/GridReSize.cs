using System;

namespace ShareInvest.EventHandler
{
    public class GridReSize : EventArgs
    {
        public int ReSize
        {
            get; private set;
        }
        public int Count
        {
            get; private set;
        }
        public GridReSize(int size, int count)
        {
            ReSize = size + 25;
            Count = count;
        }
    }
}