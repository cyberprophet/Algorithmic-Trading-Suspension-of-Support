namespace ShareInvest.Models
{
    public interface ICharts<T>
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
        T Price
        {
            get; set;
        }
    }
}