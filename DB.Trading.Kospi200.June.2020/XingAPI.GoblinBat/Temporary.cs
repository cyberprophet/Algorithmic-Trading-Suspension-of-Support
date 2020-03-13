using System.Collections.Generic;
using ShareInvest.Catalog;
using ShareInvest.EventHandler.XingAPI;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.XingAPI
{
    public partial class Temporary : CallUp
    {
        public Temporary(IReal quotes, IReal datum, Queue<string> queue)
        {
            this.queue = queue;
            ((IEvent<Quotes>)quotes).Send += OnReceiveMemorize;
            ((IEvent<Datum>)datum).Send += OnReceiveMemorize;
        }
        internal void SetStorage(string code)
        {
            SetStorage(code, queue);
        }
        private void OnReceiveMemorize(object sender, Datum e)
        {
            if (e.Time != null && e.Price > 0 && e.Volume != 0)
                queue.Enqueue(string.Concat(e.Time, ';', e.Price, '^', e.Volume));
        }
        private void OnReceiveMemorize(object sender, Quotes e)
        {
            int sell = e.Sell - Sell, buy = e.Buy - Buy;

            if (e.Price[4] > 0 && e.Price[5] > 0)
                queue.Enqueue(string.Concat(e.Time, ';', e.Price[4], '^', e.Quantity[4], '^', sell, '*', e.Price[5], '^', e.Quantity[5], '^', buy));

            Sell = e.Sell;
            Buy = e.Buy;
        }
        private int Sell
        {
            get; set;
        }
        private int Buy
        {
            get; set;
        }
        private readonly Queue<string> queue;
    }
}