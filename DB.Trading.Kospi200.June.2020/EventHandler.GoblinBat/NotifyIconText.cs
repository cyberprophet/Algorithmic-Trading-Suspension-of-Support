using System;
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
    }
}