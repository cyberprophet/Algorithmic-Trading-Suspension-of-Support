using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShareInvest.OpenAPI;

namespace ShareInvest.Strategy.OpenAPI
{
    public class Consecutive
    {
        public Consecutive(string key, string[] codes)
        {
            foreach (var code in codes)
                stocks.Add(new Stocks(key, code));

            API.SendStocksDatum += OnReceiveStocksDatum;
            API.OnReceiveBalance = false;
        }
        void OnReceiveStocksDatum(object sender, EventHandler.OpenAPI.Stocks e)
        {
            if (API.OnReceiveBalance)
                new Task(() => stocks.First(o => o.Code.Equals(e.Code)).DrawChart(e.Time, e.Price)).Start();
        }
        ConnectAPI API => ConnectAPI.GetInstance();
        readonly HashSet<Stocks> stocks = new HashSet<Stocks>();
    }
}