using System;
using System.Drawing;

namespace ShareInvest.EventHandler.XingAPI
{
    public class OnReceiveOperatingState : EventArgs
    {
        public bool State
        {
            get; private set;
        }
        public OnReceiveOperatingState(Color state) => State = state.Equals(Color.Maroon);
    }
}