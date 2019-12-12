using System.Collections.Generic;

namespace ShareInvest.Interface
{
    public interface IFetch
    {
        List<string> DayChart
        {
            get;
        }
        List<string> TickChart
        {
            get;
        }
    }
}