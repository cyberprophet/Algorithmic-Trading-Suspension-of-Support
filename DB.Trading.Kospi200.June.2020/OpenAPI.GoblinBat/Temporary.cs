using System.Collections.Generic;
using System.Text;
using ShareInvest.EventHandler.OpenAPI;
using ShareInvest.EventHandler;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.OpenAPI
{
    public class Temporary : CallUp
    {
        public Temporary(ConnectAPI api, Queue<string> quotes)
        {
            this.quotes = quotes;
            api.SendQuotes += OnReceiveMemorize;
            api.SendDatum += OnReceiveMemorize;
        }
        internal Temporary(ConnectAPI api)
        {
            Temp = new StringBuilder(1024);
            api.SendMemorize += OnReceiveMemorize;
        }
        internal void SetConnection(ConnectAPI api)
        {
            api.SendQuotes -= OnReceiveMemorize;
            api.SendDatum -= OnReceiveMemorize;
        }
        internal void SetStorage(string code)
        {
            SetStorage(code, quotes);
        }
        private void OnReceiveMemorize(object sender, Datum e)
        {
            if (e.Time != null && e.Price > 0 && e.Volume != 0)
                quotes.Enqueue(string.Concat(e.Time, ';', e.Price, '^', e.Volume));
        }
        private void OnReceiveMemorize(object sender, Quotes e)
        {
            if (e.Total.Equals(string.Empty) == false && int.TryParse(e.Time.Substring(0, 4), out int time) && time < 1535 && time > 859 && e.Price[4] > 0 && e.Price[5] > 0)
            {
                var total = e.Total.Split(';');
                quotes.Enqueue(string.Concat(e.Time, ';', e.Price[4], '^', e.Quantity[4], '^', total[0], '*', e.Price[5], '^', e.Quantity[5], '^', total[1]));
            }
        }
        private void OnReceiveMemorize(object sender, Memorize e)
        {
            if (e.SPrevNext != null)
            {
                if (!e.SPrevNext.Equals("Clear"))
                    SetStorage(e.Code, Temp.ToString().Split(';'));

                Temp = new StringBuilder(1024);

                return;
            }
            Temp.Append(string.Concat(e.Date, ",", e.Price, ",", e.Volume)).Append(';');
        }
        private StringBuilder Temp
        {
            get; set;
        }
        private readonly Queue<string> quotes;
    }
}