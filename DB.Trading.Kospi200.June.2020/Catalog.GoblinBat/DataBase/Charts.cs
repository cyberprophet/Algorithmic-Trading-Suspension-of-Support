using System.Collections.Generic;

namespace ShareInvest.Catalog.DataBase
{
    public struct Charts
    {
        public int Time
        {
            get; set;
        }
        public int Short
        {
            get; set;
        }
        public Stack<double> ShortValue
        {
            get; set;
        }
        public int Long
        {
            get; set;
        }
        public Stack<double> LongValue
        {
            get; set;
        }
    }
}