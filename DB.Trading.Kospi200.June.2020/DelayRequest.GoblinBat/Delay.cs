using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShareInvest.Message;

namespace ShareInvest.DelayRequest
{
    public class Delay
    {
        public static Delay GetInstance(int mSecond)
        {
            delay = mSecond;

            if (request == null)
                request = new Delay();

            return request;
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
                            Thread.Sleep(delay);
                        }
                        Thread.Sleep(5);
                    }
                    catch (Exception ex)
                    {
                        new ExceptionMessage(ex.StackTrace);

                        if (taskWorker.IsAlive)
                            continue;

                        request = new Delay();
                    }
                }
            });
        }
        public static int delay = 205;
        private static Delay request;
        private readonly Thread taskWorker;
        private readonly Queue<Task> requestTaskQueue = new Queue<Task>();
    }
}