using System;

namespace ShareInvest.EventHandler
{
    public class ForceQuit : EventArgs
    {
        public ForceQuit(int quit)
        {
            Quit = quit;
        }
        public int Quit
        {
            get; private set;
        }
    }
}