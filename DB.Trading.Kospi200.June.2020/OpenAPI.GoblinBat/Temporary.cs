using System.Text;
using ShareInvest.EventHandler;
using ShareInvest.EventHandler.OpenAPI;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.OpenAPI
{
    public class Temporary : CallUp
    {
        public Temporary(ConnectAPI api, StringBuilder sb, char initial) : base(initial)
        {
            this.initial = initial;
            Temp = sb;
            api.SendQuotes += OnReceiveMemorize;
            api.SendDatum += OnReceiveMemorize;
        }
        private Temporary(ConnectAPI api, char initial) : base(initial)
        {
            Temp = new StringBuilder(1024);
            api.SendMemorize += OnReceiveMemorize;
        }
        internal void SetConnection(ConnectAPI api)
        {
            api.SendQuotes -= OnReceiveMemorize;
            api.SendDatum -= OnReceiveMemorize;
            new Temporary(api, initial);
        }
        internal void SetStorage(string code)
        {
            SetStorage(code, Temp);
        }
        private void OnReceiveMemorize(object sender, Datum e)
        {
            if (e.Time != null && e.Price > 0 && e.Volume != 0)
                Temp.Append(e.Time).Append(';').Append(e.Price).Append(',').Append(e.Volume).Append('*');
        }
        private void OnReceiveMemorize(object sender, Quotes e)
        {
            if (e.Total.Equals(string.Empty) == false && int.TryParse(e.Time.Substring(0, 4), out int time) && time < 1535 && time > 859 && e.Price[4] > 0 && e.Price[5] > 0)
            {
                var total = e.Total.Split(';');
                Temp.Append(e.Time).Append(';').Append(e.Price[4]).Append(',').Append(e.Quantity[4]).Append(',').Append(total[0]).Append(',').Append(e.Price[5]).Append(',').Append(e.Quantity[5]).Append(',').Append(total[1]).Append('*');
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
        private readonly char initial;
    }
}