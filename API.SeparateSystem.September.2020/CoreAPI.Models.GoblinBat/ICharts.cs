namespace ShareInvest.Models
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
        long Date
        {
            get; set;
        }
        string Price
        {
            get; set;
        }
    }
}