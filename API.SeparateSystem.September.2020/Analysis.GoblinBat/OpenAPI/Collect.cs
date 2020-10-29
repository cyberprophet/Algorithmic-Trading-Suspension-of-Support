using System;
using System.Collections.Generic;
using System.Text;

using ShareInvest.EventHandler;

namespace ShareInvest.Analysis.OpenAPI
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
            if (Collection.Count > 0 && Collection.Count % 0xFA0 == 0)
                SendTransmitCommand(new SendSecuritiesAPI(Clone, code));
        }
        public void SendTransmitCommand(string code)
        {
            if (this.code.Equals(code))
                Send?.Invoke(this, new SendSecuritiesAPI(code, Collection));
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
            Collection = new Stack<Catalog.Request.Collect>();
        }
        char Time
        {
            get; set;
        }
        uint Index
        {
            get; set;
        }
        void SendTransmitCommand(SendSecuritiesAPI collection) => Send?.Invoke(this, collection);
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