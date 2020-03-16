using ShareInvest.Catalog;

namespace ShareInvest.Strategy.XingAPI
{
    public class Quotes : Trading
    {
        public Quotes(IEvents<EventHandler.XingAPI.Quotes> quotes, IEvents<EventHandler.XingAPI.Datum> xing, Specify specify) : base(xing, specify)
        {

        }
    }
}