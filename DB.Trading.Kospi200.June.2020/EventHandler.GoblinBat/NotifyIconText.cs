using System;
using System.Text;
using ShareInvest.GoblinBatControls;

namespace ShareInvest.EventHandler
{
    public class NotifyIconText : EventArgs
    {
        public object NotifyIcon
        {
            get; private set;
        }
        public NotifyIconText(int count)
        {
            NotifyIcon = count;
        }
        public NotifyIconText(string code)
        {
            NotifyIcon = code;
        }
        public NotifyIconText(StatisticalAnalysis analysis)
        {
            NotifyIcon = analysis;
        }
        public NotifyIconText(string account, string id, string name, string server)
        {
            NotifyIcon = new StringBuilder(account).Append(id).Append(';').Append(name).Append(';').Append(server);
        }
    }
}