using System.Collections.Generic;
using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class MakeUpWeekly : IMakeUp
    {
        public MakeUpWeekly()
        {
            FindByName = "weekly";
            Turn = 6;
            DescendingSort = new Dictionary<string, long>(256);
        }
        public Dictionary<string, long> DescendingSort
        {
            get; set;
        }
        public string FindByName
        {
            get; private set;
        }
        public int Turn
        {
            get; private set;
        }
    }
}