using System.Collections.Generic;
using ShareInvest.Interface;

namespace ShareInvest.Const
{
    public class MakeUpFor3Months : IMakeUp
    {
        public MakeUpFor3Months()
        {
            FindByName = "for3months";
            Turn = 61;
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