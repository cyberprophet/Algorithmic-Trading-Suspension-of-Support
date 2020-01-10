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
        public DialogClose(int[] strategy)
        {
            ShortTick = strategy[0];
            ShortDay = strategy[1];
            LongTick = strategy[2];
            LongDay = strategy[3];
            Reaction = strategy[4];
            Hedge = strategy[5];
            Base = strategy[6];
            Sigma = strategy[7];
            Percent = strategy[8];
            Max = strategy[9];
            Quantity = strategy[10];
            Time = strategy[11];
        }
        public int Sigma
        {
            get; private set;
        }
        public int Percent
        {
            get; private set;
        }
        public int Max
        {
            get; private set;
        }
        public int Time
        {
            get; private set;
        }
        public int Quantity
        {
            get; private set;
        }
        public int Base
        {
            get; private set;
        }
    }
}