using System.Collections.Generic;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis
{
    class Consecutive
    {
        internal Stack<double> Short
        {
            get;
        }
        internal Stack<double> Long
        {
            get;
        }
        internal Consecutive(TrendFollowingBasicFutures strategics, Holding ho)
        {
            Short = new Stack<double>();
            Long = new Stack<double>();
            tf = strategics;
            this.ho = ho;
            ho.Send += OnReceiveDrawChart;
        }
        internal void Dispose() => ho.Send -= OnReceiveDrawChart;
        internal void Connect(Holding holding) => holding.Send += OnReceiveDrawChart;
        void OnReceiveDrawChart(object sender, SendConsecutive e)
        {
            if (GetCheckOnTime(e.Date))
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