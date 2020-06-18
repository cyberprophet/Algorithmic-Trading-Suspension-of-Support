using System;
using System.Collections.Generic;
using System.Text;

namespace ShareInvest.EventHandler
{
    public class NotifyIconText : EventArgs
    {
        public object NotifyIcon
        {
            get; private set;
        }
        public NotifyIconText(int count, string code) => NotifyIcon = new Dictionary<int, string>()
        {
            {
                count,
                code
            }
        };
        public NotifyIconText(string orgOrderNo, string[] param)
        {
            if (uint.TryParse(orgOrderNo, out uint oNum) && int.TryParse(param[param.Length - 1], out int cash))
                NotifyIcon = new string[]
                {
                    string.Concat(cash.ToString("C0"), "\n", param[0]),
                    string.Concat('-', oNum)
                };
        }
        public NotifyIconText(byte start) => NotifyIcon = start;
        public NotifyIconText(int xing) => NotifyIcon = xing;
        public NotifyIconText(char initial) => NotifyIcon = initial;
        public NotifyIconText(string code) => NotifyIcon = code.Trim();
        public NotifyIconText(string account, string id, string name, string server) => NotifyIcon = new StringBuilder(account).Append(id).Append(';').Append(name).Append(';').Append(server);
        public NotifyIconText(bool boolean) => NotifyIcon = boolean;
        public NotifyIconText(Tuple<string, string> tuple) => NotifyIcon = tuple;
    }
}