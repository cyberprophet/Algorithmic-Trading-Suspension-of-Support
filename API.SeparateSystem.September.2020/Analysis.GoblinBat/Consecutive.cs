using System.Collections.Generic;

using ShareInvest.Catalog;

namespace ShareInvest.Analysis
{
    class Consecutive
    {
        protected internal Stack<double> Short
        {
            get;
        }
        protected internal Stack<double> Long
        {
            get;
        }
        protected internal Consecutive(TrendFollowingBasicFutures strategics)
        {
            tf = strategics;
        }
        void DrawChart(Charts chart)
        {
            if (GetCheckOnTime(chart.Date))
            {

            }
        }
        bool GetCheckOnTime(string date)
        {

            return false;
        }
        readonly TrendFollowingBasicFutures tf;
    }
}