using System;

namespace ShareInvest.EventHandler
{
    public class ForceQuit : EventArgs
    {
        public int quit;

        public ForceQuit(int quit)
        {
            this.quit = quit;
        }
    }
}