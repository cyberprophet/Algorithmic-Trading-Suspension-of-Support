using System.Collections.Generic;

namespace ShareInvest.Communication
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