using System.Text;
using ShareInvest.Catalog;
using ShareInvest.EventHandler.XingAPI;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.XingAPI
{
    public partial class Temporary : CallUp
    {
        public Temporary(IReals quotes, IReals datum, StringBuilder sb, string key) : base(key)
        {
            this.sb = sb;
            ((IEvents<Quotes>)quotes).Send += OnReceiveMemorize;
            ((IEvents<Datum>)datum).Send += OnReceiveMemorize;
        }
        void OnReceiveMemorize(object sender, Datum e)
        {
            if (e.Time != null && e.Price > 0 && e.Volume != 0)
                sb.Append(e.Time).Append(';').Append(e.Price).Append(',').Append(e.Volume).Append('*');
        }
        void OnReceiveMemorize(object sender, Quotes e)
        {
            int sell = e.Sell - Sell, buy = e.Buy - Buy;

            if (e.Price[4] > 0 && e.Price[5] > 0)
                sb.Append(e.Time).Append(';').Append(e.Price[4]).Append(',').Append(e.Quantity[4]).Append(',').Append(sell).Append(',').Append(e.Price[5]).Append(',').Append(e.Quantity[5]).Append(',').Append(buy).Append('*');

            Sell = e.Sell;
            Buy = e.Buy;
        }
        int Sell
        {
            get; set;
        }
        int Buy
        {
            get; set;
        }
        public void SetStorage(string code) => SetStorage(code, sb);
        readonly StringBuilder sb;
    }
}