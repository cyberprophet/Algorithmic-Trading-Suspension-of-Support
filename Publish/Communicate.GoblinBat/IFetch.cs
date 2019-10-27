using System.Collections.Generic;

namespace ShareInvest.Communicate
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