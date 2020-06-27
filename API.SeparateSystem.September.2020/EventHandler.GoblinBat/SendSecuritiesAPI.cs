using System;
using System.Windows.Forms;

namespace ShareInvest.EventHandler
{
    public class SendSecuritiesAPI : EventArgs
    {
        public object Convey
        {
            get; private set;
        }
        public SendSecuritiesAPI(FormWindowState state) => Convey = state;
    }
}