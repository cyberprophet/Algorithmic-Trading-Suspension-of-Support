using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public struct Retention : IRetention
    {
        public string Code
        {
            get; set;
        }
        public string LastDate
        {
            get; set;
        }
    }
}