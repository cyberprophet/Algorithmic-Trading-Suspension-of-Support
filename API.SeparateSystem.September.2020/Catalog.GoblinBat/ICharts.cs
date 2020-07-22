namespace ShareInvest.Catalog
{
    public interface ICharts
    {
        string Code
        {
            get; set;
        }
        string Retention
        {
            get; set;
        }
        string Date
        {
            get; set;
        }
        string Price
        {
            get; set;
        }
        int Volume
        {
            get; set;
        }
    }
}