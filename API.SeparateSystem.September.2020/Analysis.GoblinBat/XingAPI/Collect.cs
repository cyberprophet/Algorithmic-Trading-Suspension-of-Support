using System;
using System.Collections.Generic;
using System.Text;

using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.XingAPI
{
    public class Collect
    {
        public event EventHandler<SendSecuritiesAPI> Send;
        public void ToCollect(string time, StringBuilder data)
        {
            Collection.Push(new Catalog.Request.Collect
            {
                Date = time,
                Datum = data.ToString()
            });
            if (Collection.Count > 0 && Collection.Count % 0x9C4 == 0)
                Send?.Invoke(this, new SendSecuritiesAPI(Clone, code.Substring(0, 7)));
        }
        public void SendTransmitCommand(string code)
        {
            if (this.code.Equals(code))
                Send?.Invoke(this, new SendSecuritiesAPI(code.Substring(0, 7), FinalClone));
        }
        public uint GetTime(char time)
        {
            if (Time.Equals(time) == false)
            {
                Time = time;
                Index = 0;
            }
            return Index++;
        }
        public Collect(string code)
        {
            this.code = code;
            Time = 'A';
            Collection = new Stack<Catalog.Request.Collect>(0x9C5);
        }
        public int Count => Collection.Count;
        char Time
        {
            get; set;
        }
        uint Index
        {
            get; set;
        }
        Stack<Catalog.Request.Collect> FinalClone
        {
            get
            {
                var stack = new Stack<Catalog.Request.Collect>();

                while (Collection.Count > 0)
                    stack.Push(Collection.Pop());

                return stack;
            }
        }
        Queue<Catalog.Request.Collect> Clone
        {
            get
            {
                var queue = new Queue<Catalog.Request.Collect>();

                while (Collection.Count > 0)
                    queue.Enqueue(Collection.Pop());

                return queue;
            }
        }
        Stack<Catalog.Request.Collect> Collection
        {
            get;
        }
        readonly string code;
    }
}