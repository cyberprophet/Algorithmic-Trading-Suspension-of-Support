using System.Collections.Generic;

using ShareInvest.Catalog;

namespace ShareInvest.Analysis
{
    class Consecutive
    {     
        internal Consecutive(TrendFollowingBasicFutures strategics, Holding ho)
        {
            tf = strategics;
            this.ho = ho;
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
        readonly Holding ho;
    }
}