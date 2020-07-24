using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ShareInvest.DelayRequest
{
    public class Delay
    {
        public static Delay GetInstance(int milliseconds)
        {
            if (request == null)
            {
                Milliseconds = milliseconds;
                request = new Delay();
            }
            return request;
        }
        public static int Milliseconds
        {
            get; set;
        }
        public void Run() => taskWorker.Start();
        public void Dispose() => requestTaskQueue.Clear();
        public void RequestTrData(Task task) => requestTaskQueue.Enqueue(task);
        Delay()
        {
            requestTaskQueue = new Queue<Task>();
            taskWorker = new Thread(delegate ()
            {
                while (true)
                    try
                    {
                        while (requestTaskQueue.Count > 0)
                        {
                            requestTaskQueue.Dequeue().RunSynchronously();
                            Thread.Sleep(Milliseconds);
                        }
                        Thread.Sleep(0xC);
                    }
                    catch (Exception ex)
                    {
                        SendMessage(ex.StackTrace, ex.TargetSite.Name);
                    }
            });
        }
        static Delay request;
        readonly Thread taskWorker;
        readonly Queue<Task> requestTaskQueue;
        [Conditional("DEBUG")]
        void SendMessage(string message, string name) => Console.WriteLine(name + "\n" + message);
    }
}