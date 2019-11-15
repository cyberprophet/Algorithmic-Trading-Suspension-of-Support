using System;
using System.Windows.Forms;

namespace ShareInvest.EventHandler
{
    public class DialogClose : EventArgs
    {
        public int ShortDay
        {
            get; private set;
        }
        public int ShortTick
        {
            get; private set;
        }
        public int LongDay
        {
            get; private set;
        }
        public int LongTick
        {
            get; private set;
        }
        public int Reaction
        {
            get; private set;
        }
        public int Hedge
        {
            get; private set;
        }
        public object Sender
        {
            get; private set;
        }
        public DialogClose(string[] param)
        {
            param = param[0].Split('.');

            ShortDay = int.Parse(param[0]);
            ShortTick = int.Parse(param[1]);
            LongDay = int.Parse(param[2]);
            LongTick = int.Parse(param[3]);
            Reaction = int.Parse(param[4]);
            Hedge = int.Parse(param[5]);
        }
        public DialogClose(CheckState state, object sender, decimal shortDay, decimal shortTick, decimal longDay, decimal longTick, decimal reaction)
        {
            Sender = sender;
            ShortDay = (int)shortDay;
            ShortTick = (int)shortTick;
            LongDay = (int)longDay;
            LongTick = (int)longTick;
            Reaction = (int)reaction;
            Hedge = (int)state;
        }
        public DialogClose(CheckState state, decimal shortDay, decimal shortTick, decimal longDay, decimal longTick, decimal reaction)
        {
            ShortDay = (int)shortDay;
            ShortTick = (int)shortTick;
            LongDay = (int)longDay;
            LongTick = (int)longTick;
            Reaction = (int)reaction;
            Hedge = (int)state;
        }
    }
}