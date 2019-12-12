using System;

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
        public string[] Param
        {
            get; private set;
        }
        public DialogClose(string[] param)
        {
            Param = param[1].Split('.');
        }
        public DialogClose(decimal hedge, object sender, decimal shortDay, decimal shortTick, decimal longDay, decimal longTick, decimal reaction)
        {
            Sender = sender;
            ShortDay = (int)shortDay;
            ShortTick = (int)shortTick;
            LongDay = (int)longDay;
            LongTick = (int)longTick;
            Reaction = (int)reaction;
            Hedge = (int)hedge;
        }
        public DialogClose(decimal hedge, decimal shortDay, decimal shortTick, decimal longDay, decimal longTick, decimal reaction)
        {
            ShortDay = (int)shortDay;
            ShortTick = (int)shortTick;
            LongDay = (int)longDay;
            LongTick = (int)longTick;
            Reaction = (int)reaction;
            Hedge = (int)hedge;
        }
    }
}