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
        public int QueueCount
        {
            get
            {
                return requestTaskQueue.Count;
            }
        }
        public void Run()
        {
            taskWorker.Start();
        }
        public void RequestTrData(Task task)
        {
            requestTaskQueue.Enqueue(task);
        }
        private Delay()
        {
            taskWorker = new Thread(delegate ()
            {
                while (true)
                {
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
                }
            });
        }
        private static Delay request;
        private readonly Thread taskWorker;
        private readonly Queue<Task> requestTaskQueue = new Queue<Task>();
    }
}