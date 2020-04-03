using System;

namespace ShareInvest.EventHandler
{
    public class GridResize : EventArgs
    {
        public int ReSize
        {
            get; private set;
        }
        public GridResize(int size, int count) => ReSize = size + (count > 0 ? 25 : 0);
    }
}