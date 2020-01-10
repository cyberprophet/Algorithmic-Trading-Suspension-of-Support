using System;
using System.Drawing;

namespace ShareInvest.EventHandler
{
    public class Statistics : EventArgs
    {
        public Color Division
        {
            get; private set;
        }
        public int Name
        {
            get; private set;
        }
        public double Check
        {
            get; private set;
        }
        public Statistics(Color color, int name, double check)
        {
            Division = color;
            Name = name;
            Check = check;
        }
    }
}