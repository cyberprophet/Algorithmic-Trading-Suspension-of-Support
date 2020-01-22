using System.Collections.Generic;
using ShareInvest.Interface;

namespace ShareInvest.Const
{
    public class MakeUpBiweekly : IMakeUp
    {
        public MakeUpBiweekly()
        {
            FindByName = "biweekly";
            Turn = 10;
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
            get; set;
        }
    }
}