using System.Collections.Generic;

namespace ShareInvest.Catalog
{
    public class Convert
    {
        public Queue<Futures> ToStoreInFutures(string code, Stack<string> stack)
        {
            var queue = new Queue<Futures>();
            string temp = string.Empty;
            int count = 0;

            while (stack.Count > 0)
            {
                var arg = stack.Pop().Split(';');
                var index = (temp.Equals(arg[0]) ? ++count : count = 0).ToString("D3");

                if (int.TryParse(arg[2], out int volume))
                {
                    queue.Enqueue(new Futures
                    {
                        Code = code,
                        Date = string.Concat(arg[0], index),
                        Price = arg[1],
                        Volume = volume,
                        Retention = stack.Count > 0 ? string.Empty : arg[0]
                    });
                    temp = arg[0];
                }
            }
            return queue;
        }
        public Queue<Stocks> ToStoreInStocks(string code, Stack<string> stack)
        {
            var queue = new Queue<Stocks>();
            string temp = string.Empty;
            int count = 0;

            while (stack.Count > 0)
            {
                var arg = stack.Pop().Split(';');
                var index = (temp.Equals(arg[0]) ? ++count : count = 0).ToString("D3");

                if (int.TryParse(arg[2], out int volume))
                {
                    queue.Enqueue(new Stocks
                    {
                        Code = code,
                        Date = string.Concat(arg[0], index),
                        Price = arg[1],
                        Volume = volume,
                        Retention = stack.Count > 0 ? string.Empty : arg[0]
                    });
                    temp = arg[0];
                }
            }
            return queue;
        }
    }
}