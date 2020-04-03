using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShareInvest.Message;

namespace ShareInvest.DelayRequest
{
    public class Delay
    {
        public static Delay GetInstance(int milliseconds)
        {
            Milliseconds = milliseconds;

            if (request == null)
                request = new Delay();

            return request;
        }
        public static int Milliseconds
        {
            get; set;
        }
        public int QueueCount => requestTaskQueue.Count;
        public void Run() => taskWorker.Start();
        public void RequestTrData(Task task) => requestTaskQueue.Enqueue(task);
        Delay() => taskWorker = new Thread(delegate ()
        {
            while (true)
                try
                {
                    while (requestTaskQueue.Count > 0)
                    {
                        requestTaskQueue.Dequeue().RunSynchronously();
                        Thread.Sleep(Milliseconds);
                    }
                    Thread.Sleep(5);
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
        });
        static Delay request;
        readonly Thread taskWorker;
        readonly Queue<Task> requestTaskQueue = new Queue<Task>();
    }
}