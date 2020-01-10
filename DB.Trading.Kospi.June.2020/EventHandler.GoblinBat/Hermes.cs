using System;
using System.Text;

namespace ShareInvest.EventHandler
{
    public class Hermes : EventArgs
    {
        public Hermes(string[] param)
        {
            Param = param;
        }
        public Hermes(StringBuilder sb)
        {
            Strategy = sb.ToString();
        }
        public Hermes(bool check)
        {
            Check = check;
        }
        public string[] Param
        {
            get; private set;
        }
        public string Strategy
        {
            get; private set;
        }
        public bool Check
        {
            get; private set;
        }
    }
}