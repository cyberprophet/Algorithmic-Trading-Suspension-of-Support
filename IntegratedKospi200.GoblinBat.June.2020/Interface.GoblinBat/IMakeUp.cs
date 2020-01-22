using System.Collections.Generic;

namespace ShareInvest.Interface
{
    public interface IMakeUp
    {
        Dictionary<string, long> DescendingSort
        {
            get; set;
        }
        string FindByName
        {
            get;
        }
        int Turn
        {
            get; set;
        }
    }
}